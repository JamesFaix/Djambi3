﻿module Djambi.Api.Logic.Services.IndirectEffectsService

open System.Collections.Generic
open System.Linq
open Djambi.Api.Common.Collections
open Djambi.Api.Common.Control
open Djambi.Api.Logic.ModelExtensions.BoardModelExtensions
open Djambi.Api.Logic.ModelExtensions.GameModelExtensions
open Djambi.Api.Logic.PieceStrategies
open Djambi.Api.Logic.Services
open Djambi.Api.Model

let private removeSequentialDuplicates(turnCycle : int list) : int list =
    if turnCycle.Length = 1 then turnCycle
    else
        let list = turnCycle.ToList()
        for i in [(list.Count-1)..1] do
            if list.[i] = list.[i-1]
            then list.RemoveAt(i)
        list |> Seq.toList

let private getAbandonPiecesEffects (game : Game, oldPlayerId : int) : Effect list =
    game.pieces 
    |> List.filter (fun piece -> piece.playerId = Some oldPlayerId)
    |> List.map (fun p -> Effect.PieceAbandoned { oldPiece = p })

let private getEnlistPiecesEffects (game : Game, oldPlayerId : int option, newPlayerId : int) : Effect list =
    game.pieces 
    |> List.filter (fun piece -> piece.playerId = oldPlayerId)
    |> List.map (fun p -> Effect.PieceEnlisted { oldPiece = p; newPlayerId = newPlayerId })
    
let private getEliminatePlayerEffects (game : Game) (playerId : int) (killingPlayerId : int option) : Effect list =
    let p = game.players |> List.find (fun p -> p.id = playerId)
    
    let effects = new ArrayList<Effect>()
    
    effects.Add(Effect.PlayerStatusChanged {
        playerId = p.id
        oldStatus = p.status
        newStatus = Eliminated
    })

    let newCycle = 
        game.turnCycle 
        |> List.filter (fun pId -> pId <> p.id) 
        |> removeSequentialDuplicates

    effects.Add(Effect.TurnCyclePlayerRemoved {
        playerId = p.id
        oldValue = game.turnCycle
        newValue = newCycle
    })

    match killingPlayerId with
    | Some kpId -> 
        effects.AddRange (getEnlistPiecesEffects (game, Some p.id, kpId))
    | None ->
        effects.AddRange (getAbandonPiecesEffects (game, p.id))

    effects |> Seq.toList

let private getRiseOrFallFromPowerEffects (game : Game, updatedGame : Game) : Effect list =
    //Copy only the last turn of the given player, plus all other turns
    let removeBonusTurnsForPlayer playerId turns =
        let stack = new Stack<int>()
        let mutable hasAddedTargetPlayer = false
        for t in turns |> Seq.rev do
            if t = playerId && not hasAddedTargetPlayer
            then
                hasAddedTargetPlayer <- true
                stack.Push t
            else stack.Push t
        stack |> Seq.toList |> removeSequentialDuplicates

    //Insert a turn for the given player before every turn, except the current one
    let addBonusTurnsForPlayer playerId turns =
        let stack = new Stack<int>()
        for t in turns |> Seq.skip(1) |> Seq.rev do
            stack.Push t
            stack.Push playerId
        stack |> Seq.toList |> removeSequentialDuplicates

    //A player in power has multiple turns
    let hasPower (g : Game) (pId : int) =
        let turns = 
            g.turnCycle 
            |> Seq.filter (fun n -> n = pId) 
            |> Seq.length 
        turns > 1

    //Players can only rise to power if there are more than 2 left
    let powerCanBeHad (g : Game) =
        let players = 
            g.turnCycle 
            |> Seq.distinct 
            |> Seq.length 
        players > 2

    let turn = updatedGame.currentTurn.Value
    let subject = (turn.subjectPiece game).Value
    let destination = (turn.destinationCell game.parameters.regionCount).Value
    let origin = (turn.subjectCell game.parameters.regionCount).Value
    let subjectStrategy = PieceService.getStrategy subject

    let effects = new ArrayList<Effect>()
    let mutable turns = updatedGame.turnCycle

    let subjectPlayerId = subject.playerId.Value
    if subjectStrategy.canStayInCenter 
    then
        if origin.isCenter 
            && not destination.isCenter
            && hasPower updatedGame subjectPlayerId
        then 
            //If chief subject leaves power, remove bonus turns
            let newTurns = removeBonusTurnsForPlayer subjectPlayerId turns
            effects.Add(Effect.TurnCyclePlayerFellFromPower { playerId = subjectPlayerId; oldValue = turns; newValue = newTurns })
            turns <- newTurns
        elif not origin.isCenter 
            && destination.isCenter
            && powerCanBeHad updatedGame
        then 
            //If chief subject rises to power, add bonus turns
            let newTurns = addBonusTurnsForPlayer subjectPlayerId turns
            effects.Add(Effect.TurnCyclePlayerRoseToPower { playerId = subjectPlayerId; oldValue = turns; newValue = newTurns })
            turns <- newTurns
        else ()

    match (turn.targetPiece game, turn.dropCell game.parameters.regionCount) with
    | (Some target, Some drop) ->
        let targetStrategy = PieceService.getStrategy target
        
        if subjectStrategy.canEnterCenterToEvictPiece 
            && not subjectStrategy.killsTarget
            && subjectStrategy.canDropTarget
            && targetStrategy.canStayInCenter
        then
            //If chief target is moved out of power and not killed, remove bonus turns
            if destination.isCenter 
                && not drop.isCenter
                && hasPower updatedGame subjectPlayerId
            then
                let newTurns = removeBonusTurnsForPlayer subjectPlayerId turns
                effects.Add(Effect.TurnCyclePlayerFellFromPower { playerId = subjectPlayerId; oldValue = turns; newValue = newTurns })
                turns <- newTurns
            //If chief target is dropped in power and not killed, add bonus turns
            elif not destination.isCenter 
                && drop.isCenter
                && powerCanBeHad updatedGame
            then
                let newTurns = addBonusTurnsForPlayer subjectPlayerId turns
                effects.Add(Effect.TurnCyclePlayerRoseToPower { playerId = subjectPlayerId; oldValue = turns; newValue = newTurns })
                turns <- newTurns
            else ()
        else ()
    | _ -> ()

    effects |> Seq.toList

let private getVictoryEffects (game : Game) : Effect list =
    let remainingPlayers = game.players |> List.filter (fun p -> p.status = Alive && p.userId.IsSome)
    if remainingPlayers.Length = 1        
    then
        let p = remainingPlayers.[0]
        [
            Effect.PlayerStatusChanged { oldStatus = p.status; newStatus = Victorious; playerId = p.id}
            Effect.GameStatusChanged { oldValue = game.status; newValue = Finished }
            Effect.CurrentTurnChanged { oldValue = game.currentTurn; newValue = None }
        ]
    else []

let private getOutOfMovesEffects (game : Game) : Effect list =
    let effects = new ArrayList<Effect>()
    let mutable game = game
    let mutable selectionOptions = (SelectionOptionsService.getSelectableCellsFromState game) |> Result.value
   
    while selectionOptions.IsEmpty && game.turnCycle.Length > 1 do
        let currentPlayerId = game.turnCycle.[0]
        let fx = [Effect.PlayerOutOfMoves { playerId = currentPlayerId }]
        let fx = List.append fx (getEliminatePlayerEffects game currentPlayerId None)
        effects.AddRange(fx)
        game <- EventService.applyEffects fx game
        selectionOptions <- (SelectionOptionsService.getSelectableCellsFromState) game |> Result.value

    effects |> Seq.toList

let private getTernaryEffects (game : Game) : Effect list =
    let effects = new ArrayList<Effect>()
    let game = { game with currentTurn = Some Turn.empty } //This is required so that selection options come back

    let victoryEffects = getVictoryEffects game
    effects.AddRange victoryEffects

    if victoryEffects.IsEmpty
    then
        let outOfMovesEffects = getOutOfMovesEffects game
        effects.AddRange outOfMovesEffects
        let game = EventService.applyEffects outOfMovesEffects game

        //Check for victory caused by players being out of moves
        let victoryEffects = getVictoryEffects game
        effects.AddRange victoryEffects
        let game = EventService.applyEffects victoryEffects game

        if victoryEffects.IsEmpty
        then 
            let seletionOptions = SelectionOptionsService.getSelectableCellsFromState game |> Result.value
            let turn = { Turn.empty with selectionOptions = seletionOptions }
            effects.Add(Effect.CurrentTurnChanged { oldValue = game.currentTurn; newValue = Some turn })
        else ()
    else ()
    effects |> Seq.toList

let getIndirectEffectsForTurnCommit (game : Game, updatedGame : Game) : Effect list =
    (*
        The order of effects is important, both for the implementation and clarity of the event log to users.

        [Eliminate target's player] (option)
            Change player status
            Remove player from turn cycle
            Enlist pieces controlled by player
        Enlist pieces if killing neutral Chief (option)
        Player rises/falls from power (option)

        Victory (option)
        Advance turn cycle
        [Eliminate player out of moves] (option, repeat as necessary)
            Change player status
            Remove from turn cycle
            Abandon pieces
        Victory due to out-of-moves (option)
        Current turn changed   
    *)

    let effects = new ArrayList<Effect>()

    let turn = updatedGame.currentTurn.Value
    let subject = (turn.subjectPiece game).Value
    let subjectStrategy = PieceService.getStrategy subject

    let killChiefEffects = 
        match turn.targetPiece game with
        | Some target ->
            let targetStrategy = PieceService.getStrategy target

            if subjectStrategy.killsTarget 
                && targetStrategy.killsControllingPlayerWhenKilled
            then 
                if target.playerId.IsSome
                then getEliminatePlayerEffects game target.playerId.Value subject.playerId
                else getEnlistPiecesEffects (game, Some target.originalPlayerId, subject.playerId.Value)
            else []
        | _ -> []

    effects.AddRange killChiefEffects
    let updatedGame = EventService.applyEffects killChiefEffects updatedGame

    let riseFallEffects = getRiseOrFallFromPowerEffects (game, updatedGame)
    effects.AddRange riseFallEffects
    let updatedGame = EventService.applyEffects riseFallEffects updatedGame

    effects.AddRange (getTernaryEffects updatedGame)

    effects |> Seq.toList

let getIndirectEffectsForConcede (game : Game, request : PlayerStatusChangeRequest) : Effect list =
    (*
        The order of effects is important, both for the implementation and clarity of the event log to users.

        Remove player from turn cycle
        Abandon pieces controlled by player
        Victory/draw (option)
        Reset current turn
        [Eliminate player out of moves] (option, repeat as necessary)
            Change player status
            Remove from turn cycle
            Abandon pieces
        Victory due to out-of-moves (option)
        Current turn changed   
    *)
    let effects = new ArrayList<Effect>()
    
    effects.Add (Effect.TurnCyclePlayerRemoved { 
        playerId = request.playerId
        oldValue = game.turnCycle
        newValue = game.turnCycle |> List.filter (fun pId -> pId <> request.playerId)
    })

    effects.AddRange (getAbandonPiecesEffects(game, request.playerId))

    let game = EventService.applyEffects effects game
    
    effects.AddRange (getTernaryEffects game)

    effects |> Seq.toList
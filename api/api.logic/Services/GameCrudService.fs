﻿module Djambi.Api.Logic.Services.GameCrudService

open Djambi.Api.Common.Collections
open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.Model
    
let createGame (parameters : GameParameters) (session : Session) : Game AsyncHttpResult =
    let gameRequest : CreateGameRequest =   
        {
            parameters = parameters
            createdByUserId = session.userId
        }

    let playerRequest : CreatePlayerRequest =
        {
            kind = PlayerKind.User
            userId = Some session.userId
            name = None
        }

    GameRepository.createGameAndAddPlayer (gameRequest, playerRequest)
    |> thenBindAsync GameRepository.getGame

let getUpdateGameParametersEvent (game : Game, parameters : GameParameters) (session : Session) : CreateEventRequest HttpResult =
    if game.status <> GameStatus.Pending
    then Error <| HttpException (400, "Cannot change game parameters unless game is Pending.")
    elif not (session.isAdmin || session.userId = game.createdByUserId)
    then Error <| HttpException(403, "Cannot change game parameters of game created by another user.")
    else 
        let effects = new ArrayList<Effect>()

        effects.Add(Effect.parametersChanged(game.parameters, parameters))

        //If lowering region count, extra players are ejected
        let truncatedPlayers = 
            game.players 
            |> List.skipSafe parameters.regionCount 

        //If disabling AllowGuests, guests are ejected
        let ejectedGuests =
            if parameters.allowGuests = false
            then game.players
                |> Seq.filter (fun p -> p.kind = PlayerKind.Guest)
            else Seq.empty

        let removedPlayerIds = 
            truncatedPlayers 
            |> Seq.append ejectedGuests 
            |> Seq.map (fun p -> p.id)
            |> Seq.distinct
            |> Seq.toList

        if removedPlayerIds.Length > 0
        then effects.Add(Effect.playersRemoved removedPlayerIds)
        else ()

        {
            kind = EventKind.GameParametersChanged
            effects = effects |> Seq.toList
            createdByUserId = session.userId
        }
        |> Ok
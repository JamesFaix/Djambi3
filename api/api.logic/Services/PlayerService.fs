﻿module Djambi.Api.Logic.Services.PlayerService

open System
open System.Linq
open Djambi.Api.Common
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.Model

let getLobbyPlayers (lobbyId : int) (session : Session) : Player list AsyncHttpResult =
    PlayerRepository.getPlayersForLobby lobbyId

let getGamePlayers (gameId : int) (session : Session) : Player list AsyncHttpResult =
    PlayerRepository.getPlayersForGame gameId

let addPlayerToLobby (lobbyId : int, request : CreatePlayerRequest) (session : Session) : Player AsyncHttpResult =
    LobbyRepository.getLobby lobbyId
    |> thenBind (fun lobby ->
        match request.kind with
        | PlayerKind.User ->
            if request.userId.IsNone
            then Error <| HttpException(400, "UserID must be provided when adding a user player.")
            elif request.name.IsSome
            then Error <| HttpException(400, "Cannot provide name when adding a user player.")
            elif not session.isAdmin && request.userId.Value <> session.userId
            then Error <| HttpException(403, "Cannot add other users to a lobby.")
            else Ok ()

        | PlayerKind.Guest ->
            if not lobby.allowGuests
            then Error <| HttpException(400, "Lobby does not allow guest players.")
            elif request.userId.IsNone
            then Error <| HttpException(400, "UserID must be provided when adding a guest player.")
            elif request.name.IsNone
            then Error <| HttpException(400, "Must provide name when adding a guest player.")
            elif not session.isAdmin && request.userId.Value <> session.userId
            then Error <| HttpException(403, "Cannot add guests for other users to a lobby.")
            else Ok ()

        | PlayerKind.Neutral ->
            Error <| HttpException(400, "Cannot directly add neutral players to a lobby.")
    )
    |> thenBindAsync (fun _ -> PlayerRepository.addPlayerToLobby (lobbyId, request))

let removePlayerFromLobby (lobbyId : int, playerId : int) (session : Session) : Unit AsyncHttpResult =
    LobbyRepository.getLobby lobbyId //Will error if game already started
    |> thenBindAsync (fun lobby ->
        PlayerRepository.getPlayersForLobby lobbyId
        |> thenBind (fun players ->
            match players |> List.tryFind (fun p -> p.id = playerId) with
            | None -> Error <| HttpException(404, "Player not found.")
            | Some p ->
                match p.userId with
                | None -> Error <| HttpException(400, "Cannot remove neutral players from lobby.")
                | Some x ->
                    if session.isAdmin
                        || lobby.createdByUserId = session.userId
                        || x = session.userId
                    then Ok ()
                    else Error <| HttpException(403, "Cannot remove other users from lobby.")
        )
    )
    |> thenBindAsync (fun _ -> PlayerRepository.removePlayerFromLobby playerId)

let fillEmptyPlayerSlots (lobby : GameParameters) (players : Player list) : Player list AsyncHttpResult =
    let missingPlayerCount = lobby.regionCount - players.Length

    let getNeutralPlayerNamesToUse (possibleNames : string list) =
        Enumerable.Except(
            possibleNames,
            players |> Seq.map (fun p -> p.name),
            StringComparer.OrdinalIgnoreCase)
        |> Utilities.shuffle
        |> Seq.take missingPlayerCount

    if missingPlayerCount = 0
    then players |> okTask
    else
        PlayerRepository.getNeutralPlayerNames()
        |> thenMap getNeutralPlayerNamesToUse
        |> thenDoEachAsync (fun name ->
            let request = CreatePlayerRequest.neutral (name)
            PlayerRepository.addPlayerToLobby (lobby.id, request)
            |> thenMap ignore
        )
        |> thenBindAsync (fun _ -> PlayerRepository.getPlayersForLobby lobby.id)
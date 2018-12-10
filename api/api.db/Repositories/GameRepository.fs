﻿namespace Djambi.Api.Db.Repositories

open Dapper
open Newtonsoft.Json
open Djambi.Api.Common
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Db.Mapping
open Djambi.Api.Db.Model
open Djambi.Api.Db.SqlUtility
open Djambi.Api.Model

module GameRepository =
    
    let startGame(lobbyId : int,
                  startingConditions : PlayerStartConditions list,
                  gameState : GameState,
                  turnState : Turn) : int AsyncHttpResult =
        let startingConditionsJson = startingConditions |> JsonConvert.SerializeObject
        let gameStateJson = gameState |> JsonConvert.SerializeObject
        let turnStateJson = turnState |> JsonConvert.SerializeObject

        let param = DynamicParameters()
                        .add("LobbyId", lobbyId)
                        .add("StartingConditionsJson", startingConditionsJson)
                        .add("GameStateJson", gameStateJson)
                        .add("TurnStateJson", turnStateJson)

        let cmd = proc("Games_Start", param)
        querySingle<int>(cmd, "Game")

    let getGame(gameId : int) : Game AsyncHttpResult =
        let param = DynamicParameters()
                        .add("GameId", gameId)
        let cmd = proc("Games_Get", param)

        querySingle<GameSqlModel>(cmd, "Game")
        |> thenMap mapGameSqlModelResponse

    let updateGameState(gameId : int, state : GameState) : Unit AsyncHttpResult =
        let json = state |> JsonConvert.SerializeObject
        let param = DynamicParameters()
                        .add("GameId", gameId)
                        .add("GameStateJson", json)

        let cmd = proc("Games_UpdateGameState", param)
        queryUnit(cmd, "Game")

    let updateTurnState(gameId: int, state : Turn) : Unit AsyncHttpResult =
        let json = state |> JsonConvert.SerializeObject
        let param = DynamicParameters()
                        .add("GameId", gameId)
                        .add("TurnStateJson", json)
        
        let cmd = proc("Games_UpdateTurnState", param)
        queryUnit(cmd, "Game")
﻿namespace Apex.Api.Web.Controllers

open System.Threading.Tasks
open Microsoft.AspNetCore.Mvc
open FSharp.Control.Tasks
open Serilog
open Apex.Api.Common.Control.AsyncHttpResult
open Apex.Api.Logic.Interfaces
open Apex.Api.Model
open Apex.Api.Web
open Apex.Api.Common

[<ApiController>]
[<Route("api/games/{gameId}/players")>]
type PlayerController(manager : IPlayerManager,
                       logger : ILogger,
                       scp : SessionContextProvider) =
    inherit ControllerBase()
    
    [<HttpPost>]
    [<ProducesResponseType(200, Type = typeof<StateAndEventResponse>)>]
    member __.AddPlayer(gameId : int, [<FromBody>] request : CreatePlayerRequest) : Task<IActionResult> =
        let ctx = base.HttpContext
        task {
            let! session = scp.GetSessionFromContext ctx
            let! response = manager.addPlayer gameId request session |> thenExtract
            return OkObjectResult(response) :> IActionResult
        }

    [<HttpDelete("{playerId}")>]
    [<ProducesResponseType(200, Type = typeof<StateAndEventResponse>)>]
    member __.RemovePlayer(gameId : int, playerId : int) : Task<IActionResult> =
        let ctx = base.HttpContext
        task {
            let! session = scp.GetSessionFromContext ctx
            let! response = manager.removePlayer (gameId, playerId) session |> thenExtract
            return OkObjectResult(response) :> IActionResult
        }
    
    [<HttpPut("{playerId}/status/{statusName}")>]
    [<ProducesResponseType(200, Type = typeof<StateAndEventResponse>)>]
    member __.UpdateGameParameters(gameId : int, playerId : int, statusName : string) : Task<IActionResult> =
        let ctx = base.HttpContext
        task {
            let! session = scp.GetSessionFromContext ctx
            let! response =
                Enum.parseUnion<PlayerStatus> statusName
                |> Apex.Api.Common.Control.Result.bindAsync (fun status ->
                    manager.updatePlayerStatus (gameId, playerId, status) session
                )
                |> thenExtract

            return OkObjectResult(response) :> IActionResult
        }

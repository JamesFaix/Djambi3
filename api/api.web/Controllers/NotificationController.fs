﻿namespace Djambi.Api.Web.Controllers

open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Primitives
open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Logic.Interfaces
open Djambi.Api.Model
open Djambi.Api.Web
open Djambi.Api.Web.Interfaces
open Djambi.Api.Web.Sse
open FSharp.Control.Tasks
open System.Threading.Tasks
open System

type NotificationController(u : HttpUtility,
                            notificationService : INotificationService) =    

    let contentType = "text/event-stream"
    let checkForCloseDelay = TimeSpan.FromSeconds(3.0)

    let checkAcceptHeader (ctx : HttpContext) =
         if not (ctx.Request.Headers.["Accept"] = StringValues(contentType))
         then Error <| HttpException(400, sprintf "Accept header must be '%s'." contentType)
         else Ok ()

    let subscribe (gameId : int option) =
        let func (ctx : HttpContext) =
            checkAcceptHeader ctx
            |> Result.bindAsync (fun _ -> u.getSessionFromContext ctx)
            |> thenMap (fun session ->
                ctx.Response.Headers.["Content-Type"] <- StringValues(contentType)
                ctx.Response.Body.Flush()

                let subscriberId : SubscriberId = 
                    { 
                        userId = session.user.id
                        gameId = gameId 
                    }
                let subscriber = new SseSubscriber(subscriberId, ctx.Response)                

                notificationService.add subscriber

                subscriberId
            )
            |> thenBindAsync (fun subscriberId ->
                task {
                    while not ctx.RequestAborted.IsCancellationRequested do                        
                        let! _ = Task.Delay checkForCloseDelay
                        ()
                    notificationService.remove subscriberId
                    return Ok ()
                }
            )
        u.handle func

    interface INotificationsController with

        member x.getNotificationsForCurrentUser =
            subscribe None

        member x.getNotificationsForCurrentUserForGame gameId =
            subscribe (Some gameId)
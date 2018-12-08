﻿module Djambi.Api.Web.Controllers.BoardController

open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Web.HttpUtility
open Djambi.Api.Web.Managers

let getBoard(regionCount : int) =
    let func ctx =
        getSessionFromContext ctx
        |> thenBindAsync (BoardManager.getBoard regionCount)
    handle func
            
let getCellPaths(regionCount : int, cellId : int) =
    let func ctx = 
        getSessionFromContext ctx
        |> thenBindAsync (BoardManager.getCellPaths (regionCount, cellId))
    handle func

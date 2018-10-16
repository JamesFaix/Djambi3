﻿module Djambi.Api.Web.Controllers.UserController

open System
open System.Threading.Tasks
open Giraffe
open Microsoft.AspNetCore.Http
open Djambi.Api.Common
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.Web.HttpUtility
open Djambi.Api.Web.Mappings.UserWebMapping
open Djambi.Api.Web.Model.UserWebModel

let createUser : HttpHandler =
    let func (ctx : HttpContext) =            
        ctx.BindModelAsync<CreateUserJsonModel>()
        |> Task.map mapCreateUserRequest
        |> Task.bind UserRepository.createUser
        |> thenMap mapUserResponse
    handle func

let deleteUser(userId : int) =
    let func ctx =
        UserRepository.deleteUser(userId)
    handle func

let getUser(userId : int) =
    let func ctx =
        UserRepository.getUser userId
        |> thenMap mapUserResponse
    handle func

let getUsers : HttpFunc -> HttpContext -> HttpContext option Task =
    let func ctx =
        UserRepository.getUsers()
        |> thenMap (Seq.map mapUserResponse)
    handle func

let updateUser(userId : int) =
    let func ctx = 
        raise (NotImplementedException "")
    handle func
        
﻿namespace Djambi.Api.Http

open Microsoft.AspNetCore.Http

open Giraffe

open Djambi.Api.Http.LobbyJsonModels
open Djambi.Api.Http.LobbyJsonMappings
open Djambi.Api.Persistence
open Djambi.Api.Common
open System.Threading.Tasks
open System
open Djambi.Api.Http.HttpUtility

module UserController =

    //let login : HttpHandler =
    //    let func (ctx : HttpContext) =
    //        task {
    //            let! request = ctx.BindModelAsync<LoginRequestJsonModel>()
    //                          |> Task.map mapLoginRequestFromJson

    //            let! user = UserRepository.getUserByName request.userName

    //            if user.password <> request.password
    //            then raise (HttpException(401, "Login failed"))
    //            else 

    //                //start session
    //                //return cookie
    //        }
    //    handle func

    let createUser : HttpHandler =
        let func (ctx : HttpContext) =            
            ctx.BindModelAsync<CreateUserJsonModel>()
            |> Task.map mapCreateUserRequest
            |> Task.bind UserRepository.createUser
            |> Task.map mapUserResponse
        handle func

    let deleteUser(userId : int) =
        let func ctx =
            UserRepository.deleteUser(userId)
        handle func

    let getUser(userId : int) =
        let func ctx =
            UserRepository.getUser userId
            |> Task.map mapUserResponse
        handle func

    let getUsers : HttpFunc -> HttpContext -> HttpContext option Task =
        let func ctx =
            UserRepository.getUsers()
            |> Task.map (Seq.map mapUserResponse)
        handle func

    let updateUser(userId : int) =
        let func ctx = 
            raise (NotImplementedException "")
        handle func
        
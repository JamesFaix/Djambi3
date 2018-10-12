﻿module Djambi.Api.Common.Task

open System.Threading.Tasks
open FSharp.Control.Tasks
    
let map<'a, 'b> (projection : 'a -> 'b) (t : 'a Task) : 'b Task =
    task {
        let! result = t
        return projection result
    }
    
let bind<'a, 'b> (projection : 'a -> 'b Task) (t : 'a Task) : 'b Task =
    task {
        let! result = t
        return! projection result
    }

let thenMap (projection : 'a -> 'b) (t : 'a HttpResult Task) : 'b HttpResult Task =
    t |> map (Result.map projection)

let thenBind (projection : 'a -> 'b HttpResult) (t : 'a HttpResult Task) : 'b HttpResult Task =
    t |> map (Result.bind projection)

let thenBindAsync (projection : 'a -> 'b HttpResult Task) (t : 'a HttpResult Task) : 'b HttpResult Task =
    let projectIfValue (result : 'a HttpResult) =
        match result with
        | Ok x -> projection x
        | Error x -> Task.FromResult(Error x)

    t |> bind projectIfValue
        
let thenDoAsync (action : 'a -> _ HttpResult Task) (t : 'a HttpResult Task) : 'a HttpResult Task =
    task {
        let! result = t
        match result with
        | Error _ -> return result   
        | Ok x -> 
            let! actionResult = action x        
            match actionResult with
            | Ok _ -> return Ok x
            | Error y -> return Error y                    
    }

let thenDoEachAsync (action : 'a -> Unit HttpResult Task) (t : 'a seq HttpResult Task) : Unit HttpResult Task =
    let doEachAsync (items : 'a seq) : Unit HttpResult Task =
        task {
            let mutable result = Ok ()
            let mutable stop = false

            use e = items.GetEnumerator()
            while not stop && e.MoveNext() do
                let! res = action e.Current
                match res with
                | Error _ -> result <- res
                                stop <- true
                | _ -> ()

            return result
        }

    t |> thenBindAsync doEachAsync
    
let thenReplaceError (statusCode : int) (newException : HttpException) (t : 'a HttpResult Task) : 'a HttpResult Task =
    let mapIfMatch (oldException : HttpException) =
        match oldException with
        | ex when ex.statusCode = statusCode -> newException
        | _ -> oldException

    t |> map (Result.mapError mapIfMatch)
       
let thenBindError (statusCode : int) (projection : HttpException -> 'a HttpResult) (t : 'a HttpResult Task) : 'a HttpResult Task =
    task {
        let! result = t
        match result with
        | Error ex when ex.statusCode = statusCode -> 
            return projection ex
        | _ -> return result
    }
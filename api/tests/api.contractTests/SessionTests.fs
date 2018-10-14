﻿module Djambi.Api.ContractTests.SessionTests

open System.Net
open System.Threading.Tasks
open FSharp.Control.Tasks
open NUnit.Framework
open Djambi.Api.Web.Model.LobbyWebModel
open Djambi.Api.WebClient

[<Test>]
let ``Create session should work``() =
    task {
        //Arrange
        let createUserRequest = RequestFactory.createUserRequest()
        let! user = UserRepository.createUser createUserRequest |> AsyncResponse.bodyValue
        let request = RequestFactory.loginRequest createUserRequest
    
        //Act
        let! response = SessionRepository.createSession request
        
        //Assert
        response |> shouldHaveStatus HttpStatusCode.OK

        response.getToken() |> shouldBeSome

        let session = response.bodyValue
        session.id |> shouldNotBe 0
        session.userIds.Length |> shouldBe 1
        session.userIds |> shouldExist (fun id -> id = user.id)
    } :> Task

[<Test>]
let ``Create session should fail if user has another session``() =
    task {
        //Arrange
        let createUserRequest = RequestFactory.createUserRequest()    
        let! _ = UserRepository.createUser createUserRequest
        let request = RequestFactory.loginRequest createUserRequest
    
        let! response1 = SessionRepository.createSession request
        response1 |> shouldHaveStatus HttpStatusCode.OK

        //Act
        let! response2 = SessionRepository.createSession request
        
        //Assert
        response2 |> shouldBeError HttpStatusCode.Conflict "Already signed in."
        response2.getToken() |> shouldBeNone
    } :> Task

[<Test>]
let ``Create session should fail if request has a token``() =
    task {
        //Arrange
        let createUserRequest = RequestFactory.createUserRequest()    
        let! _ = UserRepository.createUser createUserRequest
        let request = RequestFactory.loginRequest createUserRequest
    
        //Act
        let! response = SessionRepository.tryToCreateSessionWithToken(request, "someToken")

        //Assert
        response |> shouldBeError HttpStatusCode.Conflict "Already signed in."
        response.getToken() |> shouldBeNone
    } :> Task

[<Test>]
let ``Create session should fail if incorrect password``() =
    task {
        //Arrange
        let createUserRequest = RequestFactory.createUserRequest()    
        let! _ = UserRepository.createUser createUserRequest
        let request = 
            {
                userName = createUserRequest.name
                password = "wrong"
            }            
    
        //Act
        let! response = SessionRepository.createSession request

        //Assert
        response |> shouldBeError HttpStatusCode.Unauthorized "Incorrect password."
        response.getToken() |> shouldBeNone
    } :> Task

[<Test>]
let ``Create session should fail with locked message on 6th incorrect password attempt``() =
    task {
        //Arrange
        let createUserRequest = RequestFactory.createUserRequest()    
        let! _ = UserRepository.createUser createUserRequest
        let request = 
            {
                userName = createUserRequest.name
                password = "wrong"
            }            
        
        for _ in [1..5] do
            let! passwordResponse = SessionRepository.createSession request
            passwordResponse |> shouldBeError HttpStatusCode.Unauthorized "Incorrect password."

        //Act
        let! lockedResponse = SessionRepository.createSession request

        //Assert
        lockedResponse |> shouldBeError HttpStatusCode.Unauthorized "Account locked."
    } :> Task

[<Test>]
let ``Add user to session should work``() =
    task {
        //Arrange
        let! (user1, token) = SetupUtility.createUserAndSignIn()
        
        let createUserRequest = RequestFactory.createUserRequest()
        let! user2 = UserRepository.createUser createUserRequest |> AsyncResponse.bodyValue
        let loginRequest = RequestFactory.loginRequest createUserRequest
    
        //Act
        let! response = SessionRepository.addUserToSession(loginRequest, token)
        
        //Assert
        response |> shouldHaveStatus HttpStatusCode.OK
        response.getToken().Value |> shouldBe token

        let session = response.bodyValue
        session.id |> shouldNotBe 0
        session.userIds.Length |> shouldBe 2
        session.userIds |> shouldExist (fun id -> id = user1.id)
        session.userIds |> shouldExist (fun id -> id = user2.id)
    } :> Task

[<Test>]
let ``Add user to session should fail if no current session``() =
    task {
        //Arrange
        let createUserRequest = RequestFactory.createUserRequest()
        let! _ = UserRepository.createUser createUserRequest
        let loginRequest = RequestFactory.loginRequest createUserRequest
       
        //Act
        let! response = SessionRepository.addUserToSession(loginRequest, "")
        
        //Assert
        response |> shouldBeError HttpStatusCode.Unauthorized "Not signed in."
        response.getToken() |> shouldBeNone
    } :> Task

[<Test>]
let ``Add user to session should fail if session has 8 users already``() =
    task {
        //Arrange
        let! (_, token) = SetupUtility.createUserAndSignIn()

        for _ in [2..8] do
            let createUserRequestN = RequestFactory.createUserRequest()
            let! _ = UserRepository.createUser createUserRequestN
            let loginRequestN = RequestFactory.loginRequest createUserRequestN
            let! addResponseN = SessionRepository.addUserToSession(loginRequestN, token)
            addResponseN |> shouldHaveStatus HttpStatusCode.OK

        let createUserRequest9 = RequestFactory.createUserRequest()
        let! _ = UserRepository.createUser createUserRequest9
        let loginRequest9 = RequestFactory.loginRequest createUserRequest9
       
        //Act
        let! response = SessionRepository.addUserToSession(loginRequest9, token)
        
        //Assert
        response |> shouldBeError HttpStatusCode.BadRequest "Session already has maximum number of users."
        response.getToken() |> shouldBeNone
    } :> Task

[<Test>]
let ``Add user to session should fail if incorrect password``() =
    task {    
        //Arrange
        let! (_, token) = SetupUtility.createUserAndSignIn()

        let createUserRequest2 = RequestFactory.createUserRequest()    
        let! _ = UserRepository.createUser createUserRequest2
        let request = 
            {
                userName = createUserRequest2.name
                password = "wrong"
            }            
    
        //Act
        let! response = SessionRepository.addUserToSession(request, token)

        //Assert
        response |> shouldBeError HttpStatusCode.Unauthorized "Incorrect password."
        response.getToken() |> shouldBeNone
    } :> Task

[<Test>]
let ``Add user to session should fail with locked message on 6th incorrect password attempt``() =
    task {
        //Arrange
        let! (_, token) = SetupUtility.createUserAndSignIn()

        let createUserRequest2 = RequestFactory.createUserRequest()    
        let! _ = UserRepository.createUser createUserRequest2
        let request = 
            {
                userName = createUserRequest2.name
                password = "wrong"
            }            
        
        for _ in [1..5] do
            let! passwordResponse = SessionRepository.addUserToSession(request, token)
            passwordResponse |> shouldBeError HttpStatusCode.Unauthorized "Incorrect password."

        //Act
        let! lockedResponse = SessionRepository.addUserToSession(request, token)

        //Assert
        lockedResponse |> shouldBeError HttpStatusCode.Unauthorized "Account locked."
    } :> Task

[<Test>]
let ``Add user to session should fail if invalid token``() =
    task {
        //Arrange
        let createUserRequest = RequestFactory.createUserRequest()
        let! _ = UserRepository.createUser createUserRequest
        let loginRequest = RequestFactory.loginRequest createUserRequest

        //Act
        let! lockedResponse = SessionRepository.addUserToSession(loginRequest, "invalidToken")

        //Assert
        lockedResponse |> shouldBeError HttpStatusCode.NotFound "Session not found."
    } :> Task

[<Test>]
let ``Add user to session should fail if user already has session``() =
    task {
        //Arrange
        let! (_, token1) = SetupUtility.createUserAndSignIn()        
        let! (_, token2) = SetupUtility.createUserAndSignIn()

        let createUserRequest = RequestFactory.createUserRequest()
        let! _ = UserRepository.createUser createUserRequest
        let loginRequest = RequestFactory.loginRequest createUserRequest

        let! _ = SessionRepository.addUserToSession(loginRequest, token1)

        //Act
        let! lockedResponse = SessionRepository.addUserToSession(loginRequest, token2)

        //Assert
        lockedResponse |> shouldBeError HttpStatusCode.Conflict "Already signed in."
    } :> Task

[<Test>]
let ``Add user to session should fail if user is already in current session``() =
    task {
        //Arrange
        let createUserRequest = RequestFactory.createUserRequest()
        let! _ = UserRepository.createUser createUserRequest
        let loginRequest = RequestFactory.loginRequest createUserRequest
        let! sessionResponse = SessionRepository.createSession loginRequest
        let token = sessionResponse.getToken().Value

        //Act
        let! lockedResponse = SessionRepository.addUserToSession(loginRequest, token)

        //Assert
        lockedResponse |> shouldBeError HttpStatusCode.Conflict "Already signed in."
    } :> Task

[<Test>]
let ``Remove user from session should work for second user in session``() =
    task {
        //Arrange
        let createUserRequest1 = RequestFactory.createUserRequest()
        let! user1 = UserRepository.createUser createUserRequest1 |> AsyncResponse.bodyValue
        let loginRequest1 = RequestFactory.loginRequest createUserRequest1
        let! sessionResponse = SessionRepository.createSession loginRequest1
        let token = sessionResponse.getToken().Value

        let createUserRequest2 = RequestFactory.createUserRequest()
        let! user2 = UserRepository.createUser createUserRequest2 |> AsyncResponse.bodyValue
        let loginRequest2 = RequestFactory.loginRequest createUserRequest2
        let! _ = SessionRepository.addUserToSession(loginRequest2, token)
    
        //Act
        let! response = SessionRepository.removeUserFromSession(user2.id, token)
        
        //Assert
        response |> shouldHaveStatus HttpStatusCode.OK
        response.getToken().Value |> shouldBe(token)

        let session = response.bodyValue
        session.id |> shouldBe sessionResponse.bodyValue.id
        session.userIds.Length |> shouldBe 1
        session.userIds |> shouldExist (fun id -> id = user1.id)    
    } :> Task
    
[<Test>]
let ``Remove user from session should work for last user in session``() =
    task {
        //Arrange
        let createUserRequest = RequestFactory.createUserRequest()
        let! user = UserRepository.createUser createUserRequest |> AsyncResponse.bodyValue
        let loginRequest = RequestFactory.loginRequest createUserRequest
        let! sessionResponse = SessionRepository.createSession loginRequest
        let token = sessionResponse.getToken().Value
            
        //Act
        let! response = SessionRepository.removeUserFromSession(user.id, token)
        
        //Assert
        response |> shouldHaveStatus HttpStatusCode.OK
        response.getToken().Value |> shouldBe ""
        response.bodyValue |> shouldBe Unchecked.defaultof<SessionResponseJsonModel>
    } :> Task

[<Test>]
let ``Remove user from session should fail if no current session``() =
    task {
        //Arrange
        let createUserRequest = RequestFactory.createUserRequest()
        let! user = UserRepository.createUser createUserRequest |> AsyncResponse.bodyValue
            
        //Act
        let! response = SessionRepository.removeUserFromSession(user.id, "")
        
        //Assert
        response |> shouldBeError HttpStatusCode.Unauthorized "Not signed in."
    } :> Task

[<Test>]
let ``Remove user from session should fail if user is in a different session``() =
    task {
        //Arrange
        let! (_, token1) = SetupUtility.createUserAndSignIn()
        let! (user2, _) = SetupUtility.createUserAndSignIn()

        //Act
        let! response = SessionRepository.removeUserFromSession(user2.id, token1)
        
        //Assert
        response |> shouldBeError HttpStatusCode.Unauthorized "Invalid token."
    } :> Task

[<Test>]
let ``Remove user from session should fail if invalid token``() =
    task {
        //Arrange
        let! (user, _) = SetupUtility.createUserAndSignIn()
            
        //Act
        let! response = SessionRepository.removeUserFromSession(user.id, "invalidToken")
        
        //Assert
        response |> shouldBeError HttpStatusCode.Unauthorized "Invalid token."
    } :> Task

[<Test>]
let ``Close session should work``() =
    task {
        //Arrange
        let! (_, token) = SetupUtility.createUserAndSignIn()
        let! _ = SetupUtility.createUserAndAddToSession token
    
        //Act
        let! response = SessionRepository.closeSession token

        //Assert
        response |> shouldHaveStatus HttpStatusCode.OK
        response.getToken().Value |> shouldBe ""
    } :> Task

[<Test>]
let ``Close session should clear cookie if no session on backend``() =
    task {
        //Arrange

        //Act
        let! response = SessionRepository.closeSession "someToken"

        //Assert
        response |> shouldHaveStatus HttpStatusCode.OK
        response.getToken().Value |> shouldBe ""
    } :> Task
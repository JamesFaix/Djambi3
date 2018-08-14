namespace djambi.api

module HttpHandlers =

    open Microsoft.AspNetCore.Http
    open Giraffe
    open Djambi.Api.Dtos
    open Djambi.Model

    open BoardGeometry
    open BoardGeometryExtensions
    open BoardRepository

//Users
    let handleCreateUser =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let! request = ctx.BindModelAsync<CreateUserRequestDto>()

                let response : UserDto = {
                    id = 1
                    name = request.name
                }

                let placeHolderResponse = {
                    text = "Create user not yet implemented"
                }
                return! json placeHolderResponse next ctx
            }

    let handleDeleteUser(userId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let placeHolderResponse = {
                    text = sprintf "Delete user %i not yet implemented" userId
                }
                return! json placeHolderResponse next ctx
            }

    let handleGetUser(userId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let response : UserDto = {
                    id = userId
                    name = "Test"
                }

                let placeHolderResponse = {
                    text = sprintf "Get user %i not yet implemented" userId
                }
                return! json placeHolderResponse next ctx
            }

    let handleUpdateUser(userId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let! request = ctx.BindModelAsync<CreateUserRequestDto>()

                let response : UserDto = {
                    id = userId
                    name = request.name
                }

                let placeHolderResponse = {
                    text = sprintf "Update user %i not yet implemented" userId
                }
                return! json placeHolderResponse next ctx
            }

//Game lobby
    let handleGetOpenGames =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {

                let response : GameMetadataDto list = 
                    [
                        {
                            id = 1
                            status = GameStatus.Open
                            boardRegionCount = 3
                            players = 
                            [
                                {
                                    id = 1
                                    name = "TestUser"
                                }
                            ]
                        }
                    ]

                let placeHolderResponse = {
                    text = "Get open games not yet implemented"
                }
                return! json placeHolderResponse next ctx
            }

    let handleCreateGame =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let! request = ctx.BindModelAsync<CreateGameRequestDto>()

                let response : GameMetadataDto = {
                    id = 1
                    status = GameStatus.Open
                    boardRegionCount = request.boardRegionCount
                    players = List.empty
                }

                let placeHolderResponse = {
                    text = "Create game not yet implemented"
                }
                return! json placeHolderResponse next ctx
            }

    let handleDeleteGame(gameId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let placeHolderResponse = {
                    text = sprintf "Delete game %i not yet implemented" gameId
                }
                return! json placeHolderResponse next ctx
            }

    let handleAddPlayerToGame(gameId : int, userId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let placeHolderResponse = {
                    text = sprintf "Add player %i to game %i not yet implemented" userId gameId
                }
                return! json placeHolderResponse next ctx
            }

    let handleDeletePlayerFromGame(gameId : int, userId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let placeHolderResponse = {
                    text = sprintf "Delete player %i from game %i not yet implemented" userId gameId
                }
                return! json placeHolderResponse next ctx
            }

    let handleStartGame(gameId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let placeHolderResponse = {
                    text = sprintf "Start game %i not yet implemented" gameId
                }
                return! json placeHolderResponse next ctx
            }

//Board
    let handleGetBoard(regionCount : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let board = BoardRepository.getBoard(regionCount)
                return! json board next ctx
            }
            
    let handleGetCellPaths(regionCount : int, cellId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let board = BoardRepository.getBoardMetadata(regionCount)
                let cell = board.cells() |> Seq.find(fun c -> c.id = cellId)
                let paths = board.paths(cell)
                            |> List.map (fun path -> 
                                path |> List.map (fun c -> c.id))
                return! json paths next ctx
            }

//Gameplay
    let handleGetGameState(gameId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let response : GameDetailsDto = {
                    id = gameId
                    status = GameStatus.Open
                    boardRegionCount = 3
                    players = List.empty
                    pieces = List.empty
                    selectionOptions = List.empty
                }

                let placeHolderResponse = {
                    text = sprintf "Get game state %i not yet implemented" gameId
                }
                return! json placeHolderResponse next ctx
            }

    let handleMakeSelection(gameId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let! request = ctx.BindJsonAsync<CreateSelectionDto>()

                //let response : GameDetailsDto = {
                //    id = gameId
                //    status = GameStatus.Open
                //    boardRegionCount = 3
                //    players = List.empty
                //    pieces = List.empty
                //    selectionOptions = landscape.paths(location) |> List.collect id
                //}
                
                let placeHolderResponse = {
                    text = sprintf "Make selection %i not yet implemented" gameId
                }
                return! json placeHolderResponse next ctx
            }

    let handleResetTurn(gameId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let response : GameDetailsDto = {
                    id = gameId
                    status = GameStatus.Open
                    boardRegionCount = 3
                    players = List.empty
                    pieces = List.empty
                    selectionOptions = List.empty
                }

                let placeHolderResponse = {
                    text = sprintf "Reset turn %i not yet implemented" gameId
                } 
                return! json placeHolderResponse next ctx
            }

    let handleCommitTurn(gameId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let response : GameDetailsDto = {
                    id = gameId
                    status = GameStatus.Open
                    boardRegionCount = 3
                    players = List.empty
                    pieces = List.empty
                    selectionOptions = List.empty
                }

                let placeHolderResponse = {
                    text = sprintf "Commit turn %i not yet implemented" gameId
                }
                return! json placeHolderResponse next ctx
            }

    let handleSendMessage(gameId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let! request = ctx.BindModelAsync<CreateMessageDto>()

                let response = {
                    text = sprintf "Send message %i not yet implemented" gameId
                }
                return! json response next ctx
            }
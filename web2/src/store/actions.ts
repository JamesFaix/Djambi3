import { LoginRequest, User, CreateUserRequest, Game, GamesQuery, GameParameters } from "../api/model";

export enum ActionStatus {
  Pending = "PENDING",
  Success = "SUCCESS",
  Error = "ERROR"
}

export enum ActionTypes {
  Login = "LOGIN",
  Logout = "LOGOUT",
  Signup = "SIGNUP",
  LoadGame = "LOAD_GAME",
  QueryGames = "QUERY_GAMES",
  UpdateGamesQuery = "UPDATE_GAMES_QUERY",
  RestoreSession = "RESTORE_SESSION",
  CreateGame = "CREATE_GAME",
  UpdateCreateGameForm = "UPDATE_CREATE_GAME_FORM"
}

export interface CustomAction {
  type: ActionTypes,
  status: ActionStatus,
}

export interface DataAction<T> extends CustomAction {
  data : T
}

function create<T>(type : ActionTypes, status : ActionStatus, data : T = undefined) {
  return {
    type: type,
    status: status,
    data: data
  };
}
const pending = <T>(type : ActionTypes, data : T = undefined) => create(type, ActionStatus.Pending, data);
const success = <T>(type : ActionTypes, data : T = undefined) => create(type, ActionStatus.Success, data);
const error = <T>(type : ActionTypes, data : T = undefined) => create(type, ActionStatus.Error, data);

export const loginRequest = (request : LoginRequest) => pending(ActionTypes.Login, request);
export const loginSuccess = (user : User) => success(ActionTypes.Login, user);
export const loginError = () => error(ActionTypes.Login);

export const logoutRequest = () => pending(ActionTypes.Logout);
export const logoutSuccess = () => success(ActionTypes.Logout);
export const logoutError = () => error(ActionTypes.Logout);

export const signupRequest = (request : CreateUserRequest) => pending(ActionTypes.Signup, request);
export const signupSuccess = (user : User) => success(ActionTypes.Signup, user);
export const signupError = () => error(ActionTypes.Signup);

export const loadGameRequest = (gameId : number) => pending(ActionTypes.LoadGame, gameId);
export const loadGameSuccess = (game : Game) => success(ActionTypes.LoadGame, game);
export const loadGameError = () => error(ActionTypes.LoadGame);

export const queryGamesRequest = (query : GamesQuery) => pending(ActionTypes.QueryGames, query);
export const queryGamesSuccess = (games : Game[]) => success(ActionTypes.QueryGames, games);
export const queryGamesError = () => error(ActionTypes.QueryGames);

export const updateGamesQuery = (query: GamesQuery) => success(ActionTypes.UpdateGamesQuery, query);

export const restoreSessionRequest = () => pending(ActionTypes.RestoreSession);
export const restoreSessionSuccess = (user: User) => success(ActionTypes.RestoreSession, user);
export const restoreSessionError = () => error(ActionTypes.RestoreSession);

export const updateCreateGameForm = (parameters : GameParameters) => success(ActionTypes.UpdateCreateGameForm, parameters);

export const createGameRequest = (parameters: GameParameters) => pending(ActionTypes.CreateGame, parameters);
export const createGameSuccess = (game : Game) => success(ActionTypes.CreateGame, game);
export const createGameError = () => error(ActionTypes.CreateGame);
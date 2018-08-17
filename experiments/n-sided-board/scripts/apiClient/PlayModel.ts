export class Player {
    readonly id : number
    readonly userId : number
    readonly name : string
    readonly isAlive : boolean
}

export enum PieceType {
    Chief = "Chief",
    Assassin = "Assassin",
    Diplomat = "Diplomat",
    Reporter = "Reporter",
    Thug = "Thug",
    Gravedigger = "Gravedigger"
}

export class Piece {
    readonly id : number
    readonly type : PieceType
    readonly playerId : number
    readonly originalPlayerId : number
    readonly isAlive : boolean
    readonly cellId : number
}

export class GameState {
    readonly players : Array<Player>
    readonly pieces : Array<Piece>
    readonly turnCycle : Array<number>
}
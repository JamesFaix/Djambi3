/* tslint:disable */
/* eslint-disable */
/**
 * Apex API
 * API for Apex
 *
 * The version of the OpenAPI document: v1
 * 
 *
 * NOTE: This class is auto generated by OpenAPI Generator (https://openapi-generator.tech).
 * https://openapi-generator.tech
 * Do not edit the class manually.
 */

/**
 * 
 * @export
 * @enum {string}
 */
export enum EventKind {
    GameParametersChanged = 'GameParametersChanged',
    GameCanceled = 'GameCanceled',
    PlayerJoined = 'PlayerJoined',
    PlayerRemoved = 'PlayerRemoved',
    GameStarted = 'GameStarted',
    TurnCommitted = 'TurnCommitted',
    TurnReset = 'TurnReset',
    CellSelected = 'CellSelected',
    PlayerStatusChanged = 'PlayerStatusChanged'
}

export function EventKindFromJSON(json: any): EventKind {
    return EventKindFromJSONTyped(json, false);
}

export function EventKindFromJSONTyped(json: any, ignoreDiscriminator: boolean): EventKind {
    return json as EventKind;
}

export function EventKindToJSON(value?: EventKind | null): any {
    return value as any;
}


/* tslint:disable */
/* eslint-disable */
/**
 * Djambi-N API
 * API for Djambi-N
 *
 * The version of the OpenAPI document: v1
 * 
 *
 * NOTE: This class is auto generated by OpenAPI Generator (https://openapi-generator.tech).
 * https://openapi-generator.tech
 * Do not edit the class manually.
 */

import { exists, mapValues } from '../runtime';
import {
    PieceKind,
    PieceKindFromJSON,
    PieceKindFromJSONTyped,
    PieceKindToJSON,
} from './';

/**
 * 
 * @export
 * @interface PieceDto
 */
export interface PieceDto {
    /**
     * 
     * @type {number}
     * @memberof PieceDto
     */
    id: number;
    /**
     * 
     * @type {PieceKind}
     * @memberof PieceDto
     */
    kind: PieceKind;
    /**
     * 
     * @type {number}
     * @memberof PieceDto
     */
    playerId?: number | null;
    /**
     * 
     * @type {number}
     * @memberof PieceDto
     */
    originalPlayerId: number;
    /**
     * 
     * @type {number}
     * @memberof PieceDto
     */
    cellId: number;
}

export function PieceDtoFromJSON(json: any): PieceDto {
    return PieceDtoFromJSONTyped(json, false);
}

export function PieceDtoFromJSONTyped(json: any, ignoreDiscriminator: boolean): PieceDto {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'id': json['id'],
        'kind': PieceKindFromJSON(json['kind']),
        'playerId': !exists(json, 'playerId') ? undefined : json['playerId'],
        'originalPlayerId': json['originalPlayerId'],
        'cellId': json['cellId'],
    };
}

export function PieceDtoToJSON(value?: PieceDto | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
        'id': value.id,
        'kind': PieceKindToJSON(value.kind),
        'playerId': value.playerId,
        'originalPlayerId': value.originalPlayerId,
        'cellId': value.cellId,
    };
}



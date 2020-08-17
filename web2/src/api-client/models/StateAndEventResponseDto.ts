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
    EventDto,
    EventDtoFromJSON,
    EventDtoFromJSONTyped,
    EventDtoToJSON,
    GameDto,
    GameDtoFromJSON,
    GameDtoFromJSONTyped,
    GameDtoToJSON,
} from './';

/**
 * 
 * @export
 * @interface StateAndEventResponseDto
 */
export interface StateAndEventResponseDto {
    /**
     * 
     * @type {GameDto}
     * @memberof StateAndEventResponseDto
     */
    game: GameDto;
    /**
     * 
     * @type {EventDto}
     * @memberof StateAndEventResponseDto
     */
    event: EventDto;
}

export function StateAndEventResponseDtoFromJSON(json: any): StateAndEventResponseDto {
    return StateAndEventResponseDtoFromJSONTyped(json, false);
}

export function StateAndEventResponseDtoFromJSONTyped(json: any, ignoreDiscriminator: boolean): StateAndEventResponseDto {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'game': GameDtoFromJSON(json['game']),
        'event': EventDtoFromJSON(json['event']),
    };
}

export function StateAndEventResponseDtoToJSON(value?: StateAndEventResponseDto | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
        'game': GameDtoToJSON(value.game),
        'event': EventDtoToJSON(value.event),
    };
}



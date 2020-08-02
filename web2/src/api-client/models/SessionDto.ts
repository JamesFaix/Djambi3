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

import { exists, mapValues } from '../runtime';
import {
    UserDto,
    UserDtoFromJSON,
    UserDtoFromJSONTyped,
    UserDtoToJSON,
} from './';

/**
 * 
 * @export
 * @interface SessionDto
 */
export interface SessionDto {
    /**
     * 
     * @type {number}
     * @memberof SessionDto
     */
    readonly id?: number;
    /**
     * 
     * @type {UserDto}
     * @memberof SessionDto
     */
    user: UserDto;
    /**
     * 
     * @type {string}
     * @memberof SessionDto
     */
    readonly token: string;
    /**
     * 
     * @type {Date}
     * @memberof SessionDto
     */
    readonly createdOn?: Date;
    /**
     * 
     * @type {Date}
     * @memberof SessionDto
     */
    readonly expiresOn?: Date;
}

export function SessionDtoFromJSON(json: any): SessionDto {
    return SessionDtoFromJSONTyped(json, false);
}

export function SessionDtoFromJSONTyped(json: any, ignoreDiscriminator: boolean): SessionDto {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'id': !exists(json, 'id') ? undefined : json['id'],
        'user': UserDtoFromJSON(json['user']),
        'token': json['token'],
        'createdOn': !exists(json, 'createdOn') ? undefined : (new Date(json['createdOn'])),
        'expiresOn': !exists(json, 'expiresOn') ? undefined : (new Date(json['expiresOn'])),
    };
}

export function SessionDtoToJSON(value?: SessionDto | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
        'user': UserDtoToJSON(value.user),
    };
}



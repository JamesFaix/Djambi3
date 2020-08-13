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
/**
 * 
 * @export
 * @interface CreateUserRequestDto
 */
export interface CreateUserRequestDto {
    /**
     * 
     * @type {string}
     * @memberof CreateUserRequestDto
     */
    name: string;
    /**
     * 
     * @type {string}
     * @memberof CreateUserRequestDto
     */
    password: string;
}

export function CreateUserRequestDtoFromJSON(json: any): CreateUserRequestDto {
    return CreateUserRequestDtoFromJSONTyped(json, false);
}

export function CreateUserRequestDtoFromJSONTyped(json: any, ignoreDiscriminator: boolean): CreateUserRequestDto {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'name': json['name'],
        'password': json['password'],
    };
}

export function CreateUserRequestDtoToJSON(value?: CreateUserRequestDto | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
        'name': value.name,
        'password': value.password,
    };
}



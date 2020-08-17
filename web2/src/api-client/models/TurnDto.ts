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
    SelectionDto,
    SelectionDtoFromJSON,
    SelectionDtoFromJSONTyped,
    SelectionDtoToJSON,
    SelectionKind,
    SelectionKindFromJSON,
    SelectionKindFromJSONTyped,
    SelectionKindToJSON,
    TurnStatus,
    TurnStatusFromJSON,
    TurnStatusFromJSONTyped,
    TurnStatusToJSON,
} from './';

/**
 * 
 * @export
 * @interface TurnDto
 */
export interface TurnDto {
    /**
     * 
     * @type {TurnStatus}
     * @memberof TurnDto
     */
    status?: TurnStatus;
    /**
     * 
     * @type {Array<SelectionDto>}
     * @memberof TurnDto
     */
    selections: Array<SelectionDto>;
    /**
     * 
     * @type {Array<number>}
     * @memberof TurnDto
     */
    selectionOptions: Array<number>;
    /**
     * 
     * @type {SelectionKind}
     * @memberof TurnDto
     */
    requiredSelectionKind?: SelectionKind;
}

export function TurnDtoFromJSON(json: any): TurnDto {
    return TurnDtoFromJSONTyped(json, false);
}

export function TurnDtoFromJSONTyped(json: any, ignoreDiscriminator: boolean): TurnDto {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'status': !exists(json, 'status') ? undefined : TurnStatusFromJSON(json['status']),
        'selections': ((json['selections'] as Array<any>).map(SelectionDtoFromJSON)),
        'selectionOptions': json['selectionOptions'],
        'requiredSelectionKind': !exists(json, 'requiredSelectionKind') ? undefined : SelectionKindFromJSON(json['requiredSelectionKind']),
    };
}

export function TurnDtoToJSON(value?: TurnDto | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
        'status': TurnStatusToJSON(value.status),
        'selections': ((value.selections as Array<any>).map(SelectionDtoToJSON)),
        'selectionOptions': value.selectionOptions,
        'requiredSelectionKind': SelectionKindToJSON(value.requiredSelectionKind),
    };
}



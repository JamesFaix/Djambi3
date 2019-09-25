import * as React from 'react';
import { GameStatus } from '../../api/model';
import { State as AppState } from '../../store/root';
import { useSelector } from 'react-redux';
import TristateDropdown from '../controls/tristateDropdown';
import { EnumMultiDropdown } from '../controls/enumDropdown';
import { IconSubmitButton } from '../controls/iconButton';
import { Icons } from '../../utilities/icons';
import HtmlInputTypes from '../htmlInputTypes';
import Controller from '../../controllers/controller';
import DateService from '../../utilities/dates';

const GamesSearchForm : React.SFC<{}> = _ => {
    const query = useSelector((state : AppState) => state.search.query);
    const onUpdate = Controller.Forms.updateGamesQuery;

    return (
        <form
            onSubmit={() => Controller.Search.searchGames(query)}
        >
            <table>
                <tbody>
                    <tr>
                        <td>Description</td>
                        <td>
                            <input
                                type={HtmlInputTypes.Text}
                                value={emptyIfNull(query.descriptionContains)}
                                onChange={e => onUpdate({ ...query, descriptionContains: nullIfEmpty(e.target.value) })}
                                autoFocus
                            />
                        </td>
                        <td>Is public</td>
                        <td>
                            <TristateDropdown
                                name={"IsPublic"}
                                value={query.isPublic}
                                onChange={(_, value) => onUpdate({ ...query, isPublic: value })}
                            />
                        </td>
                    </tr>
                    <tr>
                        <td>Created by user</td>
                        <td>
                            <input
                                type={HtmlInputTypes.Text}
                                value={emptyIfNull(query.createdByUserName)}
                                onChange={e => onUpdate({ ...query, createdByUserName: nullIfEmpty(e.target.value) })}
                            />
                        </td>
                        <td>Allow guests</td>
                        <td>
                            <TristateDropdown
                                name={"AllowGuests"}
                                value={query.allowGuests}
                                onChange={(_, value) => onUpdate({ ...query, allowGuests: value })}
                            />
                        </td>
                    </tr>
                    <tr>
                        <td>Contains user</td>
                        <td>
                            <input
                                type={HtmlInputTypes.Text}
                                value={emptyIfNull(query.playerUserName)}
                                onChange={e => onUpdate({ ...query, playerUserName: nullIfEmpty(e.target.value) })}
                            />
                        </td>
                        <td>Status</td>
                        <td>
                            <EnumMultiDropdown
                                name={"Status"}
                                values={query.statuses}
                                onChange={(_, values) => onUpdate({ ...query, statuses: values })}
                                enum={GameStatus}
                            />
                        </td>
                    </tr>
                    <tr>
                        <td>Created between</td>
                        <td>
                            <DatePicker
                                value={query.createdAfter}
                                onUpdate={d => onUpdate({ ...query, createdAfter: d })}
                                isStartDate={true}
                            />
                        </td>
                        <td>and</td>
                        <td>
                            <DatePicker
                                value={query.createdBefore}
                                onUpdate={d => onUpdate({ ...query, createdBefore: d })}
                                isStartDate={false}
                            />
                        </td>
                    </tr>
                    <tr>
                        <td>Last event between</td>
                        <td>
                            <DatePicker
                                value={query.lastEventAfter}
                                onUpdate={d => onUpdate({ ...query, lastEventAfter: d })}
                                isStartDate={true}
                            />
                        </td>
                        <td>and</td>
                        <td>
                            <DatePicker
                                value={query.lastEventBefore}
                                onUpdate={d => onUpdate({ ...query, lastEventBefore: d })}
                                isStartDate={false}
                            />
                    </td>
                    </tr>
                    <tr>
                        <td>GameId</td>
                        <td>
                            <input
                                style={{width:"50px"}}
                                type={HtmlInputTypes.Number}
                                min={1}
                                value={emptyIfNull(query.gameId)}
                                onChange={e => onUpdate({ ...query, gameId: parseInt(e.target.value) })}
                            />
                        </td>
                    </tr>
                </tbody>
            </table>
            <br/>
            <IconSubmitButton
                icon={Icons.UserActions.search}
                showTitle={true}
            />
        </form>
    );
};
export default GamesSearchForm;

function emptyIfNull(value : any) : string {
    return value === null ? "" : value;
}

function nullIfEmpty(value : string) : string {
    return value === "" ? null : value;
}

const DatePicker : React.SFC<{
    value: Date,
    onUpdate: (d:Date) => void,
    isStartDate : boolean
}> = props => {
    return (
        <input
            type={HtmlInputTypes.Date}
            value={DateService.dateToDatepickerString(props.value, props.isStartDate)}
            onChange={e => props.onUpdate(DateService.dateFromDatepickerString(e.target.value))}
            min={DateService.minDate()}
            max={DateService.maxDate()}
        />
    );
}
import * as React from 'react';
import '../../index.css';
import PageTitle from '../pageTitle';
import { UserResponse, LobbyResponse, LobbiesQueryRequest } from '../../api/model';
import ApiClient from '../../api/client';
import { Redirect } from 'react-router';
import LinkButton from '../linkButton';
import * as moment from 'moment';
import LabeledInput from '../labeledInput';
import { InputTypes } from '../../constants';
import LabeledTristateDropdown from '../labeledTristateDropdown';
import ActionButton from '../actionButton';
import Util from '../../util';

export interface FindLobbyPageProps {
    user : UserResponse,
    api : ApiClient,
}

export interface FindLobbyPageState {
    lobbies : LobbyResponse[],
    createdByUserIdFilter : number,
    playerUserIdFilter : number,
    isPublicFilter : boolean,
    allowGuestsFilter : boolean,
    descriptionContainsFilter : string
}

export default class FindLobbyPage extends React.Component<FindLobbyPageProps, FindLobbyPageState> {
    constructor(props : FindLobbyPageProps) {
        super(props);
        this.state = {
            lobbies : [],
            createdByUserIdFilter: null,
            playerUserIdFilter: null,
            isPublicFilter: null,
            allowGuestsFilter: null,
            descriptionContainsFilter: null
        };
    }

    componentDidMount() {
        this.refreshResults();
    }

    private refreshResults() {
        const query : LobbiesQueryRequest = {
            createdByUserId: this.state.createdByUserIdFilter,
            playerUserId: this.state.playerUserIdFilter,
            isPublic: this.state.isPublicFilter,
            allowGuests: this.state.allowGuestsFilter,
            descriptionContains: this.state.descriptionContainsFilter
        }

        this.props.api
            .getLobbies(query)
            .then(lobbies => {
                this.setState({lobbies : lobbies});
            })
            .catch(reason => {
                alert("Get lobby failed because " + reason);
            });
    }

//---Event handlers---

    private resetOnClick() {
        this.setState({
                createdByUserIdFilter: null,
                playerUserIdFilter: null,
                isPublicFilter: null,
                allowGuestsFilter: null,
                descriptionContainsFilter: null
            },
            () => this.refreshResults());
    }

//---Rendering---

    renderLobbyRow(lobby : LobbyResponse, rowNumber : number) {
        return (
            <tr key={"row" + rowNumber}>
                <td>
                    <LinkButton
                        label="Go"
                        to={"/lobby/" + lobby.id}
                    />
                </td>
                <td>
                    {moment(lobby.createdOn).format('MM/DD/YY hh:mm a')}
                </td>
                <td>{lobby.createdByUserId}</td>
                <td className="centeredContainer">
                    {lobby.regionCount}
                </td>
                <td className="centeredContainer">
                    <input
                        type="checkbox"
                        checked={lobby.isPublic}
                        disabled={true}
                    />
                </td>
                <td className="centeredContainer">
                    <input
                        type="checkbox"
                        checked={lobby.allowGuests}
                        disabled={true}
                    />
                </td>
                <td>{lobby.description}</td>
            </tr>
        );
    }

    renderQueryFilters() {
        return (
            <div>
                <table className="table">
                    <tbody>
                        <tr>
                            <td className="borderless">
                                <LabeledInput
                                    label="Created by"
                                    type={InputTypes.Text}
                                    value={Util.toStringSafe(this.state.createdByUserIdFilter)}
                                    onChange={e => this.setState({ createdByUserIdFilter: Number(e.target.value) })}
                                />
                            </td>
                            <td className="borderless">
                                <LabeledTristateDropdown
                                    label="Public"
                                    value={this.state.isPublicFilter}
                                    onChange={(_, value) => this.setState({ isPublicFilter: value })}
                                />
                            </td>
                        </tr>
                        <tr>
                        <td className="borderless">
                                <LabeledInput
                                    label="Has user"
                                    type={InputTypes.Text}
                                    value={Util.toStringSafe(this.state.playerUserIdFilter)}
                                    onChange={e => this.setState({ playerUserIdFilter: Number(e.target.value) })}
                                />
                            </td>
                            <td className="borderless">
                                <LabeledTristateDropdown
                                    label="Guests allowed"
                                    value={this.state.allowGuestsFilter}
                                    onChange={(_, value) => this.setState({ allowGuestsFilter: value })}
                                />
                            </td>
                        </tr>
                        <tr>
                            <td className="borderless">
                                <LabeledInput
                                    label="Description"
                                    type={InputTypes.Text}
                                    value={Util.toStringSafe(this.state.descriptionContainsFilter)}
                                    onChange={e => this.setState({ descriptionContainsFilter: e.target.value })}
                                />
                            </td>
                            <td className="borderless">
                            </td>
                        </tr>
                        <tr>
                            <td className="borderless rightAligned">
                                <ActionButton
                                    label="Search"
                                    onClick={() => this.refreshResults()}
                                />
                            </td>
                            <td className="borderless">
                                <ActionButton
                                    label="Reset"
                                    onClick={() => this.resetOnClick()}
                                />
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        );
    }

    renderLobbiesTable() {
        return (
            <div>
                <table className="table">
                    <tbody>
                        <tr>
                            <th></th>
                            <th>Created on</th>
                            <th>Created by</th>
                            <th>Regions</th>
                            <th>Public</th>
                            <th>Guests allowed</th>
                            <th>Description</th>
                        </tr>
                        {this.state.lobbies.map((lobby, i) => this.renderLobbyRow(lobby, i))}
                    </tbody>
                </table>
            </div>
        );
    }

    render() {
        //Go to home if not logged in
        if (this.props.user === null) {
            return <Redirect to='/'/>
        }

        return (
            <div>
                <PageTitle label={"Find Game"}/>
                <br/>
                <div className="centeredContainer">
                    <LinkButton label="Home" to="/dashboard"/>
                    <LinkButton label="My Games" to="/myGames"/>
                    <LinkButton label="Create Game" to="/createGame"/>
                </div>
                <br/>
                {this.renderQueryFilters()}
                <br/>
                {this.renderLobbiesTable()}
            </div>
        );
    }
}
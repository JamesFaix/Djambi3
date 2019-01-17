import * as React from 'react';
import '../../index.css';
import PageTitle from '../pageTitle';
import { User, Game, GamesQuery } from '../../api/model';
import ApiClient from '../../api/client';
import { Redirect } from 'react-router';
import LinkButton from '../linkButton';
import LabeledInput from '../labeledInput';
import { InputTypes } from '../../constants';
import LabeledTristateDropdown from '../labeledTristateDropdown';
import ActionButton from '../actionButton';
import Util from '../../util';
import LobbiesTable from '../lobbiesTable';

export interface FindLobbyPageProps {
    user : User,
    api : ApiClient,
}

export interface FindLobbyPageState {
    games : Game[],
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
            games : [],
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
        const query : GamesQuery = {
            gameId : null,
            createdByUserId: this.state.createdByUserIdFilter,
            playerUserId: this.state.playerUserIdFilter,
            isPublic: this.state.isPublicFilter,
            allowGuests: this.state.allowGuestsFilter,
            descriptionContains: this.state.descriptionContainsFilter
        }

        this.props.api
            .getGames(query)
            .then(games => {
                this.setState({games : games});
            })
            .catch(reason => {
                alert("Get games failed because " + reason);
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
                <LobbiesTable
                    games={this.state.games}
                />
            </div>
        );
    }
}
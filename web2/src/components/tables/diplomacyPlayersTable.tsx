import * as React from 'react';
import { State } from '../../store/root';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import { User, Game, Player, PlayerStatus } from '../../api/model';
import Styles from '../../styles/styles';
import IconButton from '../controls/iconButton';
import PlayerStatusIcon from '../controls/playerStatusIcon';
import PlayerNoteIcon from '../controls/playerNoteIcon';
import { SectionHeader } from '../controls/headers';
import Icons from '../../utilities/icons';
import ApiActions from '../../apiActions';

interface DiplomacyPageProps {
    user : User,
    game : Game
    changePlayerStatus : (gameId : number, playerId : number, status : PlayerStatus) => void
}

class diplomacyPlayersTable extends React.Component<DiplomacyPageProps>{
    render() {
        if (!this.props.game) {
            return null;
        }

        const user = this.props.user;
        const players = this.props.game.players.filter(p => p.userId === user.id);

        return (
            <div>
                <SectionHeader text="Diplomatic actions"/>
                <table>
                    <tbody>
                        <tr>
                            <th style={Styles.noBorder()}>
                                Player
                            </th>
                            <th style={Styles.noBorder()}>
                                Status
                            </th>
                            <th style={Styles.noBorder()}>
                            </th>
                            <th style={Styles.noBorder()}>
                                Actions
                            </th>
                        </tr>
                        {players.map((p, i) =>
                            <PlayerRow
                                game={this.props.game}
                                player={p}
                                changePlayerStatus={this.props.changePlayerStatus}
                                key={i}
                            />
                        )}
                    </tbody>
                </table>
            </div>
        )
    }
}

interface PlayerRowProps {
    game : Game,
    player : Player,
    changePlayerStatus : (gameId : number, playerId : number, status : PlayerStatus) => void
}

const PlayerRow : React.SFC<PlayerRowProps> = props => {
    const p = props.player;
    return (
        <tr style={Styles.playerBoxGlow(props.player.colorId)}>
            <td style={Styles.noBorder()}>{p.name}</td>
            <td style={Styles.noBorder()}>
                <PlayerStatusIcon
                    player={p}
                />
            </td>
            <td style={Styles.noBorder()}>
                <PlayerNoteIcon
                    player={p}
                    game={props.game}
                />
            </td>
            <td style={Styles.noBorder()}>
                <PlayerDiplomacyActionButtons
                    gameId={props.game.id}
                    player={p}
                    changePlayerStatus={props.changePlayerStatus}
                />
            </td>
        </tr>
    );
};

interface PlayerDiplomacyActionButtonsProps {
    gameId : number,
    player : Player,
    changePlayerStatus : (gameId : number, playerId : number, status : PlayerStatus) => void
}

const PlayerDiplomacyActionButtons : React.SFC<PlayerDiplomacyActionButtonsProps> = props => {
    const p = props.player;
    const s = p.status;

    const canConcede = s === PlayerStatus.Alive || s === PlayerStatus.AcceptsDraw;
    const canAcceptDraw = s === PlayerStatus.Alive;
    const canRevokeDraw = s === PlayerStatus.AcceptsDraw;

    return (
        <div>
            {canAcceptDraw ?
                <IconButton
                    title="Accept draw"
                    icon={Icons.playerStatusAcceptsDraw}
                    onClick={() => props.changePlayerStatus(props.gameId, p.id, PlayerStatus.AcceptsDraw)}
                />
            : null}
            {canRevokeDraw ?
                <IconButton
                    title="Revoke draw"
                    icon={Icons.revokeDraw}
                    onClick={() => props.changePlayerStatus(props.gameId, p.id, PlayerStatus.Alive)}
                />
            : null}
            {canConcede ?
                <IconButton
                    title="Concede"
                    icon={Icons.playerStatusConceded}
                    onClick={() => props.changePlayerStatus(props.gameId, p.id, PlayerStatus.Conceded)}
                />
            : null}
        </div>
    );
};

const mapStateToProps = (state : State) => {
    return {
        user: state.session.user,
        game: state.activeGame.game
    };
}

const mapDispatchToProps = (dispatch : Dispatch) => {
    return {
        changePlayerStatus: (gameId : number, playerId : number, status : PlayerStatus) =>
            ApiActions.changePlayerStatus(gameId, playerId, status)(dispatch)
    };
}

const DiplomacyPlayersTable = connect(mapStateToProps, mapDispatchToProps)(diplomacyPlayersTable);
export default DiplomacyPlayersTable;
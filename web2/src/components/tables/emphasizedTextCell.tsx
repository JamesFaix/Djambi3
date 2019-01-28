import * as React from 'react';
import '../../index.css';

export interface EmphasizedTextCellProps {
    text : string
}

export default class EmphasizedTextCell extends React.Component<EmphasizedTextCellProps> {

    render() {
        return  (
            <td className="lobbyPlayersTableTextCell">
                {this.props.text}
            </td>
        );
    }
}
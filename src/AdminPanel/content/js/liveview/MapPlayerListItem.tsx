import React from "react";
import { connect } from "react-redux";

import { PlayerData } from "../stores/map/types";
import { disconnectPlayer, banPlayer } from "../stores/map/actions";
import {World} from "./World";

interface MapPlayerListItemProps {
    player: PlayerData;
    highlightOn: (objectId: number) => void;
    highlightOff: (objectId: number) => void;
    disconnectPlayer: (player: PlayerData) => void;
    banPlayer: (player: PlayerData) => void;
}

class MapPlayerListItem extends React.Component<MapPlayerListItemProps, any> {

    private handleClickOnHighlightButton() {
        if (this.props.player.isHighlighted) {
            this.props.highlightOff(this.props.player.id);
        } else {
            this.props.highlightOn(this.props.player.id);
        }
    }

    public render() {
        return (
            <tr>
                <td>{ this.props.player.name } </td>
                <td> { this.props.player.id } </td>
                <td>
                    {this.renderHighlightButton()}
                    <button type="button" className="btn btn-default btn-xs glyphicon glyphicon-log-out" onClick={() => this.props.disconnectPlayer(this.props.player) } title="Disconnect"></button>
                    <button type="button" className="btn btn-default btn-xs glyphicon glyphicon-ban-circle" onClick={() => this.props.banPlayer(this.props.player) } title="Temporarily ban"></button>
                </td>
            </tr>
        );
    }

    private renderHighlightButton() {
        let className = "btn btn-default btn-xs glyphicon glyphicon-map-marker";
        if (this.props.player.isHighlighted) {
            className += " active";
        }

        return (<button type="button" className={className} onClick={() => this.handleClickOnHighlightButton()} title = "Highlight on map"> </button>);
    }
}

const mapDispatchToProps = (dispatch: any) => {
    return {
        disconnectPlayer: (player: PlayerData) => dispatch(disconnectPlayer(player)),
        banPlayer: (player: PlayerData) => dispatch(banPlayer(player))
    };
}

export default connect(null, mapDispatchToProps)(MapPlayerListItem);
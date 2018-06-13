import React from "react";
import {connect} from "react-redux";

import { ApplicationState } from "../stores/index";

import { PlayerData } from "../stores/map/types";
import MapPlayerListItem from "./MapPlayerListItem";
import { World } from "./World";


interface MapPlayerListProps {
    players: PlayerData[],
    world: World,
}

class MapPlayerList extends React.Component<MapPlayerListProps, {}> {

    public render() {
        let players = this.props.players;
        let playerList = Object.keys(players).map((key, index) => {
            let player = players[Number(key)];
            return (
                <MapPlayerListItem
                    player={player}
                    highlightOn={() => this.props.world.highlightOn(player.id)}
                    highlightOff={() => this.props.world.highlightOff(player.id)}
                    key={player.id} />
            );
        });

        return (
            <table className="table table-striped table-hover">
                <thead>
                <tr>
                    <td className="col-xs-2">Player Name</td>
                    <td className="col-xs-1">ID</td>
                    <td className="col-xs-2">Actions</td>
                </tr>
                </thead>
                <tbody>
                {playerList}
                </tbody>
            </table>
        );
    }
}

const mapStateToProps = (state: ApplicationState) => {
    return {
        players: state.mapState.players,
    };
};

export default connect(mapStateToProps)(MapPlayerList);
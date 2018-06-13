import React from "react";
import {Map} from "../stores/map/types";

export interface MapListProps {
    maps: Map[];
}

export class MapList extends React.Component<MapListProps, {}> {

    render() {
        if (this.props.maps != null) {
            var mapList = this.props.maps.map(map => <MapItem map={map} key={map.id + map.serverId * 0x10000}/>);

            return (
                <table className="table table-striped table-hover">
                    <thead>
                    <tr>
                        <td className="col-xs-10">Map Name</td>
                        <td className="col-xs-1">Players</td>
                        <td>Spectate</td>
                    </tr>
                    </thead>
                    <tbody>
                    {mapList}
                    </tbody>
                </table>
            );
        }
        return (<span>No game maps hosted</span>);
    }
}

class MapItem extends React.Component<{ map: Map }, {}> {

    private handleClick() {
        var liveMapUrl = this.getLiveMapUrl();
        window.open(liveMapUrl, "_blank");
    }

    private getLiveMapUrl() {
        return "/admin/livemap?serverId=" + this.props.map.serverId + "&mapId=" + this.props.map.id;
    }

    render() {
        return (
            <tr>
                <td>{this.props.map.name}</td>
                <td>{this.props.map.playerCount}</td>
                <td>
                    <button type="button" className="btn btn-default btn-xs glyphicon glyphicon-new-window" onClick={() => this.handleClick()}></button>
                </td>
            </tr>
        );
    }
}

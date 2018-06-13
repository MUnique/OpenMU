import React from "react";
import { connect } from "react-redux";
import { MapList } from "./MapList";

import { ApplicationState } from "../stores/index";

import {Server, ServerState} from "../stores/servers/types";
import {startServer, shutdownServer} from "../stores/servers/actions";



interface IServerItemProps {
    server: Server;
    start: (serverId: number) => void;
    shutdown: (serverId: number) => void;
}

interface ServerItemState {
    expanded: boolean;
}


class ServerItem extends React.Component<IServerItemProps, ServerItemState> {

    constructor(props: IServerItemProps) {
        super(props);
        this.state = { expanded: false };
    }

    handleClick() {
        let serverId = this.props.server.id;
        if (this.props.server.state === ServerState.Started) {
            this.props.shutdown(serverId);
        } else if (this.props.server.state === ServerState.Stopped) {
            this.props.start(serverId);
        }
    }

    expand() {
        this.setState({ expanded: !this.state.expanded });
    }

    getActionCaption() {
        if (this.props.server.state === ServerState.Started)
            return "Shutdown";
        else
            return "Start";
    }

    isActionAvailable(): boolean {
        return (this.props.server.state === ServerState.Started || this.props.server.state === ServerState.Stopped);
    }

    getActionClass(): string {

        if (this.props.server.state === ServerState.Started)
            return 'btn btn-xs btn-success';
        else
            return 'btn btn-xs btn-warning';
    }

    getExpandItemClass(): string {
        var buttonClass = "btn btn-default btn-xs ";
        if (this.state.expanded) {
            return buttonClass + 'glyphicon glyphicon-minus';
        } else {
            return buttonClass + 'glyphicon glyphicon-plus';
        }
    }

    render() {
        return (
            <tr className={this.props.server.state === ServerState.Started ? 'success' : 'warning'}>
                <td>
                    <button type="button" className={this.getExpandItemClass()} onClick={() => this.expand()}>
                    </button>
                </td>
                <td>
                    <table>
                        <thead>
                        <tr>
                            <td>{this.props.server.description}</td>
                        </tr>
                        </thead>
                        {this.state.expanded
                            ? <tbody><tr><td><MapList maps={this.props.server.maps}/></td></tr></tbody>
                            : null}
                    </table>
                </td>
                <td>
                    <div>{this.props.server.onlinePlayerCount} / {this.props.server.maximumPlayers < 2000000 ? this.props.server.maximumPlayers : "∞"}</div>
                </td>
                <td>{ServerState.getCaption(this.props.server.state)}</td>
                <td>
                    <button type="button" disabled={!this.isActionAvailable()} className={this.getActionClass()} onClick={() => this.handleClick()}>{this.getActionCaption()}</button>
                </td>
            </tr>
        );
    }
}

const mapStateToProps = (state: ApplicationState) => {
    return { };
};

const mapDispatchToProps = (dispatch: any) => {
    return {
        start: (serverId: number) => dispatch(startServer(serverId)),
        shutdown: (serverId: number) => dispatch(shutdownServer(serverId)),
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(ServerItem);
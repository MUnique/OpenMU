import React from "react";
import { connect } from "react-redux";
import ServerItem from "./ServerItem";
import { Server } from "../stores/servers/types";


import {TotalOnlineCounter} from "./TotalOnlineCounter";
import { ApplicationState } from "../stores/index";
import {serverListSubscribe, serverListUnsubscribe} from "../stores/servers/actions";


interface ServerListProps {
    servers: Server[];
    subscribe: (subscriber: any) => void;
    unsubscribe: (subscriber: any) => void;
}

class ServerList extends React.Component<ServerListProps, {}> {

    componentDidMount() {
        this.props.subscribe(this);
    }

    componentWillUnmount() {
        this.props.unsubscribe(this);
    }

    render() {
        var serverList = this.props.servers.map((server: Server) => <ServerItem server={server} key={server.id}/>);

        return (
            <table className="table table-striped table-hover">
                <thead>
                <tr>
                    <th></th>
                    <th className="col-xs-7">Server Name</th>
                    <th className="col-xs-1">Players</th>
                    <th className="col-xs-2">Current State</th>
                    <th className="col-xs-2">Action</th>
                </tr>
                </thead>
                <tfoot>
                    <TotalOnlineCounter servers={this.props.servers}/>
                </tfoot>
                <tbody>
                {serverList}
                </tbody>
            </table>
        );
    }
}


const mapStateToProps = (state: ApplicationState) => {
    return { servers: state.serverListState.servers };
};

const mapDispatchToProps = (dispatch: any) => {
    return {
        subscribe: (subscriber: any) => dispatch(serverListSubscribe(subscriber)),
        unsubscribe: (subscriber: any) => dispatch(serverListUnsubscribe(subscriber)),
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(ServerList);
import React from "react";
import { connect } from "react-redux";
import ServerItem from "./ServerItem";
import { Server, ConnectServerSettings, GameClientDefinition } from "../stores/servers/types";


import { TotalOnlineCounter } from "./TotalOnlineCounter";
import ConnectServerConfiguration from "./ConnectServerConfiguration";
import { ApplicationState } from "../stores/index";
import { serverListSubscribe, serverListUnsubscribe } from "../stores/servers/actions";
import { showModal, hideModal } from "../stores/modal/actions";


interface ServerListProps {
    servers: Server[];
    clients: GameClientDefinition[];
    subscribe: (subscriber: any) => void;
    unsubscribe: (subscriber: any) => void;
    showModal: (content: any) => void;
    hideModal: () => void;
}

class ServerList extends React.Component<ServerListProps, { }> {
    componentDidMount() {
        this.props.subscribe(this);
    }

    componentWillUnmount() {
        this.props.unsubscribe(this);
    }

    render() {
        var serverList = this.props.servers.map((server: Server) => <ServerItem server={server} key={server.id}/>);

        return (
            <div>
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
                <p>
                    <button type="button" className="btn btn-info" disabled={this.props.clients.length === 0} onClick={() => this.showConnectionServerConfig()}>
                    <span className="glyphicon glyphicon-asterisk" aria-hidden="true"></span> Create Connect Server
                </button>
                </p>
                <p>
                <button type="button" className="btn btn-info" onClick={() => this.showGameServerCreationDialog()}>
                    <span className="glyphicon glyphicon-star" aria-hidden="true"></span> Create Game Server
                </button>
                </p>
            </div>
        );
    }

    showConnectionServerConfig() {
        var modalContent = (<ConnectServerConfiguration
            id="create-connect-server"
            serverSettings={ConnectServerSettings.createNew(this.props.clients[0])}
            onSaveSuccess={() => this.props.hideModal()}
            onCancel={() => this.props.hideModal()} />);
        this.props.showModal(modalContent);
    }

    showGameServerCreationDialog() {
        alert("feature not implemented yet.");
    }
}


const mapStateToProps = (state: ApplicationState) => {
    return { servers: state.serverListState.servers, clients: state.serverListState.clients };
};

const mapDispatchToProps = (dispatch: any) => {
    return {
        subscribe: (subscriber: any) => dispatch(serverListSubscribe(subscriber)),
        unsubscribe: (subscriber: any) => dispatch(serverListUnsubscribe(subscriber)),
        showModal: (modalContent: any) => dispatch(showModal(modalContent)),
        hideModal: () => dispatch(hideModal()),
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(ServerList);

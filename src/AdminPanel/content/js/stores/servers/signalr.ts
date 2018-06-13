import { Middleware, MiddlewareAPI, Action, Store, Dispatch } from "redux";
import { Server } from "./types";
import { Map } from "../map/types";
import { ApplicationState } from "../index";
import { SignalRConnector, HubProxy } from "../signalr";
import { Constants, serverListInit, serverUpdateState, serverUpdateMapPlayerCount, serverUpdatePlayerCount, serverAddMap, serverRemoveMap, ServerListSubscribeAction, ServerListUnsubscribeAction} from "./actions";

declare var $: { connection: { serverListHub: ServerHubProxy } };

interface ServerHubClient {
    initialize: (servers: Server[]) => void;

    // Is called when a server got added to the list;
    // Currently probably never called, because there are no servers dynamically added or removed.
    addedServer: (server: Server) => void;

    // Is called when a server got removed from the list;
    // Currently probably never called, because there are no servers dynamically added or removed.
    removedServer: (serverId: number) => void;

    playerCountChanged: (serverId: number, playerCount: number) => void;

    mapPlayerCountChanged: (serverId: number, mapId: number, playerCount: number) => void;

    serverStateChanged: (serverId: number, newState: number) => void;

    mapAdded: (mapInfo: Map) => void;

    mapRemoved: (serverId: number, mapId: number) => void;
}

interface ServerHubProxy extends HubProxy {
    client: ServerHubClient;
}

interface ServerHub {
}

export var serverListConnector: ServerListSignalRConnector;

class ServerListSignalRConnector extends SignalRConnector<ServerHubProxy> {

    constructor(store: Store<ApplicationState>) {
        super(store);
    }

    protected initializeHub(): ServerHubProxy {

        var hubProxy = $.connection.serverListHub;
        hubProxy.client.initialize = (servers: Server[]) => this.initialize(servers);
        hubProxy.client.serverStateChanged = (serverId: number, newState: number) => this.store.dispatch(serverUpdateState(serverId, newState));
        hubProxy.client.playerCountChanged = (serverId: number, playerCount: number) => this.store.dispatch(serverUpdatePlayerCount(serverId, playerCount));
        hubProxy.client.mapPlayerCountChanged = (serverId: number, mapId: number, playerCount: number) => this.store.dispatch(serverUpdateMapPlayerCount(serverId, mapId, playerCount));
        hubProxy.client.addedServer = (server: Server) => this.addServer(server);
        hubProxy.client.removedServer = (serverId: number) => this.removeServer(serverId);
        hubProxy.client.mapAdded = (map: Map) => this.store.dispatch(serverAddMap(map));
        hubProxy.client.mapRemoved = (serverId: number, mapId: number) => this.store.dispatch(serverRemoveMap(serverId, mapId));
        return hubProxy;
    }

    private initialize(servers: Server[]) {
        if (console && console.log) {
            console.log("server hub: init");
        }

        this.store.dispatch(serverListInit(servers));
    }

    private addServer(server: Server) {
        if (console && console.log) {
            console.warn("server hub: add server - not implemented");
        }
    }

    private removeServer(serverId: number) {
        if (console && console.log) {
            console.warn("server hub: remove server - not implemented");
        }
    }
}

export const serverListConnectorMiddleware: Middleware =
    (api: MiddlewareAPI) =>
        (next: Dispatch) =>
        (action: Action) => {
            if (action.type === Constants.SERVERLIST_SUBSCRIBE) {
                serverListConnector.subscribe((action as ServerListSubscribeAction).subscriber);
            } else if (action.type === Constants.SERVERLIST_UNSUBSCRIBE) {
                serverListConnector.unsubscribe((action as ServerListUnsubscribeAction).subscriber);
            } else { 
                next(action);
            }
        };

export function configureServerListConnector(store: Store<ApplicationState>): void {
    serverListConnector = new ServerListSignalRConnector(store);
}
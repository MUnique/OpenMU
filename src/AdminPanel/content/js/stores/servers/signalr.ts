import { Middleware, MiddlewareAPI, Action, Store, Dispatch } from "redux";
import { Server } from "./types";
import { Map } from "../map/types";
import { ApplicationState } from "../index";
import { SignalRConnector } from "../signalr";
import { Constants, serverListInit, serverUpdateState, serverUpdateMapPlayerCount, serverUpdatePlayerCount, serverAddMap, serverRemoveMap, ServerListSubscribeAction, ServerListUnsubscribeAction} from "./actions";

export var serverListConnector: ServerListSignalRConnector;

class ServerListSignalRConnector extends SignalRConnector {

    constructor(store: Store<ApplicationState>) {
        super(store);
    }

    protected onBeforeConnect(): void {
        
        this.connection.on("Initialize", this.initialize.bind(this));
        this.connection.on("ServerStateChanged", (serverId: number, newState: number) => this.store.dispatch(serverUpdateState(serverId, newState)));
        this.connection.on("PlayerCountChanged", (serverId: number, playerCount: number) => this.store.dispatch(serverUpdatePlayerCount(serverId, playerCount)));
        this.connection.on("MapPlayerCountChanged", (serverId: number, mapId: number, playerCount: number) => this.store.dispatch(serverUpdateMapPlayerCount(serverId, mapId, playerCount)));
        this.connection.on("AddedServer", (server: Server) => this.addServer(server));
        this.connection.on("RemovedServer", (serverId: number) => this.removeServer(serverId));
        this.connection.on("MapAdded", (map: Map) => this.store.dispatch(serverAddMap(map)));
        this.connection.on("MapRemoved", (serverId: number, mapId: number) => this.store.dispatch(serverRemoveMap(serverId, mapId)));
    }

    protected getHubPath(): string {
        return "/signalr/hubs/serverListHub";
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
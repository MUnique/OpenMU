import Redux from "redux";
import { Server, GameClientDefinition } from "./types";
import { Constants, ServerAction as ServerStateAction, ServerUpdateStateAction, ServerUpdatePlayerCountAction,
    ServerUpdateMapPlayerCountAction, ServerListInitAction, ServerAddMapAction, ServerRemoveMapAction } from
    "./actions";

export type ServerListState = {
    readonly servers: Server[];
    readonly clients: GameClientDefinition[];
};

export const initialState: ServerListState =
{
    servers: [],
    clients: [],
};

// This reducer handles the server actions and returns the new server list state.
export const serverStateReducer: Redux.Reducer<ServerListState> =
    (state: ServerListState = initialState, action: Redux.Action) => {
        switch ((action as Redux.Action).type) {
        case Constants.SERVER_UPDATE_STATE:
        {
            let serverUpdateAction = action as ServerUpdateStateAction;
            return getModificatedState(state,
                serverUpdateAction,
                () => { return { state: serverUpdateAction.newState } });
        }
        case Constants.SERVER_UPDATE_PLAYERCOUNT:
        {
            let serverUpdateAction = action as ServerUpdatePlayerCountAction;
            return getModificatedState(state,
                serverUpdateAction,
                () => { return { onlinePlayerCount: serverUpdateAction.playerCount } });
        }
        case Constants.SERVER_UPDATE_MAP_PLAYERCOUNT:
        {
            let serverUpdateAction = action as ServerUpdateMapPlayerCountAction;
            return getModificatedState(state,
                serverUpdateAction,
                (server) => { return updateGameMapPlayerCount(server, serverUpdateAction) });
        }
        case Constants.SERVER_MAP_ADD:
        {
            let serverUpdateAction = action as ServerAddMapAction;
            return getModificatedState(state,
                serverUpdateAction,
                (server) => {
                    let maps = server.maps.slice(0, server.maps.length);
                    maps.push(serverUpdateAction.map);
                    return { ...server, maps: maps };
                });
        }
        case Constants.SERVER_MAP_REMOVE:
        {
            let serverUpdateAction = action as ServerRemoveMapAction;
            return getModificatedState(state,
                serverUpdateAction,
                (server) => {
                    return {
                        ...server,
                        maps: server.maps.filter(map => map.id !== serverUpdateAction.mapId)
                    };
                });
        }
        case Constants.SERVERLIST_INIT:
            let serverInitAction = action as ServerListInitAction;
            return { ...state, servers: serverInitAction.servers, clients: serverInitAction.clients };
        default:
            return state;

        }
    };

function updateGameMapPlayerCount(server: Server, action: ServerUpdateMapPlayerCountAction) : Partial<Server> {
    let maps = server.maps.slice(0, server.maps.length);
    let index = maps.findIndex(map => map.id === action.mapId);
    maps[index] = {...maps[index], playerCount: action.playerCount}
    return { maps };
}

function getModificatedState(state: ServerListState, action: ServerStateAction, modifiedProperties: (server: Server) => Partial<Server>): ServerListState {
    let serverUpdateAction = action as ServerUpdateStateAction;
    let index = state.servers.findIndex((server: Server) => { return server.id === serverUpdateAction.serverId });
    if (index >= 0) {
        let server = state.servers[index];
        let newServers = state.servers.slice(0, state.servers.length);
        if (index >= 0) {
            var changed = modifiedProperties(server);
            newServers[index] = { ...server, ...changed };
        }

        return { ...state, servers: newServers };
    }

    return state;
}
import Redux from "redux";
import { Server, ConnectServerSettings, GameClientDefinition } from "./types";
import { Map } from "../map/types";

export enum Constants {
    SERVER_UPDATE_PLAYERCOUNT = "SERVER_UPDATE_PLAYERCOUNT",
    SERVER_UPDATE_MAP_PLAYERCOUNT = "SERVER_UPDATE_MAP_PLAYERCOUNT",
    SERVER_UPDATE_STATE = "SERVER_UPDATE_STATE",
    SERVERLIST_INIT = "SERVERLIST_INIT",
    SERVERLIST_SUBSCRIBE = "SERVERLIST_SUBSCRIBE",
    SERVERLIST_UNSUBSCRIBE = "SERVERLIST_UNSUBSCRIBE",
    SERVER_MAP_ADD = "SERVER_MAP_ADD",
    SERVER_MAP_REMOVE = "SERVER_MAP_REMOVE",
}

export interface ServerListSubscribeAction extends Redux.Action {
    type: Constants.SERVERLIST_SUBSCRIBE,
    subscriber: any,
}

export interface ServerListUnsubscribeAction extends Redux.Action {
    type: Constants.SERVERLIST_UNSUBSCRIBE,
    subscriber: any,
}

export interface ServerAction extends Redux.Action {
    serverId: number,
}

export interface ServerUpdateStateAction extends ServerAction {
    type: Constants.SERVER_UPDATE_STATE,  
    newState: number,
}

export interface ServerUpdatePlayerCountAction extends ServerAction {
    type: Constants.SERVER_UPDATE_PLAYERCOUNT,
    playerCount: number,
}

export interface ServerUpdateMapPlayerCountAction extends ServerAction {
    type: Constants.SERVER_UPDATE_MAP_PLAYERCOUNT,
    mapId: number,
    playerCount: number,
}

export interface ServerAddMapAction extends ServerAction {
    type: Constants.SERVER_MAP_ADD,
    map: Map,
}

export interface ServerRemoveMapAction extends ServerAction {
    type: Constants.SERVER_MAP_REMOVE,
    mapId: number,
}

export interface ServerListInitAction extends Redux.Action {
    type: Constants.SERVERLIST_INIT,
    servers: Server[],
    clients: GameClientDefinition[],
}


export const serverUpdateState: Redux.ActionCreator<ServerUpdateStateAction> = (serverId: number, newState: number) => ({
    type: Constants.SERVER_UPDATE_STATE,
    serverId,
    newState,
});

export const serverUpdatePlayerCount: Redux.ActionCreator<ServerUpdatePlayerCountAction> =
    (serverId: number, playerCount: number) => ({
        type: Constants.SERVER_UPDATE_PLAYERCOUNT,
        serverId,
        playerCount,
    });

export const serverUpdateMapPlayerCount: Redux.ActionCreator<ServerUpdateMapPlayerCountAction> =
    (serverId: number, mapId: number, playerCount: number) => ({
        type: Constants.SERVER_UPDATE_MAP_PLAYERCOUNT,
        serverId,
        mapId,
        playerCount,
    });

export const serverAddMap: Redux.ActionCreator<ServerAddMapAction> =
    (map: Map) => ({
        type: Constants.SERVER_MAP_ADD,
        serverId: map.serverId,
        map: map
    });

export const serverRemoveMap: Redux.ActionCreator<ServerRemoveMapAction> =
    (serverId: number, mapId: number) => ({
        type: Constants.SERVER_MAP_REMOVE,
        serverId,
        mapId,
    });


export const serverListSubscribe: Redux.ActionCreator<ServerListSubscribeAction> =
    (subscriber: any) => ({ type: Constants.SERVERLIST_SUBSCRIBE, subscriber });

export const serverListUnsubscribe: Redux.ActionCreator<ServerListUnsubscribeAction> =
    (subscriber: any) => ({ type: Constants.SERVERLIST_UNSUBSCRIBE, subscriber });

export const serverListInit: Redux.ActionCreator<ServerListInitAction> =
    (servers: Server[], clients: GameClientDefinition[]) => ({
        type: Constants.SERVERLIST_INIT,
        servers: servers,
        clients: clients,
    });

export function startServer(serverId: number) {
    return serverAction(serverId, "start");
}

export function shutdownServer(serverId: number) {
    return serverAction(serverId, "shutdown");
}

function serverAction(serverId: number, actionName: string) {
    return (dispatch: Redux.Dispatch) => {
        return fetch("/admin/server/" + actionName + "/" + serverId)
            .catch(error => { console.error("something went wrong during " + actionName + " of server " + serverId, error) });
    }
}

export function saveConnectServerConfiguration(configuration: ConnectServerSettings, successCallback: () => void, errorCallback: (error: any) => void) {
    return (dispatch: Redux.Dispatch) => {
        return fetch("/admin/connectServer",
                {
                    body: JSON.stringify(configuration),
                    cache: 'no-cache',
                    headers: {
                        'content-type': 'application/json'
                    },
                    method: 'POST',
            })
            .then(response => {
                // configuration.id = response;
                if (successCallback) {
                    successCallback();
                }               
            })
            .catch(error => {
                console.error("something went wrong during saving connect server " + configuration.id, error);
                if (errorCallback) {
                    errorCallback(error);
                }
            });
    }
}

export function deleteConnectServerConfiguration(configuration: ConnectServerSettings) {
    return (dispatch: Redux.Dispatch) => {
        return fetch("/admin/connectServer/" + configuration.id,
                {
                    method: 'DELETE',
                })
            .catch(error => { console.error("something went wrong during deleting connect server " + configuration.id, error) });
    }
}
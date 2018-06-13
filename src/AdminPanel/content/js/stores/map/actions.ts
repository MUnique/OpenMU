import Redux from "redux";
import {PlayerData} from "./types";

export enum Constants {
    MAP_ADD_PLAYER = "MAP_ADD_PLAYER",
    MAP_REMOVE_PLAYER = "MAP_REMOVE_PLAYER",
    MAP_HIGHLIGHT_ON = "MAP_HIGHLIGHT_ON",
    MAP_HIGHLIGHT_OFF = "MAP_HIGHLIGHT_OFF",
}

export interface MapHighlightAction extends Redux.Action {
    objectId: number,
}
export interface MapHighlightOnAction extends Redux.Action, MapHighlightAction {
    type: Constants.MAP_HIGHLIGHT_ON,
    objectId: number,
}

export interface MapHighlightOffAction extends Redux.Action, MapHighlightAction {
    type: Constants.MAP_HIGHLIGHT_OFF,
    objectId: number,
}

export interface MapAddPlayerAction extends Redux.Action {
    type: Constants.MAP_ADD_PLAYER,
    player: PlayerData,
}

export interface MapRemovePlayerAction extends Redux.Action {
    type: Constants.MAP_REMOVE_PLAYER,
    player: PlayerData,
}

export const highlightPlayerOnMap: Redux.ActionCreator<MapHighlightOnAction> = (objectId: number) => ({
    type: Constants.MAP_HIGHLIGHT_ON,
    objectId,
});

export const unhighlightPlayerOnMap: Redux.ActionCreator<MapHighlightOffAction> = (objectId: number) => ({
    type: Constants.MAP_HIGHLIGHT_OFF,
    objectId,
});

export const addPlayer: Redux.ActionCreator<MapAddPlayerAction> = (player: PlayerData) => ({
    type: Constants.MAP_ADD_PLAYER,
    player,
});

export const removePlayer: Redux.ActionCreator<MapRemovePlayerAction> = (player: PlayerData) => ({
    type: Constants.MAP_REMOVE_PLAYER,
    player,
});


export function banPlayer(player: PlayerData) {
    return playerAction(player, "ban");
}

export function disconnectPlayer(player: PlayerData) {
    return playerAction(player, "disconnect");
}

function playerAction(player: PlayerData, actionName: string) {
    return (dispatch: Redux.Dispatch) => {
        return fetch("/admin/player/" + actionName + "/" + player.serverId + "/" + player.name)
            .then(
                response => response.json(),
                error => {
                    console.error("something went wrong during " + actionName + " of player " + player.name, error);
                });
    }
}
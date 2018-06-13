import Redux from "redux";
import { PlayerData } from "./types";
import {
    Constants,
    MapAddPlayerAction,
    MapRemovePlayerAction,
    MapHighlightAction,
} from "./actions";

export type MapState = {
    readonly players: { [id: number]: PlayerData };
};

const initialState: MapState =
{
    players: {}
};


// This reducer handles the account actions and returns the new account list state.
export const mapStateReducer: Redux.Reducer<MapState> =
    (state: MapState = initialState, action: Redux.Action) => {
        let actionType = (action as Redux.Action).type;
        switch (actionType) {
        case Constants.MAP_ADD_PLAYER:
        {
            let addPlayerAction = action as MapAddPlayerAction;
            let player = addPlayerAction.player;
            let newPlayers = { ...state.players };

            newPlayers[player.id] = player;
            return { ...state, players: newPlayers };
        }
        case Constants.MAP_REMOVE_PLAYER:
        {
            let removePlayerAction = action as MapRemovePlayerAction;
            let player = removePlayerAction.player;
            let newPlayers = { ...state.players };

            delete newPlayers[player.id];

            return { ...state, players: newPlayers };
        }
        case Constants.MAP_HIGHLIGHT_ON:
        case Constants.MAP_HIGHLIGHT_OFF:
        {
            let playerHighlightAction = action as MapHighlightAction;
            let playerId = playerHighlightAction.objectId;
            let oldPlayer = state.players[playerId];
            let newPlayers = { ...state.players };
            newPlayers[playerId] = { ...oldPlayer, isHighlighted: actionType === Constants.MAP_HIGHLIGHT_ON };

            return {
                ...state,
                players: newPlayers
            };
        }

        default:
            return state;

        }
    };
import React from "react";
import ReactDOM from "react-dom";
import { Provider } from "react-redux";
import { World } from "./World";
import MapPlayerList from "./MapPlayerList";

import configureStore from "../stores/store";

import { initialState as initialServersState } from "content/js/stores/servers/reducer";
import { initialState as initialLogState } from "content/js/stores/log/reducer";
import { initialState as initialAccountState } from "content/js/stores/accounts/reducer";
import { initialState as initialPluginsState } from "content/js/stores/plugins/reducer";
import { initialState as initialFetchState } from "content/js/stores/fetch/reducer";
import { initialState as initialModalState } from "content/js/stores/modal/reducer";


export const store = configureStore(
    {
        fetchState: initialFetchState,
        modalState: initialModalState,
        accountListState: initialAccountState,
        logTableState: initialLogState,
        serverListState: initialServersState,
        mapState: {
            players: {}
        },
        systemState: { snapshots: [] },
        plugInListState: initialPluginsState,
    }
);


export function renderPlayerList(playerListContainer: HTMLElement, world: World) : void
{
    ReactDOM.render(
        <Provider store={store}>
            <MapPlayerList world={world}/>
        </Provider>,
        playerListContainer
    );
}
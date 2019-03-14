import React from "react";
import ReactDOM from "react-dom";
import { Provider } from "react-redux";
import { World } from "./World";
import MapPlayerList from "./MapPlayerList";

import configureStore from "../stores/store";


export const store = configureStore(
    {
        fetchState: { isFetching: false },
        accountListState: { accounts: [], page: 1, pageSize: 20, createDialogVisible: false },
        logTableState: { entries: [], characterFilter: null, serverFilter: null, autoRefresh: true, loggerFilter: null, loggers: [], idOfLastReceivedEntry: 0 },
        serverListState: { servers: [] },
        mapState: {
            players: { }
        },
        systemState: { snapshots: [] },
        plugInListState: { plugins: [], page: 1, pageSize: 20, createDialogVisible: false },
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
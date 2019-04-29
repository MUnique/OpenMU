import React from "react";
import ReactDOM from "react-dom";
import { Provider } from "react-redux";
import { Layout } from "./Layout";

import configureStore from "../stores/store";
import { initialState as initialServersState } from "content/js/stores/servers/reducer";
import { initialState as initialLogState } from "content/js/stores/log/reducer";
import { initialState as initialAccountState } from "content/js/stores/accounts/reducer";
import { initialState as initialPluginsState } from "content/js/stores/plugins/reducer";
import { initialState as initialFetchState } from "content/js/stores/fetch/reducer";
import { initialState as initialModalState } from "content/js/stores/modal/reducer";


const store = configureStore(
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



// This here is our entry point
ReactDOM.render(
    <Provider store={store}>
        <Layout />
    </Provider>, document.getElementById("content")
);
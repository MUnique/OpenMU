import React from "react";
import ReactDOM from "react-dom";
import { Provider } from "react-redux";
import { Layout } from "./Layout";

import configureStore from "../stores/store";


const store = configureStore(
    {
        fetchState: { isFetching: false },
        accountListState: { accounts: [], page: 1, pageSize: 20, createDialogVisible: false },
        logTableState: {
            entries: [],
            characterFilter: null,
            serverFilter: null,
            autoRefresh: true,
            loggerFilter: null,
            loggers: [],
            idOfLastReceivedEntry: 0,
        },
        serverListState: { servers: [] },
        mapState: {
            players: {}
        },
        systemState: { snapshots: [] },
        plugInListState: { plugins: [], page: 1, pageSize: 20, createDialogVisible: false },
    }
);



// This here is our entry point
ReactDOM.render(
    <Provider store={store}>
        <Layout />
    </Provider>, document.getElementById("content")
);
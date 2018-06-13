import { Store, createStore, applyMiddleware } from 'redux';
import thunk from 'redux-thunk';
import { ApplicationState, reducers } from "./index";
import {configureServerListConnector, serverListConnectorMiddleware } from "./servers/signalr";
import { configureLogTableConnector, logTableConnectorMiddleware} from "./log/signalr";
import {fetchMiddleware} from "./fetch/actions";
import { configureSystemHubConnector, systemHubConnectorMiddleware} from "./system/signalr";

export default function configureStore(initialState: ApplicationState): Store<ApplicationState> {
    
    
    let store = createStore(reducers, initialState, applyMiddleware(fetchMiddleware, logTableConnectorMiddleware, serverListConnectorMiddleware, systemHubConnectorMiddleware, thunk));
    configureLogTableConnector(store);
    configureServerListConnector(store);
    configureSystemHubConnector(store);

    return store;
}
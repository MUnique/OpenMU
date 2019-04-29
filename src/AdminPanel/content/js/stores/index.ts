import Redux from "redux";
import { AccountListState, accountStateReducer}  from "./accounts/reducer";
import { FetchState, fetchStateReducer } from "./fetch/reducer";
import { ServerListState, serverStateReducer } from "./servers/reducer";
import { LogTableState, logTableStateReducer } from "./log/reducer";
import { MapState, mapStateReducer } from "./map/reducer";
import { SystemState, systemStateReducer } from "./system/reducer";
import { PlugInListState, plugInListStateReducer } from "./plugins/reducer";
import { ModalState, modalStateReducer } from "./modal/reducer";


export type ApplicationState = {
    fetchState: FetchState;
    modalState: ModalState;
    accountListState: AccountListState;
    serverListState: ServerListState;
    logTableState: LogTableState;
    mapState: MapState;
    systemState: SystemState;
    plugInListState: PlugInListState;
}

export const reducers: Redux.Reducer<ApplicationState> = Redux.combineReducers<ApplicationState>({
    fetchState: fetchStateReducer,
    modalState: modalStateReducer,
    accountListState: accountStateReducer,
    serverListState: serverStateReducer,
    logTableState: logTableStateReducer,
    mapState: mapStateReducer,
    systemState: systemStateReducer,
    plugInListState: plugInListStateReducer,
});
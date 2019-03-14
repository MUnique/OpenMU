import Redux from "redux";
import { PlugInConfiguration } from "./types";
import {
    Constants,
    FetchPlugInsSuccessAction,
    FetchPlugInsErrorAction,
    PlugInSaveSuccessAction,
    PlugInSaveErrorAction,
    CustomPlugInDeleteSuccessAction,
    CustomPlugInDeleteErrorAction
} from "./actions";


export type PlugInListState = {
    readonly page: number,
    readonly pageSize: number,
    readonly plugins: PlugInConfiguration[],
    readonly createDialogVisible: boolean,
};

const initialState: PlugInListState =
{
    page: 1,
    pageSize: 20,
    plugins: [],
    createDialogVisible: false,
};


// This reducer handles the plugin actions and returns the new plugin list state.
export const plugInListStateReducer: Redux.Reducer<PlugInListState> =
    (state: PlugInListState = initialState, action: Redux.Action) => {
        switch ((action as Redux.Action).type) {
            case Constants.PLUGINS_FETCH_OK:
                let fetchSuccessAction = action as FetchPlugInsSuccessAction;
                return { ...state, pageSize: fetchSuccessAction.pageSize, page: fetchSuccessAction.page, plugins: fetchSuccessAction.response };
            case Constants.PLUGINS_FETCH_ERROR:
                let fetchErrorAction = action as FetchPlugInsErrorAction;
                console.log('An error occurred when fetching plugins.', fetchErrorAction.error);
                return state;

            case Constants.PLUGIN_SAVE_OK:
                console.log('PLUGIN_SAVE_OK');
                let pluginSaveAction = action as PlugInSaveSuccessAction;
                let plugInConfiguration = pluginSaveAction.plugin;
                let index = state.plugins.findIndex((plugin : PlugInConfiguration) => { return plugin.id === plugInConfiguration.id });

                let newAccounts = state.plugins.slice(0, state.plugins.length);
                if (index >= 0) {
                    newAccounts[index] = plugInConfiguration;
                } else {
                    newAccounts.push(plugInConfiguration);
                }
                return { ...state, plugins: newAccounts, createDialogVisible: false };
            case Constants.PLUGIN_SAVE_ERROR:
                let pluginSaveErrorAction = action as PlugInSaveErrorAction;
                console.log('An error occurred when saving an plugin.', pluginSaveErrorAction.error);

                return state;
            case Constants.PLUGIN_DELETE_OK:
                let pluginDeleteAction = action as CustomPlugInDeleteSuccessAction;
                return { ...state, plugins: state.plugins.filter(plugin => plugin !== pluginDeleteAction.plugin) };
            case Constants.PLUGIN_DELETE_ERROR:
                console.log('An error occurred when deleting an plugin.', (action as CustomPlugInDeleteErrorAction).error);
            case Constants.PLUGIN_SHOW_CREATE:
                return { ...state, createDialogVisible: true };
            case Constants.PLUGIN_HIDE_CREATE:
                console.log('PLUGIN_HIDE_CREATE');
                return { ...state, createDialogVisible: false };
            default:
                return state;
        }
    };
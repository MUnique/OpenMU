import Redux from "redux";
import { PlugInConfiguration, PlugInExtensionPoint } from "./types";
import {
    Constants,
    FetchPlugInsSuccessAction,
    FetchPlugInsErrorAction,
    FetchPlugInPointsSuccessAction,
    FetchPlugInPointsErrorAction,
    PlugInSaveSuccessAction,
    PlugInSaveErrorAction,
    CustomPlugInDeleteSuccessAction,
    CustomPlugInDeleteErrorAction
} from "./actions";

export type PlugInListState = {
    readonly page: number,
    readonly pageSize: number,
    readonly hasMoreEntries: boolean,
    readonly plugins: PlugInConfiguration[],
    readonly extensionPoints: PlugInExtensionPoint[],
    readonly selectedExtensionPointId: any,
    readonly createDialogVisible: boolean,
    readonly filterName: string,
    readonly filterType: string,
};

const initialState: PlugInListState =
{
    page: 1,
    pageSize: 20,
    hasMoreEntries: false,
    plugins: [],
    extensionPoints: [],
    selectedExtensionPointId: null,
    createDialogVisible: false,
    filterName: "",
    filterType: "",
};

// This reducer handles the plugin actions and returns the new plugin list state.
export const plugInListStateReducer: Redux.Reducer<PlugInListState> =
    (state: PlugInListState = initialState, action: Redux.Action) => {
        switch ((action as Redux.Action).type) {
            case Constants.PLUGINS_FETCH_OK:
                let fetchSuccessAction = action as FetchPlugInsSuccessAction;
                if (fetchSuccessAction.response.length === 0
                    && fetchSuccessAction.filterName === state.filterName
                    && fetchSuccessAction.filterType === state.filterType
                    && fetchSuccessAction.selectedExtensionPointId === state.selectedExtensionPointId) {
                    // When we changed the page and found nothing, we don't change the entries and page number etc.
                    // We do this because we don't want to change the page to an empty page.
                    return {
                        ...state,
                        hasMoreEntries: false,
                        selectedExtensionPointId: fetchSuccessAction.selectedExtensionPointId,
                        filterName: fetchSuccessAction.filterName,
                        filterType: fetchSuccessAction.filterType,
                    };
                }

                return {
                    ...state,
                    pageSize: fetchSuccessAction.pageSize,
                    page: fetchSuccessAction.page,
                    plugins: fetchSuccessAction.response,
                    hasMoreEntries: fetchSuccessAction.response.length === fetchSuccessAction.pageSize,
                    selectedExtensionPointId: fetchSuccessAction.selectedExtensionPointId,
                    filterName: fetchSuccessAction.filterName,
                    filterType: fetchSuccessAction.filterType,
                };
            case Constants.PLUGINS_FETCH_ERROR:
                let fetchErrorAction = action as FetchPlugInsErrorAction;
                console.log('An error occurred when fetching plugins.', fetchErrorAction.error);
                return state;
            case Constants.PLUGIN_POINTS_FETCH_OK:
                let fetchPointsSuccessAction = action as FetchPlugInPointsSuccessAction;
                return { ...state, extensionPoints: fetchPointsSuccessAction.response };
            case Constants.PLUGIN_POINTS_FETCH_ERROR:
                let fetchPointsErrorAction = action as FetchPlugInPointsErrorAction;
                console.log('An error occurred when fetching plugin extension points.', fetchPointsErrorAction.error);
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
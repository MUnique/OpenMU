import Redux from "redux";
import Action = Redux.Action;
import ActionCreator = Redux.ActionCreator;
import { PlugInConfiguration } from "./types";

export enum Constants {
    PLUGINS_FETCH_OK = "PLUGINS_FETCH_OK",
    PLUGINS_FETCH_ERROR = "PLUGINS_FETCH_ERROR",
    PLUGIN_SAVE_OK = "PLUGIN_SAVE_OK",
    PLUGIN_SAVE_ERROR = "PLUGIN_SAVE_ERROR",
    PLUGIN_DELETE_OK = "PLUGIN_DELETE_OK",
    PLUGIN_DELETE_ERROR = "PLUGIN_DELETE_ERROR",
    PLUGIN_SHOW_CREATE = "PLUGIN_SHOW_CREATE",
    PLUGIN_HIDE_CREATE = "PLUGIN_HIDE_CREATE",
};

export interface FetchPlugInsSuccessAction extends Action {
    type: Constants.PLUGINS_FETCH_OK,
    response?: PlugInConfiguration[],
    page: number;
    pageSize: number;
}

export interface FetchPlugInsErrorAction extends Action {
    type: Constants.PLUGINS_FETCH_ERROR,
    error: string,
}

interface PlugInConfigurationSuccessAction extends Action {
    type: any;
    plugin: PlugInConfiguration;
}

interface PlugInConfigurationErrorAction extends Action {
    type: any;
    plugin: PlugInConfiguration;
    error: string;
}


export interface PlugInSaveSuccessAction extends PlugInConfigurationSuccessAction {
    type: Constants.PLUGIN_SAVE_OK
}

export interface PlugInSaveErrorAction extends PlugInConfigurationErrorAction {
    type: Constants.PLUGIN_SAVE_ERROR
}

export interface CustomPlugInDeleteSuccessAction extends PlugInConfigurationSuccessAction {
    type: Constants.PLUGIN_DELETE_OK
}
export interface CustomPlugInDeleteErrorAction extends PlugInConfigurationErrorAction {
    type: Constants.PLUGIN_DELETE_ERROR
}

export interface ShowCreateDialogAction extends Action {
    type: Constants.PLUGIN_SHOW_CREATE
}

export interface HideCreateDialogAction extends Action {
    type: Constants.PLUGIN_HIDE_CREATE
}


export const savePlugInConfigurationSuccess: ActionCreator<PlugInSaveSuccessAction> = (plugin: PlugInConfiguration) => ({
    type: Constants.PLUGIN_SAVE_OK,
    plugin,
});

export const savePlugInConfigurationError: ActionCreator<PlugInSaveErrorAction> = (plugin: PlugInConfiguration, error: string) => ({
    type: Constants.PLUGIN_SAVE_ERROR,
    plugin,
    error
});

export const deletePlugInConfigurationSuccess: ActionCreator<CustomPlugInDeleteSuccessAction> = (plugin: PlugInConfiguration) => ({
    type: Constants.PLUGIN_DELETE_OK,
    plugin,
});

export const deletePlugInConfigurationError: ActionCreator<CustomPlugInDeleteErrorAction> = (plugin: PlugInConfiguration, error: string) => ({
    type: Constants.PLUGIN_DELETE_ERROR,
    plugin,
    error
});

export const fetchedPlugInConfigurationsSuccessfully: ActionCreator<FetchPlugInsSuccessAction> = (response: PlugInConfiguration[], page: number, pageSize: number) => ({
    type: Constants.PLUGINS_FETCH_OK,
    response,
    page,
    pageSize,
});

export const fetchPlugInConfigurationsFailed: ActionCreator<FetchPlugInsErrorAction> = (error: string) => ({
    type: Constants.PLUGINS_FETCH_ERROR,
    error,
});

export const showCreateDialog: ActionCreator<ShowCreateDialogAction> = () => ({
    type: Constants.PLUGIN_SHOW_CREATE
});

export const hideCreateDialog: ActionCreator<HideCreateDialogAction> = () => ({
    type: Constants.PLUGIN_HIDE_CREATE
});

export function fetchPlugInConfigurations(page: number, entriesPerPage: number) {
    return (dispatch: Redux.Dispatch) => {
        let offset = (page - 1) * entriesPerPage;
        return fetch("/admin/plugin/list/" + offset + "/" + entriesPerPage)
            .then(
                response => response.json(),
                error => {
                    dispatch(fetchPlugInConfigurationsFailed(error));
                })
            .then(response => {
                dispatch(fetchedPlugInConfigurationsSuccessfully(response, page, entriesPerPage));
            });
    }
}

export function deletePlugInConfiguration(plugin: PlugInConfiguration) {
    return (dispatch: Redux.Dispatch) => {
        return fetch("/admin/plugin/delete/" + plugin.id,
                {
                    cache: 'no-cache',
                    method: 'POST',
                })
            .then(() => {
                dispatch(deletePlugInConfigurationSuccess(plugin));
            })
            .catch(error => {
                dispatch(deletePlugInConfigurationError(plugin, error.toString()));
            });
    }
}

export function savePlugInConfiguration(plugin: PlugInConfiguration) {
    return (dispatch: Redux.Dispatch) => {

        return fetch("/admin/plugin/save",
                {
                    body: JSON.stringify(plugin),
                    cache: 'no-cache',
                    headers: {
                        'content-type': 'application/json'
                    },
                    method: 'POST',
                })
            .then(response => response.text())
            .then((response) => {
                if (plugin.id === null) {
                    plugin = {...plugin, id: response};
                }

                dispatch(savePlugInConfigurationSuccess(plugin));
            })
            .catch(error => {
                dispatch(savePlugInConfigurationError(plugin, error.toString()));
            });
    }
}
import Redux from "redux";
import Action = Redux.Action;
import ActionCreator = Redux.ActionCreator;
import { PlugInConfiguration, PlugInExtensionPoint } from "./types";
import { hideModal } from "content/js/stores/modal/actions";

export enum Constants {
    PLUGINS_FETCH_OK = "PLUGINS_FETCH_OK",
    PLUGINS_FETCH_ERROR = "PLUGINS_FETCH_ERROR",
    PLUGIN_POINTS_FETCH_OK = "PLUGINS_POINTS_FETCH_OK",
    PLUGIN_POINTS_FETCH_ERROR = "PLUGINS_POINTS_FETCH_ERROR",
    PLUGIN_SAVE_OK = "PLUGIN_SAVE_OK",
    PLUGIN_SAVE_ERROR = "PLUGIN_SAVE_ERROR",
    PLUGIN_DELETE_OK = "PLUGIN_DELETE_OK",
    PLUGIN_DELETE_ERROR = "PLUGIN_DELETE_ERROR",
};

export interface FetchPlugInsSuccessAction extends Action {
    type: Constants.PLUGINS_FETCH_OK,
    response?: PlugInConfiguration[],
    filterName: string,
    filterType: string,
    page: number;
    pageSize: number;
    selectedExtensionPointId: any;
}

export interface FetchPlugInsErrorAction extends Action {
    type: Constants.PLUGINS_FETCH_ERROR,
    error: string,
}

export interface FetchPlugInPointsSuccessAction extends Action {
    type: Constants.PLUGIN_POINTS_FETCH_OK,
    response?: PlugInExtensionPoint[],
}

export interface FetchPlugInPointsErrorAction extends Action {
    type: Constants.PLUGIN_POINTS_FETCH_ERROR,
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

export const fetchedPlugInConfigurationsSuccessfully: ActionCreator<FetchPlugInsSuccessAction> = (response: PlugInConfiguration[], selectedExtensionPointId: any, filterName: string, filterType: string, page: number, pageSize: number) => ({
    type: Constants.PLUGINS_FETCH_OK,
    response,
    selectedExtensionPointId,
    filterName,
    filterType,
    page,
    pageSize,
});

export const fetchPlugInConfigurationsFailed: ActionCreator<FetchPlugInsErrorAction> = (error: string) => ({
    type: Constants.PLUGINS_FETCH_ERROR,
    error,
});

export const fetchedPlugInPointsSuccessfully: ActionCreator<FetchPlugInPointsSuccessAction> = (response: PlugInExtensionPoint[]) => ({
    type: Constants.PLUGIN_POINTS_FETCH_OK,
    response,
});

export const fetchPlugInPointsFailed: ActionCreator<FetchPlugInPointsErrorAction> = (error: string) => ({
    type: Constants.PLUGIN_POINTS_FETCH_ERROR,
    error,
});

export function fetchPlugInConfigurations(extensionPoint: any, filterName: string, filterType: string, page: number, entriesPerPage: number) {
    return (dispatch: Redux.Dispatch) => {
        let offset = (page - 1) * entriesPerPage;

        var url = "/admin/plugin/list/" + extensionPoint + "/" + offset + "/" + entriesPerPage;
        if (filterName != null && filterName.length > 0) {
            url += "?name=" + filterName;
        }

        if (filterType != null && filterType.length > 0) {
            if (filterName != null && filterName.length > 0) {
                url += "&";
            } else {
                url += "?";
            }

            url += "type=" + filterType;
        }

        return fetch(url)
            .then(
                response => response.json(),
                error => {
                    dispatch(fetchPlugInConfigurationsFailed(error));
                })
            .then(response => {
                dispatch(fetchedPlugInConfigurationsSuccessfully(response, extensionPoint, filterName, filterType, page, entriesPerPage));
            });
    }
}

export function fetchPlugInPoints() {
    return (dispatch: Redux.Dispatch) => {
        return fetch("/admin/plugin/extensionpoints")
            .then(
                response => response.json(),
                error => {
                    dispatch(fetchPlugInPointsFailed(error));
                })
            .then(response => {
                dispatch(fetchedPlugInPointsSuccessfully(response));
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
                dispatch(hideModal());
            })
            .catch(error => {
                dispatch(savePlugInConfigurationError(plugin, error.toString()));
            });
    }
}
import Redux from "redux";
import { LogEventData } from "./types";
import {
    Constants,
    LogSetAutoRefreshAction,
    LogEventAction,
    LogInitializeAction,
    LogFilterByCharacterAction,
    LogFilterByLoggerAction,
    LogFilterByServerAction,
} from "./actions";

export interface LogTableState {
    readonly entries: LogEventData[];
    readonly idOfLastReceivedEntry: number;
    readonly loggers: string[];
    readonly autoRefresh: boolean; // unused?
    readonly loggerFilter: string;
    readonly characterFilter: string;
    readonly serverFilter: string;
}

const initialState: LogTableState = {
        entries: [],
        loggers: [],
        autoRefresh: true,
        loggerFilter: null,
        characterFilter: null,
        serverFilter: null,
        idOfLastReceivedEntry: 0,
};

export const logTableStateReducer: Redux.Reducer<LogTableState> =
    (state: LogTableState = initialState, action: Redux.Action) => {
        switch ((action as Redux.Action).type) {
        case Constants.LOG_SETAUTOREFRESH:
            let setAction = action as LogSetAutoRefreshAction;
            if (setAction.value) {
                return { ...state, autoRefresh: setAction.value };
            }

            return { ...state, autoRefresh: setAction.value };
        case Constants.LOG_LOGEVENT:
        {
            if (!state.autoRefresh) {
                return state;
            }

            let logAction = action as LogEventAction;
            let entries = state.entries.slice(0, state.entries.length);
            entries[entries.length] = logAction.event;
            if (entries.length > 200) {
                entries.shift();
            }

            return {
                ...state,
                entries,
                idOfLastReceivedEntry: Math.max(logAction.id, state.idOfLastReceivedEntry)
            };
        }
        case Constants.LOG_INITIALIZE:
        {
            let initAction = action as LogInitializeAction;
            let entries = state.entries.concat(initAction.cachedEvents);
            entries = entries.slice(Math.max(0, entries.length - 200),
                Math.min(entries.length, 200));
            return { ...state, loggers: initAction.loggers, entries: entries };
        }
        case Constants.LOG_FILTER_CHARACTER:
            let filterByCharacterAction = action as LogFilterByCharacterAction;
            return { ...state, characterFilter: filterByCharacterAction.characterName };

        case Constants.LOG_FILTER_LOGGER:
            let filterByLoggerAction = action as LogFilterByLoggerAction;
            return { ...state, loggerFilter: filterByLoggerAction.logger };
        case Constants.LOG_FILTER_SERVER:
            let filterByServerAction = action as LogFilterByServerAction;
            return { ...state, serverFilter: filterByServerAction.serverId };
        default:
            return state;

        }
    };
import Redux from "redux";
import Action = Redux.Action;
import ActionCreator = Redux.ActionCreator;
import { LogEntryData } from "./types";

export enum Constants {
    LOG_SUBSCRIBE = "LOG_SUBSCRIBE",
    LOG_UNSUBSCRIBE = "LOG_UNSUBSCRIBE",
    LOG_FILTER_SERVER = "LOG_FILTER_SERVER",
    LOG_FILTER_LOGGER = "LOG_FILTER_LOGGER",
    LOG_FILTER_CHARACTER = "LOG_FILTER_CHARACTER",
    LOG_SETAUTOREFRESH = "LOG_SETAUTOREFRESH",
    LOG_INITIALIZE = "LOG_INITIALIZE",
    LOG_LOGEVENT = "LOG_LOGEVENT",
};

export interface LogSubscribeAction extends Action {
    type: Constants.LOG_SUBSCRIBE,
    subscriber: any,
}
export interface LogUnsubscribeAction extends Action {
    type: Constants.LOG_UNSUBSCRIBE,
    subscriber: any,
}
export interface LogFilterByServerAction extends Action {
    type: Constants.LOG_FILTER_SERVER,
    serverId: string,
}
export interface LogFilterByLoggerAction extends Action {
    type: Constants.LOG_FILTER_LOGGER,
    logger: string,
}
export interface LogFilterByCharacterAction extends Action {
    type: Constants.LOG_FILTER_CHARACTER,
    characterName: string,
}

export interface LogSetAutoRefreshAction extends Action {
    type: Constants.LOG_SETAUTOREFRESH,
    value: boolean,
}

export interface LogInitializeAction extends Action {
    type: Constants.LOG_INITIALIZE,
    loggers: string[],
    cachedEvents: LogEntryData[],
}

export interface LogEventAction extends Action {
    type: Constants.LOG_LOGEVENT,
    event: LogEntryData,
}


export const logInitialize: ActionCreator<LogInitializeAction> = (loggers: string[], cachedEvents: LogEntryData[]) => ({
    type: Constants.LOG_INITIALIZE,
    loggers,
    cachedEvents,
});

export const logEvent: ActionCreator<LogEventAction> = (event: LogEntryData) => ({
    type: Constants.LOG_LOGEVENT,
    event
});

export const logSubscribe: ActionCreator<LogSubscribeAction> = (subscriber: any) => ({
    type: Constants.LOG_SUBSCRIBE,
    subscriber,
});
export const logUnsubscribe: ActionCreator<LogUnsubscribeAction> = (subscriber: any) => ({
    type: Constants.LOG_UNSUBSCRIBE,
    subscriber,
});


export const logFilterByServer: ActionCreator<LogFilterByServerAction> = (serverId: string) => ({
    type: Constants.LOG_FILTER_SERVER,
    serverId,
});

export const logFilterByLogger: ActionCreator<LogFilterByLoggerAction> = (logger: string) => ({
    type: Constants.LOG_FILTER_LOGGER,
    logger,
});


export const logFilterByCharacter: ActionCreator<LogFilterByCharacterAction> = (characterName: string) => ({
    type: Constants.LOG_FILTER_CHARACTER,
    characterName,
});


export const logSetAutoRefresh: ActionCreator<LogSetAutoRefreshAction> = (value: boolean) => ({
    type: Constants.LOG_SETAUTOREFRESH,
    value,
});
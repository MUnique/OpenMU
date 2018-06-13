import Redux from "redux";

export enum Constants {
    SYSTEM_UPDATE = "SYSTEM_UPDATE",
    SYSTEM_SUBSCRIBE = "SYSTEM_SUBSCRIBE",
    SYSTEM_UNSUBSCRIBE = "SYSTEM_UNSUBSCRIBE",
}


export interface SystemSubscribeAction extends Redux.Action {
    type: Constants.SYSTEM_SUBSCRIBE,
    subscriber: any,
}

export interface SystemUnsubscribeAction extends Redux.Action {
    type: Constants.SYSTEM_UNSUBSCRIBE,
    subscriber: any,
}

export interface SystemUpdateAction extends Redux.Action {
    type: Constants.SYSTEM_UPDATE,
    cpuPercentTotal: number,
    cpuPercentInstance: number,
    bytesSent: number,
    bytesReceived: number,
}


export const systemSubscribe: Redux.ActionCreator<SystemSubscribeAction> =
    (subscriber: any) => ({ type: Constants.SYSTEM_SUBSCRIBE, subscriber });

export const systemUnsubscribe: Redux.ActionCreator<SystemUnsubscribeAction> =
    (subscriber: any) => ({ type: Constants.SYSTEM_UNSUBSCRIBE, subscriber });

export const systemUpdate: Redux.ActionCreator<SystemUpdateAction> =
(cpuPercentTotal: number,
    cpuPercentInstance: number,
    bytesSent: number,
    bytesReceived: number,) => ({
    type: Constants.SYSTEM_UPDATE,
    cpuPercentTotal,
    cpuPercentInstance,
    bytesSent,
    bytesReceived,
});

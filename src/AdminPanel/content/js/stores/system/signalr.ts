import { Middleware, MiddlewareAPI, Action, Store, Dispatch } from "redux";

import { ApplicationState } from "../index";
import { SignalRConnector } from "../signalr";
import { Constants, SystemSubscribeAction, SystemUnsubscribeAction, systemUpdate, } from "./actions";

/*
declare var $: { connection: { systemHub: SystemHubProxy } };

interface SystemHubClient {
    update: (cpuPercentTotal: number, cpuPercentInstance: number, bytesSent: number, bytesReceived: number) => void;
}

interface SystemHubProxy extends HubProxy {
    client: SystemHubClient;
}

interface ServerHub {
}*/

export var systemHubConnector: SystemHubSignalRConnector;

class SystemHubSignalRConnector extends SignalRConnector {
    constructor(store: Store<ApplicationState>) {
        super(store);
    }

    protected getHubPath(): string {
        return "/signalr/hubs/systemHub";
    }

    protected onBeforeConnect(): void {
        this.connection.on("Update",
            (cpuPercentTotal: number, cpuPercentInstance: number, bytesSent: number, bytesReceived: number) =>
            this.store.dispatch(systemUpdate(cpuPercentTotal, cpuPercentInstance, bytesSent, bytesReceived)));
    }
}

export const systemHubConnectorMiddleware: Middleware =
    (api: MiddlewareAPI) =>
        (next: Dispatch) =>
        (action: Action) => {
            if (action.type === Constants.SYSTEM_SUBSCRIBE) {
                systemHubConnector.subscribe((action as SystemSubscribeAction).subscriber);
            } else if (action.type === Constants.SYSTEM_UNSUBSCRIBE) {
                systemHubConnector.unsubscribe((action as SystemUnsubscribeAction).subscriber);
            } else {
                return next(action);
            }
        };

export function configureSystemHubConnector(store: Store<ApplicationState>): void {
    systemHubConnector = new SystemHubSignalRConnector(store);
}
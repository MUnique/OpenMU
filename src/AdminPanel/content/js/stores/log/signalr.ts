import Redux, { MiddlewareAPI, Middleware, Dispatch, Action } from "redux";
import Store = Redux.Store;
import { LogEntryData } from "./types";
import { ApplicationState } from "../index";
import { SignalRConnector, HubProxy, HubServer } from "../signalr";
import { Constants, logInitialize, logEvent, LogSubscribeAction, LogUnsubscribeAction } from "./actions";


declare var $: { connection: { fooBarHub: LogHubProxy } };

interface LogEventArgs {
    Id: number;
    FormattedEvent: string;
    LoggingEvent: LogEntryData;
}

interface LogHubClient {
    initialize: (loggers: string[], cachedEvents: LogEventArgs[]) => void;
    onLoggedEvent: (formattedEvent: any, logEvent: LogEntryData) => void;
}

interface LogHubProxy extends HubProxy {
    client: LogHubClient;
    server: LogHubServer;
}

interface LogHubServer extends HubServer {
    subscribe(): void;
    subscribe(group: string, idOfLastReceivedEntry: number): void;
}

export var logTableConnector: LogTableSignalRConnector;

class LogTableSignalRConnector extends SignalRConnector<LogHubProxy> {

    constructor(store: Store<ApplicationState>) {
        super(store);
    }

    onFirstSubscription(): void {
        this.hub.server.subscribe('MyGroup', this.store.getState().logTableState.idOfLastReceivedEntry);
    }

    protected initializeHub(): LogHubProxy {
        var hubProxy = $.connection.fooBarHub;
        hubProxy.client.initialize = (loggers, cachedEvents) => this.initialize(loggers, cachedEvents);
        hubProxy.client.onLoggedEvent = (formattedEvent, loggedEvent) => this.onLoggedEvent(formattedEvent, loggedEvent);
        return hubProxy;
    }

    private initialize(loggers: string[], cachedEvents: LogEventArgs[]) {
        if (console && console.log) {
            console.log("log hub: init");
        }

        this.store.dispatch(logInitialize(loggers, cachedEvents.map(event => { return event.LoggingEvent })));
    }

    private onLoggedEvent(formattedEvent: any, loggedEvent: LogEntryData) {
        if (console && console.log) {
            console.log("onLoggedEvent", formattedEvent, loggedEvent);
        }

        this.store.dispatch(logEvent(loggedEvent));
    }
}

export const logTableConnectorMiddleware: Middleware =
    (api: MiddlewareAPI) =>
        (next: Dispatch) =>
        (action: Action) => {
            if (action.type === Constants.LOG_SUBSCRIBE) {
                logTableConnector.subscribe((action as LogSubscribeAction).subscriber);
            } else if (action.type === Constants.LOG_UNSUBSCRIBE) {
                logTableConnector.unsubscribe((action as LogUnsubscribeAction).subscriber);
            } else {
                next(action);
            }
        };

export function configureLogTableConnector(store: Store<ApplicationState>): void {
    logTableConnector = new LogTableSignalRConnector(store);
}
import Redux, { MiddlewareAPI, Middleware, Dispatch, Action } from "redux";
import Store = Redux.Store;
import { LogEventData, LogEventArgs } from "./types";
import { ApplicationState } from "../index";
import { SignalRConnector } from "../signalr";
import { Constants, logInitialize, logEvent, LogSubscribeAction, LogUnsubscribeAction } from "./actions";


export var logTableConnector: LogTableSignalRConnector;

class LogTableSignalRConnector extends SignalRConnector {
    
    constructor(store: Store<ApplicationState>) {
        super(store);
    }

    protected onBeforeConnect(): void {
        this.connection.on("Initialize", this.initialize.bind(this));
        this.connection.on("OnLoggedEvent", this.onLoggedEvent.bind(this));
    }

    protected onConnected(): void {
        this.connection.send("SubscribeToGroupWithMessageOffset", "MyGroup", this.store.getState().logTableState.idOfLastReceivedEntry);
    }

    protected onBeforeDisconnect(): void {
        this.connection.send("UnsubscribeFromGroup", "MyGroup");
    }

    protected getHubPath(): string {
        return "/signalr/hubs/logHub";
    }


    private initialize(loggers: string[], cachedEvents: LogEventArgs[]) {
        if (console && console.log) {
            console.log("log hub: init");
        }

        this.store.dispatch(logInitialize(loggers, cachedEvents.map(event => { return event.loggingEvent })));
    }

    private onLoggedEvent(formattedEvent: any, loggedEvent: LogEventData, id: number) {
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
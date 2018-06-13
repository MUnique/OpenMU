import { Store } from "redux";
import { ApplicationState } from "./index";

declare var $: { connection: { hub: HubConnection } };

export interface HubProxy {
    server: HubServer;
}

export interface HubServer {
    subscribe(): void;
    unsubscribe(): void;
}

interface HubConnection {
    start(onConnected: () => void): void;
    start(): void;
    stop(): void;
    state: number;
}

export abstract class SignalRConnector<THubProxy extends HubProxy> {
    static subscribers: any[] = [];
    static postponedOnFirstSubscription: (() => void)[] = [];
    hubSubscribers: number;
    protected store: Store<ApplicationState>;
    protected hub: THubProxy = null;

    constructor(store: Store<ApplicationState>) {
        this.store = store;
        this.hubSubscribers = 0;
        this.hub = this.initializeHub();
    }

    public subscribe(subscriber: any) {
        this.hubSubscribers++;
        SignalRConnector.subscribers.push(subscriber);
        if (SignalRConnector.subscribers.length === 1) {
            SignalRConnector.postponedOnFirstSubscription.push(() => this.onFirstSubscription());
            console.log("connection start");
            $.connection.hub.start(SignalRConnector.onConnected);
            // TODO: Handle errors, disconnects, reconnects etc. Extend FetchState accordingly
        }
        else if ($.connection.hub.state === 1) {
            console.log("connected, directly calling onFirstSubscription");
            this.onFirstSubscription();
        }
        else if (this.hubSubscribers === 1) {
            console.log("first subscriber of this specific hub type, but hub is already connecting. postponing the call to onFirstSubscription.");
            SignalRConnector.postponedOnFirstSubscription.push(() => this.onFirstSubscription());
        }
    }

    public unsubscribe(subscriber: any) {
        this.hubSubscribers--;
        if (this.hubSubscribers === 0) {
            console.log("calling onLastUnsubscription");
            this.onLastUnsubscription();
        }

        let index = SignalRConnector.subscribers.indexOf(subscriber);
        if (index >= 0) {
            SignalRConnector.subscribers.splice(index, 1);
            if (SignalRConnector.subscribers.length === 0) {
                console.log("connection stop");
                $.connection.hub.stop();
            }
        }
    }

    protected abstract initializeHub(): THubProxy;

    static onConnected(): void {
        console.log("connected to SignalR server");
        SignalRConnector.postponedOnFirstSubscription.forEach(callback => callback());
        SignalRConnector.postponedOnFirstSubscription = [];
    }

    protected onFirstSubscription(): void {
        this.hub.server.subscribe();
    }

    protected onLastUnsubscription(): void {
        this.hub.server.unsubscribe();
    }
}
import { Store } from "redux";
import { ApplicationState } from "./index";
import { HubConnection, HubConnectionBuilder, LogLevel } from "signalr";


export interface HubProxy {
    server: HubServer;
}

export interface HubServer {
    subscribe(): void;
    unsubscribe(): void;
}

export abstract class SignalRConnector {
    subscribers: any[] = [];
    postponedOnFirstSubscription: (() => void)[] = [];
    hubSubscribers: number;
    connection: HubConnection;
    connected: boolean;
    protected store: Store<ApplicationState>;

    constructor(store: Store<ApplicationState>) {
        this.store = store;
        this.hubSubscribers = 0;
    }

    public subscribe(subscriber: any) {
        this.hubSubscribers++;
        this.subscribers.push(subscriber);
        if (this.subscribers.length === 1) {
            this.postponedOnFirstSubscription.push(() => this.onFirstSubscription());
            let hubPath = this.getHubPath();
            console.info("connecting to " + hubPath);

            // TODO: Handle errors, disconnects, reconnects etc. Extend FetchState accordingly
            this.connection = new HubConnectionBuilder()
                .withUrl(hubPath)
                .configureLogging(LogLevel.Information)
                .build();
            this.connection.start()
                .then(() => this.onConnected())
                .catch(err => console.error(err.toString()));
        }
        else if (this.connected) {
            console.log("connected, directly calling onFirstSubscription");
            this.onFirstSubscription();
        }
        else if (this.hubSubscribers === 1) {
            console.log("first subscriber of this specific hub type, but hub is already connecting. postponing the call to onFirstSubscription.");
            this.postponedOnFirstSubscription.push(() => this.onFirstSubscription());
        }
    }

    public unsubscribe(subscriber: any) {
        this.hubSubscribers--;
        if (this.hubSubscribers === 0) {
            console.log("calling onLastUnsubscription");
            this.onLastUnsubscription();
        }

        let index = this.subscribers.indexOf(subscriber);
        if (index >= 0) {
            this.subscribers.splice(index, 1);
            if (this.subscribers.length === 0) {
                console.log("connection stop");
                this.connection.stop();
            }
        }
    }

    protected abstract getHubPath(): string;

    protected onFirstSubscription(): void {
        this.connection.send("Subscribe");
    }

    protected onLastUnsubscription(): void {
        this.connection.send("Unsubscribe");
    }

    private onConnected(): void {
        console.log("connected to SignalR server");
        this.connected = true;
        this.postponedOnFirstSubscription.forEach(callback => callback());
        this.postponedOnFirstSubscription = [];
    }
}
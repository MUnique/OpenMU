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
    connection: HubConnection;
    connected: boolean;
    protected store: Store<ApplicationState>;

    constructor(store: Store<ApplicationState>) {
        this.store = store;
    }

    public subscribe(subscriber: any) {
        this.subscribers.push(subscriber);
        if (this.subscribers.length === 1) {
            this.tryConnect();
        }
    }

    public unsubscribe(subscriber: any) {
        let index = this.subscribers.indexOf(subscriber);
        if (index >= 0) {
            this.subscribers.splice(index, 1);
            if (this.subscribers.length === 0) {
                console.log("calling onBeforeDisconnect");
                this.onBeforeDisconnect();

                console.log("connection stop");
                this.connection.stop();
            }
        }
    }

    protected abstract getHubPath(): string;

    protected onBeforeConnect(): void {
        // Register client methods in derived classes here
        // e.g. this.connection.on("Initialize", ...)
    }

    protected onConnected(): void {
        this.connection.send("Subscribe");
    }

    protected onBeforeDisconnect(): void {
        this.connection.send("Unsubscribe");
    }

    private onConnectedInternal(): void {
        console.log("connected to SignalR server");
        this.connected = true;
        this.onConnected();
    }

    private tryConnect(): void {
        let hubPath = this.getHubPath();
        if (this.subscribers.length === 0) {
            console.info(hubPath + ": cancelling tryConnect, no more subscribers");
            return;
        }
        
        console.info(hubPath + ": tryConnect");

        // TODO: Extend FetchState accordingly
        this.connection = new HubConnectionBuilder()
            .withUrl(hubPath)
            .configureLogging(LogLevel.Information)
            .build();
        this.connection.onclose(error => {
            console.error(hubPath + ": connection closed, error: " + error);
            if (this.subscribers.length > 0) {
                console.info(hubPath + ": reconnecting in 10 seconds...");
                setTimeout(() => this.tryConnect(), 10000);
            }
        });
        this.onBeforeConnect();
        this.connection.start()
            .then(() => this.onConnectedInternal())
            .catch(error => {
                console.error(hubPath + ": connection start failed, error: " + error.toString());
                if (this.subscribers.length > 0) {
                    console.info(hubPath + ": trying again in 10 seconds...");
                    setTimeout(() => this.tryConnect(), 10000);
                }
            });
    }
}
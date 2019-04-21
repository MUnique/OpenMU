import { Map } from "../map/types";

export interface Server {
    readonly id: number;
    readonly state: ServerState;
    readonly type: ServerType;
    readonly description: string;
    readonly onlinePlayerCount: number;
    readonly maximumPlayers: number;
    readonly maps: Map[];
}

export interface ConnectServer extends Server {
    readonly settings: ConnectServerSettings;
}

export enum ServerState {
    Stopped = 0,
    Starting = 1,
    Started = 2,
    Stopping = 3,
};

export namespace ServerState {

    export function getCaption(state: ServerState) {
        switch (state) {
            case ServerState.Stopped:
            return "Stopped";
            case ServerState.Starting:
            return "Starting ...";
            case ServerState.Started:
            return "Started";
            case ServerState.Stopping:
            return "Stopping ...";
        }

        return "";
    }
}

export enum ServerType {
    Undefined = 0,
    GameServer = 1,
    ConnectServer = 2,
    ChatServer = 3,
}

export interface ConnectServerSettings {
    readonly id: number;
    readonly description: string;
    readonly disconnectOnUnknownPacket: boolean;
    readonly maximumReceiveSize: number;
    readonly clientListenerPort: number;
    readonly timeOutSeconds: number;
    readonly currentPatchVersion: Uint8Array;
    readonly patchAddress: string;
    readonly maxConnectionsPerAddress: number;
    readonly checkMaxConnectionsPerAddress: boolean;
    readonly maxConnections: number;
    readonly listenerBacklog: number;
    readonly maxFtpRequests: number;
    readonly maxIpRequests: number;
    readonly maxServerListRequests: number;
}
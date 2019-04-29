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
    readonly id: any;
    readonly description: string;
    readonly gameClient: GameClientDefinition;
    readonly disconnectOnUnknownPacket: boolean;
    readonly maximumReceiveSize: number;
    readonly clientListenerPort: number;
    readonly timeout: string;
    readonly currentPatchVersion: string;
    readonly patchAddress: string;
    readonly maxConnectionsPerAddress: number;
    readonly checkMaxConnectionsPerAddress: boolean;
    readonly maxConnections: number;
    readonly listenerBacklog: number;
    readonly maxFtpRequests: number;
    readonly maxIpRequests: number;
    readonly maxServerListRequests: number;
}


export namespace ConnectServerSettings {

    export function timeoutSeconds(settings: ConnectServerSettings) : number {
        return parseInt(settings.timeout.split(':')[0]) * 3600 + parseInt(settings.timeout.split(':')[1]) * 60 + parseInt(settings.timeout.split(':')[2]);
    }

    export function toTimeSpanString(value: number): string {
        let hours: number = Math.trunc((value / 3600) % 60);
        let minutes: number = Math.trunc((value / 60) % 60);
        let seconds: number = Math.trunc(value % 60);
        return String(hours).padStart(2, '0') + ':' + String(minutes).padStart(2, '0') + ':' + String(seconds).padStart(2, '0');
    }

    export function createNew(client: GameClientDefinition) : ConnectServerSettings {
        return {
            id: '00000000-0000-0000-0000-000000000000',
            gameClient: client,
            maxFtpRequests: 1,
            maxIpRequests: 3,
            maxServerListRequests: 30,
            maximumReceiveSize: 6,
            clientListenerPort: 44405,
            description: "Connect Server (new)",
            listenerBacklog: 100,
            checkMaxConnectionsPerAddress: true,
            maxConnectionsPerAddress: 20,
            maxConnections: 10000,
            timeout: "00:01:00",
            patchAddress: "patch.yourserver.com",
            disconnectOnUnknownPacket: true,
            currentPatchVersion: btoa('\u0001\u0003\u0000'),
        };
    }
}

export interface GameClientDefinition {
    readonly id: any;
    readonly description: string;
    readonly season: number;
    readonly episode: number;
    readonly language: ClientLanguage;
    readonly version: Uint8Array;
    readonly serial: Uint8Array;
}

export enum ClientLanguage {
    Invariant = 0,
    English = 1,
    Japanese = 2,
    Vietnamese = 3,
    Filipino = 4,
    Chinese = 5,
    Korean = 6,
    Thai = 7
};

export namespace ClientLanguage {

    export function getCaption(state: ClientLanguage) {
        switch (state) {
            case ClientLanguage.Invariant:
                return "Invariant";
            case ClientLanguage.English:
                return "English";
            case ClientLanguage.Japanese:
                return "Japanese";
            case ClientLanguage.Vietnamese:
                return "Vietnamese";
            case ClientLanguage.Filipino:
                return "Filipino";
            case ClientLanguage.Chinese:
                return "Chinese";
            case ClientLanguage.Korean:
                return "Korean";
            case ClientLanguage.Thai:
                return "Thai";
        }

        return "";
    }
}
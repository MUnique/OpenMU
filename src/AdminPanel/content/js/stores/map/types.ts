
export interface Map {
    readonly id: number;
    readonly name: string;
    readonly playerCount: number;
    readonly serverId: number;
    readonly players: ObjectData[];
}

export interface ObjectData {
    readonly id: number;
    readonly name: string;
    readonly serverId: number;
    readonly mapId: number;
    
    readonly x: number;
    readonly y: number;
    readonly rotation: number;
}

export interface PlayerData extends ObjectData {
    readonly isHighlighted: boolean;
}

export interface NpcData extends ObjectData {
    readonly isMonster: boolean;
}

export interface Step {
    readonly x: number;
    readonly y: number;
    readonly direction: number;
}
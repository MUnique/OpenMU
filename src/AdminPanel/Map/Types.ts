
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
    readonly mapId: number;

    readonly x: number;
    readonly y: number;
    readonly direction: Direction;
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
    readonly direction: Direction;
}

/*
 * Direction where an object is looking at or heading to.
 *
 * The directions are named/valued after how they look like due the original game client.
 * That means, when a character looks to 'south', it looks straight downwards.
 * Side note: The map is rotated by 45 degree on the game client, but not on the live map.
 */
export enum Direction {
    Undefined = 0,
    West = 1,
    SouthWest = 2,
    South = 3,
    SouthEast = 4,
    East = 5,
    NorthEast = 6,
    North = 7,
    NorthWest = 8,
}

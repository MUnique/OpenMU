
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

export interface ObjectData {
    readonly id: number;
    readonly name: string;
    readonly mapId: number;

    readonly x: number;
    readonly y: number;
    readonly direction: Direction;

    readonly level?: number;
    readonly masterLevel?: number;
}

export interface PlayerData extends ObjectData {
    readonly isHighlighted: boolean;
    readonly level: number;
    readonly masterLevel: number;
}

export interface NpcData extends ObjectData {
    readonly npcObjectKind: number;
}

export interface Step {
    readonly x: number;
    readonly y: number;
    readonly direction: Direction;
}

/*
 * How the monster filter of the live map compares a name with its text
 * (matching the C# enum MonsterFilterMode).
 */
export const enum MonsterFilterMode {
    Contains = 0,
    StartsWith = 1,
    EndsWith = 2,
}

/*
 * A monster which matches the monster filter, as it's listed next to the map.
 */
export interface MonsterFilterMatch {
    readonly id: number;
    readonly name: string;
    readonly x: number;
    readonly y: number;
}

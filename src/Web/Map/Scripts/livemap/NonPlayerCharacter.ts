import * as THREE from "three";
import { Attackable, attackableAlphaMapTexture } from "./Attackable";
import { NpcData } from "./Types";

/*
 * NpcObjectKind enum (matching the C# enum values).
 */
enum NpcObjectKind {
    Monster = 0,
    PassiveNpc = 1,
    Guard = 2,
    Trap = 3,
    Gate = 4,
    Statue = 5,
    SoccerBall = 6,
    Destructible = 7,
}

function getColorForObjectKind(kind: NpcObjectKind): number {
    switch (kind) {
        case NpcObjectKind.Monster:
            return 0x00FFFF;
        case NpcObjectKind.PassiveNpc:
        case NpcObjectKind.Guard:
            return 0xFF8800;
        default:
            return 0xFFFF00;
    }
}

export class NonPlayerCharacter extends Attackable<NpcData> {
    private static readonly size: number = 4;
    private static defaultGeometry: THREE.Geometry = new THREE.BoxGeometry(
        NonPlayerCharacter.size,
        NonPlayerCharacter.size,
        NonPlayerCharacter.size);
    constructor(data: NpcData) {
        const color = getColorForObjectKind(data.npcObjectKind);
        super(
            data,
            NonPlayerCharacter.defaultGeometry,
            new THREE.MeshBasicMaterial({ color, alphaMap: attackableAlphaMapTexture, transparent: true }),
        );
    }
};

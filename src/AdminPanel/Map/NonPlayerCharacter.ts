import * as THREE from "three";
import { Attackable, attackableAlphaMapTexture } from "./Attackable";
import { NpcData } from "./Types";

export class NonPlayerCharacter extends Attackable<NpcData> {
    private static readonly size : number = 4;
    private static defaultGeometry: THREE.Geometry = new THREE.BoxGeometry(
        NonPlayerCharacter.size,
        NonPlayerCharacter.size,
        NonPlayerCharacter.size);
    constructor(data: NpcData) {
        super(
            data,
            NonPlayerCharacter.defaultGeometry,
            new THREE.MeshBasicMaterial({ color: 0x00FFFF, alphaMap: attackableAlphaMapTexture, transparent: true }),
        );
    }
};

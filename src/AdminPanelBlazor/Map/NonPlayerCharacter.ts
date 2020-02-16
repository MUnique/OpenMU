import * as THREE from "three";
import { Attackable, attackableAlphaMapTexture } from "./Attackable";
import { NpcData } from "./Types";

export class NonPlayerCharacter extends Attackable<NpcData> {
    static defaultGeometry: THREE.Geometry = new THREE.BoxGeometry(4, 4, 4);
    constructor(data: NpcData) {
        super(
            data,
            NonPlayerCharacter.defaultGeometry,
            new THREE.MeshBasicMaterial({ color: 0x00FFFF, alphaMap: attackableAlphaMapTexture, transparent: true })
        );
    }
};
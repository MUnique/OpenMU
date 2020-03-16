import * as THREE from "three";
import { Attackable, attackableAlphaMapTexture } from "./Attackable";
import { PlayerData } from "./Types";

export class Player extends Attackable<PlayerData> {
    static defaultGeometry: THREE.Geometry = new THREE.BoxGeometry(4, 4, 4);
    constructor(data: PlayerData) {
        super(
            data,
            Player.defaultGeometry,
            new THREE.MeshBasicMaterial({ color: 0xFF0000 + data.id, alphaMap: attackableAlphaMapTexture, transparent: true })
        );
    }

    public update() {
        if (this.data.isHighlighted) {
            this.scale.setScalar(1.5);
        } else {
            this.scale.setScalar(1.0);
        }
    }
};
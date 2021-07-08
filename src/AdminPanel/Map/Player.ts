import * as THREE from "three";
import { Attackable, attackableAlphaMapTexture } from "./Attackable";
import { PlayerData } from "./Types";

export class Player extends Attackable<PlayerData> {
    private static readonly size = 4;
    private static defaultGeometry: THREE.Geometry = new THREE.BoxGeometry(Player.size, Player.size, Player.size);
    constructor(data: PlayerData) {
        super(
            data,
            Player.defaultGeometry,
            new THREE.MeshBasicMaterial({
                color: 0xFF0000 + data.id,
                alphaMap: attackableAlphaMapTexture,
                transparent: true
            })
        );
    }

    public update(): void {
        if (this.data.isHighlighted) {
            this.scale.setScalar(1.5);
        } else {
            this.scale.setScalar(1.0);
        }
    }
};

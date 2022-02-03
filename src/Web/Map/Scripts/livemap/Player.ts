import * as THREE from "three";
import { Attackable, attackableAlphaMapTexture } from "./Attackable";
import { PlayerData } from "./Types";

export class Player extends Attackable<PlayerData> {
    private static readonly size: number = 4;
    private static defaultGeometry: THREE.Geometry = new THREE.BoxGeometry(Player.size, Player.size, Player.size);
    constructor(data: PlayerData) {
        super(
            data,
            Player.defaultGeometry,
            new THREE.MeshBasicMaterial({
                alphaMap: attackableAlphaMapTexture,
                color: 0xFF0000 + data.id,
                transparent: true,
            }),
        );
    }

    public update(): void {
        const highlightedScale = 1.5;
        const normalScale = 1.0;

        if (this.data.isHighlighted) {
            
            this.scale.setScalar(highlightedScale);
        } else {
            this.scale.setScalar(normalScale);
        }
    }
};

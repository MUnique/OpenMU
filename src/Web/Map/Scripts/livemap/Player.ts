import * as THREE from "three";
import { Attackable, attackableAlphaMapTexture } from "./Attackable";
import { PlayerData } from "./Types";

export class Player extends Attackable<PlayerData> {
    private static readonly size: number = 4;
    private static readonly defaultColor: number = 0xBE88E0;
    private static readonly highlightedColor: number = 0xFFFFFF;
    private static defaultGeometry: THREE.Geometry = new THREE.BoxGeometry(Player.size, Player.size, Player.size);
    constructor(data: PlayerData) {
        super(
            data,
            Player.defaultGeometry,
            new THREE.MeshBasicMaterial({
                alphaMap: attackableAlphaMapTexture,
                color: Player.defaultColor,
                transparent: true,
            }),
        );
        this.renderOrder = 1;
    }

    public update(): void {
        const highlightedScale = 1.5;
        const normalScale = 1.0;

        if (this.data.isHighlighted) {
            this.scale.setScalar(highlightedScale);
            (this.material as THREE.MeshBasicMaterial).color.setHex(Player.highlightedColor);
        } else {
            this.scale.setScalar(normalScale);
            (this.material as THREE.MeshBasicMaterial).color.setHex(Player.defaultColor);
        }
    }
};

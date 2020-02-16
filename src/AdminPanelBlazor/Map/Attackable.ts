import * as THREE from "three";
import TWEEN from "tween";
import { ObjectData, Step, Direction } from "./Types";

export interface GameObject extends THREE.Object3D {
    data: ObjectData;
    respawn(newData: any): void;
    moveTo(newX: number, newY: number, moveType: any, walkDelay: number, steps: Step[]): void;
    rotateTo(rotation: Direction): void;
    gotKilled(): void;
}

export class Attackable<TData extends ObjectData> extends THREE.Mesh implements GameObject {
    data: TData;
    moveTween: TWEEN.Tween;
    material: THREE.Material;
    constructor(data: TData, geometry: THREE.Geometry, material: THREE.Material) {
        super(geometry, material);
        this.data = data;
        this.moveTween = null;
    }

    gotKilled() {
        // we fade the color out
        let state = { opacity: 1 };
        let tween = new TWEEN.Tween(state)
            .to({ opacity: 0.1 }, 1000)
            .onUpdate(() => this.material.opacity = state.opacity)
            .easing(TWEEN.Easing.Circular.Out)
            .start();
    }

    respawn(newData: any) {
        this.data = newData;
        this.material.opacity = 1.0;
        let state = { scale: 0 };
        let tween = new TWEEN.Tween(state)
            .to({ scale: 1 }, 500)
            .onUpdate(() => this.scale.setScalar(state.scale))
            .easing(TWEEN.Easing.Back.Out)
            .start();
        this.setObjectPositionOnMap(this.data.x, this.data.y);
        this.setRotation(this.data.direction);
    }

    moveTo(newX: number, newY: number, moveType: any, walkDelay: number, steps: Step[]) {
        let state = { x: this.data.x, y: this.data.y };
        this.data = this.data = Object.assign({}, this.data, { x: newX, y: newY });

        if (this.moveTween !== null) {
            this.moveTween.stop();
        }

        this.moveTween = new TWEEN.Tween(state)
            .onUpdate(() => this.setObjectPositionOnMap(state.x, state.y));

        if (moveType === "Instant" || moveType === 1) {
            this.moveTween = this.moveTween.easing(TWEEN.Easing.Elastic.Out)
                .to({ x: newX, y: newY }, 300);
        } else {
            for (let i in steps) {
                if (steps.hasOwnProperty(i)) {
                    let step = steps[i];
                    let stepTween = new TWEEN.Tween(state)
                        .to({ x: step.x, y: step.y }, walkDelay)
                        .onStart(() => this.rotateTo(step.direction))
                        .onUpdate(() => this.setObjectPositionOnMap(state.x, state.y));
                    this.moveTween.chain(stepTween);
                }
            }
        }

        this.moveTween.start();
    }

    rotateTo(rotation: Direction) {
        if (this.data !== undefined) {
            this.data = Object.assign({}, this.data, rotation);
        }

        // TODO: Tween it :)

        this.setRotation(rotation);
    }

    private setRotation(value: Direction) {
        this.rotation.z = THREE.Math.degToRad((value * 360) / 8);
    }

    setObjectPositionOnMap(newX: number, newY: number) {
        const offset = 128;

        this.position.y = offset - newX;
        this.position.x = newY - offset;
    }
};

export var attackableAlphaMapTexture: THREE.Texture;
new THREE.TextureLoader().load('/img/attackable_alphamap.png', (t: THREE.Texture) => {
    attackableAlphaMapTexture = t;
});


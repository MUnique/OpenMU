import * as THREE from "three";
import TWEEN from "tween";
import { ObjectData, Step, Direction } from "./Types";
import { GameObject } from "./GameObject";
import { NameLabel } from "./NameLabel";

const NAME_LABEL_Z_POSITION = 200;
const NAME_INDEX = 0;

export class Attackable<TData extends ObjectData> extends THREE.Mesh implements GameObject {
    public data: TData;
    public material: THREE.Material;
    public readonly nameLabel: NameLabel;

    private moveTween: TWEEN.Tween;

    constructor(data: TData, geometry: THREE.Geometry, material: THREE.Material) {
        super(geometry, material);
        this.data = data;
        this.moveTween = null;
        this.nameLabel = new NameLabel();
        this.nameLabel.position.z = NAME_LABEL_Z_POSITION;
        this.add(this.nameLabel);
    }

    public showLabel(): void {
        this.nameLabel.show(this.data.name.split(" - Id:")[NAME_INDEX]);
    }

    public hideLabel(): void {
        this.nameLabel.hide();
    }

    public gotKilled(): void {
        // we fade the color out
        const fadeOutDurationMs = 1000;
        const startingOpacity = 1;
        const fadedOutOpacity = 0.1;
        
        const state = { opacity: startingOpacity };
        const tween = new TWEEN.Tween(state)
            .to({ opacity: fadedOutOpacity }, fadeOutDurationMs)
            .onUpdate(() => this.material.opacity = state.opacity)
            .easing(TWEEN.Easing.Circular.Out)
            .start();
    }

    public respawn(newData: TData): void {
        const scaleUpDurationMs = 500;
        this.data = newData;
        this.material.opacity = 1.0;
        const state = { scale: 0 };
        const tween = new TWEEN.Tween(state)
            .to({ scale: 1 }, scaleUpDurationMs)
            .onUpdate(() => this.scale.setScalar(state.scale))
            .easing(TWEEN.Easing.Back.Out)
            .start();
        this.setObjectPositionOnMap(this.data.x, this.data.y);
        this.setRotation(this.data.direction);
    }

    public moveTo(newX: number, newY: number, moveType: any, walkDelay: number, steps: Step[]): void {
        const state = { x: this.data.x, y: this.data.y };
        this.data = this.data = Object.assign({}, this.data, { x: newX, y: newY });

        if (this.moveTween !== null) {
            this.moveTween.stop();
        }

        this.moveTween = new TWEEN.Tween(state)
            .onUpdate(() => this.setObjectPositionOnMap(state.x, state.y));

        if (moveType === "Instant" || moveType === 1) {
            const moveDurationMs = 300;
            this.moveTween = this.moveTween.easing(TWEEN.Easing.Elastic.Out)
                .to({ x: newX, y: newY }, moveDurationMs);
        } else {
            for (const i in steps) {
                if (steps.hasOwnProperty(i)) {
                    const step = steps[i];
                    const stepTween = new TWEEN.Tween(state)
                        .to({ x: step.x, y: step.y }, walkDelay)
                        .onStart(() => this.rotateTo(step.direction))
                        .onUpdate(() => this.setObjectPositionOnMap(state.x, state.y));
                    this.moveTween.chain(stepTween);
                }
            }
        }

        this.moveTween.start();
    }

    public rotateTo(rotation: Direction): void {
        if (this.data !== undefined) {
            this.data = Object.assign({}, this.data, rotation);
        }

        const degreesOfOneTurn = 360;
        const numberOfDirectionValues = 8;
        const targetAngle = THREE.Math.degToRad((rotation * degreesOfOneTurn) / numberOfDirectionValues);
        const rotateDurationMs = 200;

        const state = { z: this.rotation.z };
        new TWEEN.Tween(state)
            .to({ z: targetAngle }, rotateDurationMs)
            .onUpdate(() => this.rotation.z = state.z)
            .easing(TWEEN.Easing.Quadratic.Out)
            .start();
    }

    private setRotation(value: Direction): void {
        const degreesOfOneTurn = 360;
        const numberOfDirectionValues = 8;
        this.rotation.z = THREE.Math.degToRad((value * degreesOfOneTurn) / numberOfDirectionValues);
    }

    private setObjectPositionOnMap(newX: number, newY: number): void {
        const offset = 128;

        this.position.y = offset - newX;
        this.position.x = newY - offset;
    }
}

export var attackableAlphaMapTexture: THREE.Texture;
new THREE.TextureLoader().load("_content/MUnique.OpenMU.Web.Map/img/attackable_alphamap.png", (t: THREE.Texture) => {
    attackableAlphaMapTexture = t;
});


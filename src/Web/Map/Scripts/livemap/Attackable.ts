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

    /*
     * All tweens of the current movement - the tween of the first step and the
     * step tweens which are chained to it. They are all kept, because stopping
     * the first tween doesn't stop the chained ones anymore as soon as it finished.
     */
    private moveTweens: TWEEN.Tween[] = [];
    private rotateTween: TWEEN.Tween | null = null;
    private scaleTween: TWEEN.Tween | null = null;
    private fadeTween: TWEEN.Tween | null = null;

    constructor(data: TData, geometry: THREE.Geometry, material: THREE.Material) {
        super(geometry, material);
        this.data = data;
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

    /**
     * Stops all running animations and releases the resources of this object.
     * The geometry is not disposed, because it's shared between all objects of the same type.
     */
    public dispose(): void {
        this.stopTweens();
        this.remove(this.nameLabel);
        this.nameLabel.dispose();
        this.material.dispose();
    }

    public gotKilled(): void {
        // we fade the color out
        const fadeOutDurationMs = 1000;
        const startingOpacity = 1;
        const fadedOutOpacity = 0.1;

        this.fadeTween?.stop();
        const state = { opacity: startingOpacity };
        this.fadeTween = new TWEEN.Tween(state)
            .to({ opacity: fadedOutOpacity }, fadeOutDurationMs)
            .onUpdate(() => this.material.opacity = state.opacity)
            .easing(TWEEN.Easing.Circular.Out)
            .start();
    }

    public respawn(newData: TData): void {
        const scaleUpDurationMs = 500;
        this.data = newData;
        this.material.opacity = 1.0;

        this.fadeTween?.stop();
        this.fadeTween = null;
        this.scaleTween?.stop();
        const state = { scale: 0 };
        this.scaleTween = new TWEEN.Tween(state)
            .to({ scale: 1 }, scaleUpDurationMs)
            .onUpdate(() => this.scale.setScalar(state.scale))
            .easing(TWEEN.Easing.Back.Out)
            .start();
        this.setObjectPositionOnMap(this.data.x, this.data.y);
        this.setRotation(this.data.direction);
    }

    public moveTo(newX: number, newY: number, moveType: any, walkDelay: number, steps: Step[]): void {
        const state = { x: this.data.x, y: this.data.y };
        this.data = Object.assign({}, this.data, { x: newX, y: newY });

        this.stopMoveTweens();

        const isWalking = moveType !== "Instant" && moveType !== 1
            && steps !== undefined && steps !== null && steps.length > 0;
        if (!isWalking) {
            const moveDurationMs = 300;
            const moveTween = new TWEEN.Tween(state)
                .to({ x: newX, y: newY }, moveDurationMs)
                .onUpdate(() => this.setObjectPositionOnMap(state.x, state.y))
                .easing(TWEEN.Easing.Elastic.Out);
            this.moveTweens.push(moveTween);
            moveTween.start();
            return;
        }

        // Each step tween is chained to its predecessor, so that the steps are walked one after another.
        let previousTween: TWEEN.Tween | null = null;
        for (const step of steps) {
            const stepTween = new TWEEN.Tween(state)
                .to({ x: step.x, y: step.y }, walkDelay)
                .onStart(() => this.rotateTo(step.direction))
                .onUpdate(() => this.setObjectPositionOnMap(state.x, state.y));
            previousTween?.chain(stepTween);
            previousTween = stepTween;
            this.moveTweens.push(stepTween);
        }

        this.moveTweens[0].start();
    }

    public rotateTo(rotation: Direction): void {
        if (this.data !== undefined) {
            this.data = Object.assign({}, this.data, { direction: rotation });
        }

        const degreesOfOneTurn = 360;
        const numberOfDirectionValues = 8;
        const targetAngle = THREE.Math.degToRad((rotation * degreesOfOneTurn) / numberOfDirectionValues);
        const rotateDurationMs = 200;

        this.rotateTween?.stop();
        const state = { z: this.rotation.z };
        this.rotateTween = new TWEEN.Tween(state)
            .to({ z: targetAngle }, rotateDurationMs)
            .onUpdate(() => this.rotation.z = state.z)
            .easing(TWEEN.Easing.Quadratic.Out)
            .start();
    }

    private stopTweens(): void {
        this.stopMoveTweens();

        this.rotateTween?.stop();
        this.rotateTween = null;
        this.scaleTween?.stop();
        this.scaleTween = null;
        this.fadeTween?.stop();
        this.fadeTween = null;
    }

    private stopMoveTweens(): void {
        for (const moveTween of this.moveTweens) {
            moveTween.stop();
        }

        this.moveTweens = [];
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


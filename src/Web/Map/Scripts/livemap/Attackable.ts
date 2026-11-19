import * as THREE from "three";
import TWEEN from "tween";
import { ObjectData, Step, Direction } from "./Types";
import { GameObject } from "./GameObject";
import { NameLabel } from "./NameLabel";

const NAME_LABEL_Z_POSITION = 200;
const NAME_INDEX = 0;
const NAME_SEPARATOR = " - Id:";

/**
 * The offset between the tile coordinates of the server and the coordinates of the scene.
 */
const MAP_OFFSET = 128;

/**
 * A position on the map, in tile coordinates.
 */
interface MapPosition {
    x: number;
    y: number;
}

/**
 * Gets the name of an object as it's shown: the server appends the id and the position to it.
 * @param data - The data of the object.
 * @returns The name without the appended id and position.
 */
export const getDisplayName = (data: ObjectData): string => data.name.split(NAME_SEPARATOR)[NAME_INDEX];

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
        this.nameLabel.show(getDisplayName(this.data));
    }

    public hideLabel(): void {
        this.nameLabel.hide();
    }

    /**
     * Keeps the labels of this object at the same size on the screen when the map is zoomed.
     * @param zoom - The zoom factor of the camera.
     */
    public setZoom(zoom: number): void {
        this.nameLabel.setZoom(zoom);
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
        this.stopMoveTweens();

        // The animation has to start where the icon currently is, not at the target of the previous move
        // (which is what this.data holds). Otherwise, a new walk request which arrives before the previous
        // walk has finished makes the icon jump ahead and slide back again.
        const state = this.getCurrentMapPosition();
        this.data = Object.assign({}, this.data, { x: newX, y: newY });

        const isWalking = moveType !== "Instant" && moveType !== 1
            && steps !== undefined && steps !== null && steps.length > 0;
        if (isWalking) {
            this.walkAlong(state, steps, walkDelay);
        } else {
            this.slideTo(state, newX, newY);
        }
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

    private slideTo(state: MapPosition, newX: number, newY: number): void {
        const moveDurationMs = 300;
        const moveTween = new TWEEN.Tween(state)
            .to({ x: newX, y: newY }, moveDurationMs)
            .onUpdate(() => this.setObjectPositionOnMap(state.x, state.y))
            .easing(TWEEN.Easing.Quadratic.Out);
        this.moveTweens.push(moveTween);
        moveTween.start();
    }

    private walkAlong(state: MapPosition, steps: Step[], walkDelay: number): void {
        const maxSlideDistance = 2;
        const firstStep = steps[0];
        if (Math.max(Math.abs(state.x - firstStep.x), Math.abs(state.y - firstStep.y)) > maxSlideDistance) {
            // Too far away from the new path (e.g. after a correction): snap instead of sliding across the map.
            state.x = firstStep.x;
            state.y = firstStep.y;
            this.setObjectPositionOnMap(state.x, state.y);
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

    private setRotation(value: Direction): void {
        const degreesOfOneTurn = 360;
        const numberOfDirectionValues = 8;
        this.rotation.z = THREE.Math.degToRad((value * degreesOfOneTurn) / numberOfDirectionValues);
    }

    /**
     * Gets the tile position at which the icon is currently drawn, which is the inverse of
     * {@link Attackable.setObjectPositionOnMap}. It differs from the position in the data while a move is animated.
     * The icon is always positioned before it's added to the world (see respawn).
     */
    private getCurrentMapPosition(): MapPosition {
        return { x: MAP_OFFSET - this.position.y, y: this.position.x + MAP_OFFSET };
    }

    private setObjectPositionOnMap(newX: number, newY: number): void {
        this.position.y = MAP_OFFSET - newX;
        this.position.x = newY - MAP_OFFSET;
    }
}

export var attackableAlphaMapTexture: THREE.Texture;

/**
 * The alpha threshold of the icons of players and NPCs. The icons are boxes with a circular alpha map,
 * so the corners of their squares are fully transparent. Without a threshold, these transparent pixels still
 * write the depth buffer, and hide everything which is drawn behind them afterwards - e.g. a player icon
 * next to the (bigger) icon of the selected player. With it, only the visible circle hides other icons.
 */
export const attackableAlphaTest: number = 0.05;
new THREE.TextureLoader().load("_content/MUnique.OpenMU.Web.Map/img/attackable_alphamap.png", (t: THREE.Texture) => {
    attackableAlphaMapTexture = t;
});


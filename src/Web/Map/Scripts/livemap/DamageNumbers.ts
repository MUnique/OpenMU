import * as THREE from "three";
import { DamageNumber } from "./DamageNumber";

const EMPTY_LENGTH = 0;
const RANDOM_OFFSET = 6;
const RANDOM_OFFSET_CENTER = 0.5;

/*
 * A pool of damage numbers which float above the objects of the map. The numbers are
 * reused, because creating a canvas and a texture per hit would be far too expensive.
 */
export class DamageNumbers extends THREE.Object3D {
    private static readonly poolSize: number = 64;

    private readonly numbers: DamageNumber[] = [];
    private nextIndex: number = 0;
    private zoom: number = 1;

    constructor() {
        super();
        for (let i = 0; i < DamageNumbers.poolSize; i++) {
            const number = new DamageNumber();
            this.numbers.push(number);
            this.add(number);
        }
    }

    /**
     * Keeps the numbers at the same size on the screen when the map is zoomed.
     * @param zoom - The zoom factor of the camera.
     */
    public setZoom(zoom: number): void {
        this.zoom = zoom;
        for (const number of this.numbers) {
            number.setZoom(zoom);
        }
    }

    /**
     * Shows a damage number above the specified position of the map.
     * @param x - The x coordinate in scene units.
     * @param y - The y coordinate in scene units.
     * @param damage - The damage to show.
     * @param color - The color of the text.
     */
    public show(x: number, y: number, damage: number, color: string): void {
        const number = this.numbers[this.nextIndex];
        this.nextIndex = (this.nextIndex + 1) % this.numbers.length;

        // Slightly spread out numbers of hits which happen in quick succession, so they don't
        // cover each other completely.
        const offset = (Math.random() - RANDOM_OFFSET_CENTER) * RANDOM_OFFSET / this.zoom;
        number.show(x + offset, y, damage.toString(), color, this.zoom);
    }

    /**
     * Releases the resources of all pooled numbers.
     */
    public dispose(): void {
        for (const number of this.numbers) {
            this.remove(number);
            number.dispose();
        }

        this.numbers.length = EMPTY_LENGTH;
    }
}

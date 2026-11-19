import * as THREE from "three";
import TWEEN from "tween";

const CANVAS_HEIGHT = 64;
const DAMAGE_SCALE_Y = 8;
const DAMAGE_Z_POSITION = 300;
const EMPTY_CANVAS_SIZE = 0;
const FONT = "bold 40px Consolas, monospace";
const LIFETIME_MS = 900;
const MIN_CANVAS_WIDTH = 64;
const OUTLINE_WIDTH = 6;
const PADDING = 12;
const RISE_DISTANCE = 10;

export class DamageNumber extends THREE.Sprite {
    /*
     * A shared material for numbers which were never shown. It's never rendered (the number is
     * invisible until 'show' is called) and therefore never allocates any resources in the renderer.
     */
    private static readonly hiddenMaterial: THREE.SpriteMaterial = new THREE.SpriteMaterial();

    private canvas: HTMLCanvasElement | null = null;
    private context: CanvasRenderingContext2D | null = null;
    private texture: THREE.CanvasTexture | null = null;
    private labelMaterial: THREE.SpriteMaterial | null = null;
    private zoom: number = 1;
    private unscaledWidth: number = 0;
    private unscaledHeight: number = 0;
    private tween: TWEEN.Tween | null = null;

    constructor() {
        super(DamageNumber.hiddenMaterial);
        this.position.z = DAMAGE_Z_POSITION;
        this.renderOrder = 3;
        this.visible = false;
    }

    public raycast(): void {
        // No-op: damage numbers should not intercept raycasts.
    }

    /**
     * Keeps the number at the same size on the screen when the map is zoomed.
     * @param zoom - The zoom factor of the camera.
     */
    public setZoom(zoom: number): void {
        this.zoom = zoom;
        this.applyScale();
    }

    /**
     * Shows the number at the specified position and lets it float upwards while fading out.
     * @param x - The x coordinate in scene units.
     * @param y - The y coordinate in scene units.
     * @param text - The text to show.
     * @param color - The color of the text.
     * @param zoom - The current zoom factor of the camera.
     */
    public show(x: number, y: number, text: string, color: string, zoom: number): void {
        this.zoom = zoom;
        this.createCanvasIfRequired();
        this.render(text, color);

        this.tween?.stop();
        this.position.x = x;
        this.position.y = y;
        this.setOpacity(1);
        this.visible = true;
        this.applyScale();

        const state = { y: this.position.y, opacity: 1 };
        this.tween = new TWEEN.Tween(state)
            .to({ y: this.position.y + (RISE_DISTANCE / zoom), opacity: 0 }, LIFETIME_MS)
            .easing(TWEEN.Easing.Quadratic.Out)
            .onUpdate(() => {
                this.position.y = state.y;
                this.setOpacity(state.opacity);
            })
            .onComplete(() => {
                this.visible = false;
            })
            .start();
    }

    /**
     * Releases the canvas, texture and material of this number.
     * They are created again when the number is shown the next time.
     */
    public dispose(): void {
        this.tween?.stop();
        this.tween = null;
        this.visible = false;
        this.material = DamageNumber.hiddenMaterial;

        if (this.labelMaterial !== null) {
            this.labelMaterial.dispose();
            this.labelMaterial = null;
        }

        if (this.texture !== null) {
            this.texture.dispose();
            this.texture = null;
        }

        if (this.canvas !== null) {
            // Releases the backing store of the canvas, which is by far the biggest part of a number.
            this.canvas.width = EMPTY_CANVAS_SIZE;
            this.canvas.height = EMPTY_CANVAS_SIZE;
            this.canvas = null;
        }

        this.context = null;
    }

    /*
     * Creates the canvas, texture and material of this number, if that didn't happen yet.
     * They are created lazily, because the pool is much bigger than the number of
     * hits which are usually shown at the same time.
     */
    private createCanvasIfRequired(): void {
        if (this.canvas !== null) {
            return;
        }

        const canvas = document.createElement("canvas");
        canvas.height = CANVAS_HEIGHT;
        const context = canvas.getContext("2d");
        if (!context) {
            throw new Error("Failed to get 2D context");
        }

        this.canvas = canvas;
        this.context = context;
        this.texture = new THREE.CanvasTexture(canvas);
        this.labelMaterial = new THREE.SpriteMaterial({
            depthTest: false,
            depthWrite: false,
            map: this.texture,
            transparent: true,
        });
        this.material = this.labelMaterial;
    }

    private setOpacity(opacity: number): void {
        if (this.labelMaterial !== null) {
            this.labelMaterial.opacity = opacity;
        }
    }

    private applyScale(): void {
        if (this.unscaledWidth === EMPTY_CANVAS_SIZE || this.unscaledHeight === EMPTY_CANVAS_SIZE) {
            return;
        }

        this.scale.set(this.unscaledWidth / this.zoom, this.unscaledHeight / this.zoom, 1);
    }

    private render(text: string, color: string): void {
        const ctx = this.context;
        const canvas = this.canvas;
        const texture = this.texture;
        if (ctx === null || canvas === null || texture === null) {
            return;
        }

        ctx.font = FONT;
        const textWidth = ctx.measureText(text).width;
        const width = Math.max(MIN_CANVAS_WIDTH, this.nextPowerOfTwo(textWidth + PADDING * 2));

        // Resizing resets the context, so the font is applied again below.
        canvas.width = width;
        canvas.height = CANVAS_HEIGHT;
        ctx.font = FONT;

        const centerX = canvas.width / 2;
        const centerY = canvas.height / 2;
        ctx.textAlign = "center";
        ctx.textBaseline = "middle";
        ctx.lineJoin = "round";
        ctx.lineWidth = OUTLINE_WIDTH;
        ctx.strokeStyle = "rgba(0, 0, 0, 0.9)";
        ctx.strokeText(text, centerX, centerY);
        ctx.fillStyle = color;
        ctx.fillText(text, centerX, centerY);

        this.unscaledWidth = (canvas.width / canvas.height) * DAMAGE_SCALE_Y;
        this.unscaledHeight = DAMAGE_SCALE_Y;
        texture.needsUpdate = true;
    }

    private nextPowerOfTwo(value: number): number {
        return Math.pow(2, Math.ceil(Math.log2(value)));
    }
}

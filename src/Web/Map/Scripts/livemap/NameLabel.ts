import * as THREE from "three";

const CANVAS_HEIGHT = 64;
const LABEL_SCALE_X = 40;
const LABEL_SCALE_Y = 6;
const LABEL_SCALE_Z = 1;
const CLEAR_X = 0;
const CLEAR_Y = 0;
const SCALE_MULTIPLIER = 8;
const EMPTY_CANVAS_SIZE = 0;

export class NameLabel extends THREE.Sprite {
    /*
     * A shared material for labels which were never shown. It's never rendered
     * (the label is invisible until 'show' is called) and therefore never
     * allocates any resources in the renderer.
     */
    private static readonly hiddenMaterial: THREE.SpriteMaterial = new THREE.SpriteMaterial();

    private canvas: HTMLCanvasElement | null = null;
    private context: CanvasRenderingContext2D | null = null;
    private texture: THREE.CanvasTexture | null = null;
    private labelMaterial: THREE.SpriteMaterial | null = null;

    constructor() {
        super(NameLabel.hiddenMaterial);

        this.scale.set(LABEL_SCALE_X, LABEL_SCALE_Y, LABEL_SCALE_Z);
        this.visible = false;
    }

    public raycast(): void {
        // No-op: name labels should not intercept raycasts.
    }

    public show(name: string): void {
        this.createCanvasIfRequired();
        this.renderLabel(name);
        this.visible = true;
    }

    public hide(): void {
        this.visible = false;
    }

    /**
     * Releases the canvas, texture and material of this label.
     * They are created again when the label is shown the next time.
     */
    public dispose(): void {
        this.visible = false;
        this.material = NameLabel.hiddenMaterial;

        if (this.labelMaterial !== null) {
            this.labelMaterial.dispose();
            this.labelMaterial = null;
        }

        if (this.texture !== null) {
            this.texture.dispose();
            this.texture = null;
        }

        if (this.canvas !== null) {
            // Releases the backing store of the canvas, which is by far the biggest part of a label.
            this.canvas.width = EMPTY_CANVAS_SIZE;
            this.canvas.height = EMPTY_CANVAS_SIZE;
            this.canvas = null;
        }

        this.context = null;
    }

    /*
     * Creates the canvas, texture and material of this label, if that didn't happen yet.
     * They are created lazily, because most objects on the map are never hovered
     * and a canvas costs a few hundred kilobytes each.
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
        this.labelMaterial = new THREE.SpriteMaterial({ map: this.texture, transparent: true });
        this.material = this.labelMaterial;
    }

    private renderLabel(name: string): void {
        const ctx = this.context;
        const canvas = this.canvas;
        const texture = this.texture;
        if (ctx === null || canvas === null || texture === null) {
            return;
        }

        ctx.font = "bold 28px Consolas, monospace";
        const textWidth = ctx.measureText(name).width;

        const padding = 12;
        const labelWidth = textWidth + padding * 2;
        const labelHeight = 40;

        canvas.width = this.nextPowerOfTwo(labelWidth + padding * 2);
        canvas.height = labelHeight + padding * 2;

        // Re-apply font after canvas resize (resize resets context state)
        ctx.font = "bold 28px Consolas, monospace";
        this.scale.set((canvas.width / canvas.height) * SCALE_MULTIPLIER, SCALE_MULTIPLIER, LABEL_SCALE_Z);

        ctx.clearRect(CLEAR_X, CLEAR_Y, canvas.width, canvas.height);

        const x = (canvas.width - labelWidth) / 2;
        const y = (canvas.height - labelHeight) / 2;
        const radius = 8;

        ctx.fillStyle = "rgba(0, 0, 0, 0.65)";
        ctx.beginPath();
        ctx.moveTo(x + radius, y);
        ctx.lineTo(x + labelWidth - radius, y);
        ctx.quadraticCurveTo(x + labelWidth, y, x + labelWidth, y + radius);
        ctx.lineTo(x + labelWidth, y + labelHeight - radius);
        ctx.quadraticCurveTo(x + labelWidth, y + labelHeight, x + labelWidth - radius, y + labelHeight);
        ctx.lineTo(x + radius, y + labelHeight);
        ctx.quadraticCurveTo(x, y + labelHeight, x, y + labelHeight - radius);
        ctx.lineTo(x, y + radius);
        ctx.quadraticCurveTo(x, y, x + radius, y);
        ctx.closePath();
        ctx.fill();

        ctx.fillStyle = "#ffffff";
        ctx.textAlign = "center";
        ctx.textBaseline = "middle";

        ctx.fillText(name, canvas.width / 2, canvas.height / 2);

        texture.needsUpdate = true;
    }

    private nextPowerOfTwo(value: number): number {
        return Math.pow(2, Math.ceil(Math.log2(value)));
    }
}

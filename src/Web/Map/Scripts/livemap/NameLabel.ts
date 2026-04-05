import * as THREE from "three";

export class NameLabel extends THREE.Sprite {
    private readonly canvas: HTMLCanvasElement;
    private readonly context: CanvasRenderingContext2D;
    private readonly texture: THREE.CanvasTexture;
    private isVisible: boolean = false;

    constructor() {
        const canvas = document.createElement("canvas");
        canvas.width = 512;
        canvas.height = 64;
        const context = canvas.getContext("2d")!;
        const texture = new THREE.CanvasTexture(canvas);
        const material = new THREE.SpriteMaterial({ map: texture, transparent: true });

        super(material);

        this.canvas = canvas;
        this.context = context;
        this.texture = texture;

        this.scale.set(40, 6, 1);
        this.visible = false;
    }

    public raycast(): void {
        // No-op: name labels should not intercept raycasts.
    }

    public show(name: string): void {
        this.renderLabel(name);
        this.visible = true;
        this.isVisible = true;
    }

    public hide(): void {
        this.visible = false;
        this.isVisible = false;
    }

    private renderLabel(name: string): void {
        const ctx = this.context;
        const canvas = this.canvas;

        ctx.font = "bold 28px Consolas, monospace";
        const textWidth = ctx.measureText(name).width;

        const padding = 12;
        const labelWidth = textWidth + padding * 2;
        const labelHeight = 40;

        canvas.width = this.nextPowerOfTwo(labelWidth + padding * 2);
        canvas.height = labelHeight + padding * 2;

        // Re-apply font after canvas resize (resize resets context state)
        ctx.font = "bold 28px Consolas, monospace";
        this.scale.set((canvas.width / canvas.height) * 8, 8, 1);

        ctx.clearRect(0, 0, canvas.width, canvas.height);

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

        this.texture.needsUpdate = true;
    }

    private nextPowerOfTwo(value: number): number {
        return Math.pow(2, Math.ceil(Math.log2(value)));
    }
}

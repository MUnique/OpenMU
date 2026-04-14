import * as THREE from "three";
import { ObjectData } from "./Types";
import { GameObject } from "./GameObject";
import { World } from "./World";

function isGameObject(obj: THREE.Object3D): obj is GameObject {
    return (obj as GameObject).data !== undefined;
}

export class WorldObjectPicker {
    private readonly raycaster = new THREE.Raycaster();
    private readonly mouse = new THREE.Vector2();

    private isHoverRaycastPending = false;
    private lastMouseX = 0;
    private lastMouseY = 0;
    private static readonly hoverThresholdSquared = 16;

    private readonly handleClick: (e: MouseEvent) => void;
    private readonly handleMouseMove: (e: MouseEvent) => void;

    constructor(
        private readonly worldCanvas: HTMLElement,
        private readonly worldMesh: World,
        private readonly camera: THREE.Camera,
        private readonly onObjectPicked: (data: ObjectData) => void,
        private readonly onObjectHovered?: (data: ObjectData | null) => void,
    ) {
        this.handleClick = this.onClick.bind(this);
        this.handleMouseMove = this.onMouseMove.bind(this);

        worldCanvas.addEventListener("click", this.handleClick, false);
        worldCanvas.addEventListener("mousemove", this.handleMouseMove, false);
    }

    public dispose(): void {
        this.worldCanvas.removeEventListener("click", this.handleClick);
        this.worldCanvas.removeEventListener("mousemove", this.handleMouseMove);
    }

    private onClick(e: MouseEvent): void {
        const data = this.pickAt(e.offsetX, e.offsetY);
        if (data !== null) {
            this.onObjectPicked(data);
        }
    }

    private onMouseMove(e: MouseEvent): void {
        if (!this.onObjectHovered) {
            return;
        }

        const dx = e.offsetX - this.lastMouseX;
        const dy = e.offsetY - this.lastMouseY;
        if (dx * dx + dy * dy < WorldObjectPicker.hoverThresholdSquared) {
            return;
        }

        if (!this.isHoverRaycastPending) {
            this.isHoverRaycastPending = true;
            this.lastMouseX = e.offsetX;
            this.lastMouseY = e.offsetY;

            const offsetX = e.offsetX;
            const offsetY = e.offsetY;

            requestAnimationFrame(() => {
                this.isHoverRaycastPending = false;
                this.onObjectHovered!(this.pickAt(offsetX, offsetY));
            });
        }
    }

    private pickAt(offsetX: number, offsetY: number): ObjectData | null {
        this.mouse.x = (offsetX / this.worldCanvas.clientWidth) * 2 - 1;
        this.mouse.y = -(offsetY / this.worldCanvas.clientHeight) * 2 + 1;
        this.raycaster.setFromCamera(this.mouse, this.camera);

        const intersects = this.raycaster.intersectObjects(this.worldMesh.children, true);
        return intersects.length ? this.extractObjectData(intersects[0]) : null;
    }

    private extractObjectData(intersection: THREE.Intersection): ObjectData | null {
        let obj: THREE.Object3D | null = intersection.object;
        while (obj) {
            if (isGameObject(obj) && obj.data.id !== undefined) {
                return obj.data;
            }
            obj = obj.parent;
        }
        return null;
    }
}

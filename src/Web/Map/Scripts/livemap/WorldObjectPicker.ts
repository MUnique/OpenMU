import * as THREE from "three";
import { ObjectData } from "./Types";
import { GameObject } from "./GameObject";
import { World } from "./World";

export class WorldObjectPicker {
    constructor(
        worldCanvas: HTMLElement,
        worldMesh: World,
        camera: THREE.Camera,
        onObjectPicked: (data: ObjectData) => void,
        onObjectHovered?: (data: ObjectData | null) => void) {

        const raycaster = new THREE.Raycaster();
        const mouse = new THREE.Vector2();
        raycaster.setFromCamera(mouse, camera);

        const pick = (mouseEvent: MouseEvent, onHit: (data: ObjectData | null) => void): void => {
            mouse.x = (mouseEvent.offsetX / worldCanvas.clientWidth) * 2 - 1;
            mouse.y = -(mouseEvent.offsetY / worldCanvas.clientHeight) * 2 + 1;
            raycaster.setFromCamera(mouse, camera);
            const intersects = raycaster.intersectObjects(worldMesh.children, true);
            const data = intersects.length
                ? this.extractObjectData(intersects[0])
                : null;
            onHit(data);
        };

        worldCanvas.addEventListener("click", (mouseEvent: MouseEvent) => {
            pick(mouseEvent, (data) => {
                if (data !== null && onObjectPicked) {
                    onObjectPicked(data);
                }
            });
        }, false);

        worldCanvas.addEventListener("mousemove", (mouseEvent: MouseEvent) => {
            pick(mouseEvent, (data) => {
                if (onObjectHovered) {
                    onObjectHovered(data);
                }
            });
        }, false);
    }

    private extractObjectData(intersection: THREE.Intersection): ObjectData | null {
        let obj: THREE.Object3D | null = intersection.object;
        while (obj) {
            const gameObject = obj as GameObject;
            if (gameObject.data && gameObject.data.id !== undefined) {
                return gameObject.data;
            }
            obj = obj.parent;
        }

        return null;
    }
}

import * as THREE from "three";
import { ObjectData } from "./Types";
import { GameObject } from "./GameObject";
import { World } from "./World";

export class WorldObjectPicker {
    constructor(
        worldCanvas: HTMLElement,
        worldMesh: World,
        camera: THREE.Camera,
        onObjectPicked: (data: ObjectData) => void) {

        const raycaster = new THREE.Raycaster();
        const mouse = new THREE.Vector2();
        raycaster.setFromCamera(mouse, camera);

        worldCanvas.addEventListener("click", (mouseEvent: MouseEvent) => {
            mouse.x = (mouseEvent.offsetX / worldCanvas.clientWidth) * 2 - 1;
            mouse.y = -(mouseEvent.offsetY / worldCanvas.clientHeight) * 2 + 1;
            raycaster.setFromCamera(mouse, camera);
            const intersects = raycaster.intersectObjects(worldMesh.children, true);
            if (intersects.length > 0 && onObjectPicked) {
                const data = this.extractObjectData(intersects[0]);
                onObjectPicked(data);
            }
        }, false);
    }

    private extractObjectData(intersection: THREE.Intersection): ObjectData {
        const gameObject = intersection.object as GameObject;
        if (gameObject != null) {
            return gameObject.data;
        }

        return null;
    }
}

import * as THREE from "three";
import { ObjectData } from "./Types";
import { GameObject } from "./Attackable";

export class WorldObjectPicker {
    constructor(worldCanvas: any, worldMesh: any, camera: any, onObjectPicked: (data: ObjectData) => void) {
        var raycaster = new THREE.Raycaster();
        var mouse = new THREE.Vector2();
        raycaster.setFromCamera(mouse, camera);
        worldCanvas.addEventListener('click', (mouseEvent: any) => {
            mouse.x = (mouseEvent.offsetX / worldCanvas.clientWidth) * 2 - 1;
            mouse.y = -(mouseEvent.offsetY / worldCanvas.clientHeight) * 2 + 1;
            raycaster.setFromCamera(mouse, camera);
            let intersects = raycaster.intersectObjects(worldMesh.children, true);
            if (intersects.length > 0 && onObjectPicked) {
                let data = this.extractObjectData(intersects[0]);
                onObjectPicked(data);
            }
        }, false);
    }

    extractObjectData(intersection: THREE.Intersection): ObjectData {
        let attackable = intersection.object as GameObject;
        if (attackable != null) {
            return attackable.data;
        }

        return null;
    }
};
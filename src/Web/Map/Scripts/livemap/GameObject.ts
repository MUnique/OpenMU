import * as THREE from "three";
import { ObjectData, Step, Direction } from "./Types";

export interface GameObject extends THREE.Object3D {
    data: ObjectData;
    respawn(newData: ObjectData): void;
    moveTo(newX: number, newY: number, moveType: any, walkDelay: number, steps: Step[]): void;
    rotateTo(rotation: Direction): void;
    gotKilled(): void;

    /**
     * Scales the labels of the object so that they keep their size on the screen when the map is zoomed.
     * @param zoom - The zoom factor of the camera.
     */
    setZoom(zoom: number): void;

    /**
     * Stops all running animations and releases the resources (materials, textures)
     * which are exclusively used by this object.
     */
    dispose(): void;
}

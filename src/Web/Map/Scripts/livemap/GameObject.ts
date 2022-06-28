import * as THREE from "three";
import { ObjectData, Step, Direction } from "./Types";

export interface GameObject extends THREE.Object3D {
    data: ObjectData;
    respawn(newData: ObjectData): void;
    moveTo(newX: number, newY: number, moveType: any, walkDelay: number, steps: Step[]): void;
    rotateTo(rotation: Direction): void;
    gotKilled(): void;
}

import * as THREE from "three";
import TWEEN from "tween";
import { Queue } from "./Queue";

export class Attacks extends THREE.Points {
    // private static readonly visibleZ = 0;
    // private static readonly invisibleZ = -100; // below the map plane

    freeAttackIndexes: Queue<number>;
    pointLifetimeInMs: number;
    geometry: THREE.Geometry;

    constructor() {
        const pointsMaterial = new THREE.PointsMaterial({
            size: 2,
            vertexColors: THREE.VertexColors,
            sizeAttenuation: false
        });
        pointsMaterial.needsUpdate = true;
        const colors: THREE.Color[] = [];
        const maximumAnimatedPoints = 10000;
        const animatedPointsGeometry = new THREE.Geometry(); // TODO: Use BufferGeometry
        const queue = new Queue<number>();
        for (let i = 0; i < maximumAnimatedPoints; i++) {
            animatedPointsGeometry.vertices.push(new THREE.Vector3(0, 0, -Infinity));

            const color = new THREE.Color();
            color.setHex(0xEE0000);
            colors.push(color);
            queue.enqueue(i);
        }

        animatedPointsGeometry.colors = colors;
        animatedPointsGeometry.colorsNeedUpdate = true;
        animatedPointsGeometry.elementsNeedUpdate = true;

        super(animatedPointsGeometry, pointsMaterial);
        this.pointLifetimeInMs = 500; // attack particles survive 500 ms
        this.freeAttackIndexes = queue;
        this.geometry = animatedPointsGeometry;
    }

    update(): void {
    }

    public addAttack(attacker: any, target: any) {
        if (this.freeAttackIndexes.peek() === null) {
            return;
        }

        if (target === undefined || target === null) {
            return;
        }

        const newIndex = this.freeAttackIndexes.dequeue();
        if (newIndex === null) {
            return;
        }

        this.geometry.colors[newIndex].setHex(0xFF0000 + attacker.data.Id); // attacker id is only a 16 bit integer, so we can just add it to the red color.
        this.geometry.colorsNeedUpdate = true;

        const visibleZ = 0;
        const invisibleZ = -Infinity; // we assume the map plane is at z=0, and this object sits above.

        const vertice = this.geometry.vertices[newIndex];
        vertice.set(attacker.position.x, attacker.position.y, visibleZ);
        const state = { x: vertice.x, y: vertice.y };
        const tween = new TWEEN.Tween(state)
            .to({ x: target.position.x, y: target.position.y }, this.pointLifetimeInMs)
            .onUpdate(() => {
                vertice.set(state.x, state.y, visibleZ);
                this.geometry.verticesNeedUpdate = true;
            })
            .onComplete(() => {
                vertice.z = invisibleZ; // hide below the map
                this.geometry.verticesNeedUpdate = true;
                this.freeAttackIndexes.enqueue(newIndex);
            })
            .start();

        this.geometry.verticesNeedUpdate = true;
    }
};

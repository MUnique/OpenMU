import * as THREE from "three";
import TWEEN from "tween";
import { Queue } from "./Queue";

export class Attacks extends THREE.Points {
    freeAttackIndexes: Queue<number>;
    pointLifetimeInMs: number;
    geometry: THREE.Geometry;
    constructor() {
        let pointsMaterial =
            new THREE.PointsMaterial({
                size: 2,
                vertexColors: THREE.VertexColors,
                sizeAttenuation: false
            });
        pointsMaterial.needsUpdate = true;
        let colors: THREE.Color[] = [];
        let maximumAnimatedPoints = 10000;
        let animatedPointsGeometry = new THREE.Geometry(); // TODO: Use BufferGeometry
        let queue = new Queue<number>();
        for (let i = 0; i < maximumAnimatedPoints; i++) {
            animatedPointsGeometry.vertices.push(new THREE.Vector3(0, 0, -100)); // is hidden behind the map plane

            let color = new THREE.Color();
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

    update() {
    }

    public addAttack(attacker: any, target: any) {
        if (this.freeAttackIndexes.peek() === null) {
            return;
        }

        if (target === undefined || target === null) {
            return;
        }

        let newIndex = this.freeAttackIndexes.dequeue();
        if (newIndex === null) {
            return;
        }

        this.geometry.colors[newIndex].setHex(0xFF0000 + attacker.data.Id); // attacker id is only a 16 bit integer, so we can just add it to the red color.
        this.geometry.colorsNeedUpdate = true;

        let vertice = this.geometry.vertices[newIndex];
        vertice.set(attacker.position.x, attacker.position.y, 0);
        let state = { x: vertice.x, y: vertice.y };
        let tween = new TWEEN.Tween(state)
            .to({ x: target.position.x, y: target.position.y }, this.pointLifetimeInMs)
            .onUpdate(() => {
                vertice.set(state.x, state.y, 0);
                this.geometry.verticesNeedUpdate = true;
            })
            .onComplete(() => {
                vertice.z = -100; // hide below the map
                this.geometry.verticesNeedUpdate = true;
                this.freeAttackIndexes.enqueue(newIndex);
            })
            .start();

        this.geometry.verticesNeedUpdate = true;
    }
};
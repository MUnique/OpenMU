import * as THREE from "three";
import TWEEN from "tween";
import { WorldObjectPicker } from "./WorldObjectPicker";
import { World } from "./World";
import { store, renderPlayerList } from "./PlayerList";
import { ObjectData } from "../stores/map/types";


/*
 * Minimalistic interface for the stats object which shows the fps and frametimes.
 */
interface Stats {
    update(): void;
}

// TODO: create component
/*
 * Class for a Map application which is shown on the whole browser page.
 */
export class MapApp {
    private stats: Stats;
    private camera: THREE.Camera;
    private world: World;
    private scene: THREE.Scene;
    private renderer: THREE.Renderer;
    private container: HTMLElement;
    private picker: WorldObjectPicker;

    constructor(stats: Stats, serverId: number, mapId: number, mapContainer: HTMLElement, playerListContainer: HTMLElement, onPickObjectHandler: (data: ObjectData) => void) {
        this.stats = stats;
        this.container = mapContainer;
        this.renderer = new THREE.WebGLRenderer({ antialias: false });
        this.scene = new THREE.Scene();
        this.world = new World(store, serverId, mapId);
        this.scene.add(this.world);
        this.camera = this.createCamera();

        this.renderer.setSize(window.innerHeight, window.innerHeight);
        this.container.appendChild(this.renderer.domElement);
        this.onWindowResize();
        window.addEventListener('resize', this.onWindowResize, false);

        this.picker = new WorldObjectPicker(mapContainer, this.world, this.camera, onPickObjectHandler);

        renderPlayerList(playerListContainer, this.world);


        this.animate(); //starts the rendering loop
    }

    private animate(time?: any) {
        requestAnimationFrame(() => this.animate(time)); // request the next frame to be rendered
        this.stats.update(); // updates the stats (fps and frametimes)
        TWEEN.update(time); // updates all existing Tweens
        this.world.update(); // update world and it's objects
        this.renderer.render(this.scene, this.camera);
    }

    /*
     * Creates an orthographic camera which looks down to the map plane from the center.
     */
    private createCamera(): THREE.Camera {
        const MAP_SIZE = 256;

        const NEAR = 0.1, FAR = 10000;

        var camera = new THREE.OrthographicCamera(MAP_SIZE / -2, MAP_SIZE / 2, MAP_SIZE / 2, MAP_SIZE / -2, NEAR, FAR);
        camera.position.z = 1000;
        return camera;
    }

    /*
     * Handles the window resizing by updating the resolution of the renderer.
     */
    private onWindowResize() {
        var newSize = window.innerHeight;
        this.container.style.width = newSize + 'px';
        this.renderer.setSize(newSize, newSize);
        this.world.onSizeChanged(newSize);
    }
}
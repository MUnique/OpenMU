import * as THREE from "three";
import TWEEN from "tween";
import { WorldObjectPicker } from "./WorldObjectPicker";
import { World } from "./World";
import { MonsterFilterMatch, MonsterFilterMode, ObjectData } from "./Types";

/**
 * Class for a Map application which is shown on the whole browser page.
 */
export class MapApp {
    private static readonly mapSize: number = 256;
    private static readonly minZoom: number = 1;
    private static readonly maxZoom: number = 8;
    private static readonly wheelZoomSpeed: number = 0.0018;
    private static readonly maxWheelDelta: number = 200;
    private static readonly pixelsPerWheelLine: number = 33;
    private static readonly pixelsPerWheelPage: number = 100;

    private stats: Stats;
    private camera: THREE.OrthographicCamera;
    private world: World;
    private scene: THREE.Scene;
    private renderer: THREE.Renderer;
    private container: HTMLElement;
    private picker: WorldObjectPicker;
    private lastHighlightedId: number | null = null;
    private isDisposing: boolean = false;
    private isDisposed: boolean = false;
    private isPanning: boolean = false;
    private lastPanX: number = 0;
    private lastPanY: number = 0;
    private animationFrameId: number | null = null;
    private resizeEventListener: () => void;
    private wheelEventListener: (e: WheelEvent) => void;
    private mouseDownEventListener: (e: MouseEvent) => void;
    private mouseMoveEventListener: (e: MouseEvent) => void;
    private mouseUpEventListener: (e: MouseEvent) => void;
    private contextMenuEventListener: (e: Event) => void;
    private readonly onPickObjectHandler: (data: ObjectData) => void;

    constructor(
        stats: Stats,
        serverId: number,
        mapId: string,
        mapContainer: HTMLElement,
        onPickObjectHandler: (data: ObjectData) => void) {
        this.stats = stats;
        this.container = mapContainer;
        this.onPickObjectHandler = onPickObjectHandler;
        this.renderer = new THREE.WebGLRenderer({ antialias: false });
        this.scene = new THREE.Scene();
        this.world = new World(serverId, mapId);
        this.scene.add(this.world);
        this.camera = this.createCamera();

        this.renderer.setSize(window.innerHeight, window.innerHeight);
        this.container.appendChild(this.renderer.domElement);
        this.onWindowResize();
        this.resizeEventListener = () => this.onWindowResize();
        window.addEventListener("resize", this.resizeEventListener, false);

        this.wheelEventListener = (e) => this.onWheel(e);
        this.mouseDownEventListener = (e) => this.onPanStart(e);
        this.mouseMoveEventListener = (e) => this.onPanMove(e);
        this.mouseUpEventListener = () => this.onPanEnd();
        this.contextMenuEventListener = (e) => e.preventDefault();
        this.renderer.domElement.addEventListener("wheel", this.wheelEventListener, { passive: false });
        this.renderer.domElement.addEventListener("mousedown", this.mouseDownEventListener, false);
        this.renderer.domElement.addEventListener("contextmenu", this.contextMenuEventListener, false);
        window.addEventListener("mousemove", this.mouseMoveEventListener, false);
        window.addEventListener("mouseup", this.mouseUpEventListener, false);

        const hideInfo = () => {
            const info = document.getElementById("selected_info");
            if (info) {
                info.style.display = "none";
            }
        };

        this.picker = new WorldObjectPicker(
            this.renderer.domElement,
            this.world,
            this.camera,
            (data) => {
                this.onObjectPicked(data);
                if (data !== null) {
                    onPickObjectHandler(data);
                } else {
                    hideInfo();
                }
            },
            (data) => this.onObjectHovered(data));

        this.animate(); //starts the rendering loop
    }

    public highlightByName(playerName: string): boolean {
        return this.world.highlightPlayerByName(playerName);
    }

    /**
     * Sets the monster filter, see World.setMonsterFilter.
     * @param mode - How the name is compared with the text.
     * @param text - The text; an empty one shows all monsters again.
     */
    public setMonsterFilter(mode: MonsterFilterMode, text: string): void {
        this.world.setMonsterFilter(mode, text);
    }

    /**
     * Gets the monsters which match the monster filter, see World.getMonsterFilterMatches.
     * @returns The matching monsters; empty when no filter is set.
     */
    public getMonsterFilterMatches(): MonsterFilterMatch[] {
        return this.world.getMonsterFilterMatches();
    }

    /**
     * Selects the object with the specified id, as if it was clicked on the map.
     * @param objectId - The id of the object to select.
     * @returns True if the object was found and selected, false otherwise.
     */
    public selectObject(objectId: number): boolean {
        const data = this.world.getObjectById(objectId)?.data as ObjectData | undefined;
        if (data === undefined) {
            // The object isn't in the scene yet. Clear any stale selection so the
            // previously highlighted object doesn't appear to still be selected.
            this.onObjectPicked(null);
            const info = document.getElementById("selected_info");
            if (info) {
                info.style.display = "none";
            }

            return false;
        }

        this.onObjectPicked(data);
        this.onPickObjectHandler(data);
        return true;
    }

    /**
     * Resets the camera to the whole map.
     */
    public resetView(): void {
        this.camera.zoom = MapApp.minZoom;
        this.camera.position.x = 0;
        this.camera.position.y = 0;
        this.camera.updateProjectionMatrix();
        this.world.setZoom(this.camera.zoom);
    }

    public dispose(): void {
        if (this.isDisposing || this.isDisposed) {
            return;
        }

        this.isDisposing = true;
        window.removeEventListener("resize", this.resizeEventListener);
        window.removeEventListener("mousemove", this.mouseMoveEventListener);
        window.removeEventListener("mouseup", this.mouseUpEventListener);
        this.renderer.domElement.removeEventListener("wheel", this.wheelEventListener);
        this.renderer.domElement.removeEventListener("mousedown", this.mouseDownEventListener);
        this.renderer.domElement.removeEventListener("contextmenu", this.contextMenuEventListener);
        if (this.animationFrameId !== null) {
            cancelAnimationFrame(this.animationFrameId);
            this.animationFrameId = null;
        }

        this.picker.dispose();

        this.scene.remove(this.world);
        this.world.dispose();
        this.world = null;

        const canvas = this.renderer.domElement;
        const webGlRenderer = this.renderer as THREE.WebGLRenderer;
        if (webGlRenderer != null) {
            webGlRenderer.dispose();

            // Releases the WebGL context. Browsers only allow a limited number of them,
            // so we don't want to keep it until it's garbage collected.
            webGlRenderer.forceContextLoss();
        }

        if (canvas.parentNode !== null) {
            canvas.parentNode.removeChild(canvas);
        }

        this.renderer = null;
        this.isDisposing = false;
        this.isDisposed = true;
    }


    private animate(time?: number): void {
        if (this.isDisposing || this.isDisposed) {
            return;
        }

        // request the next frame to be rendered, with the timestamp of that frame
        this.animationFrameId = requestAnimationFrame((frameTime) => this.animate(frameTime));
        this.stats?.update(); // updates the stats (fps and frametimes)
        TWEEN.update(time); // updates all existing Tweens
        this.world.update(); // update world and it's objects
        this.renderer.render(this.scene, this.camera);
    }

    /**
     * Creates an orthographic camera which looks down to the map plane from the center.
     */
    private createCamera(): THREE.OrthographicCamera {
        const NEAR = 0.1, FAR = 10000;

        const camera = new THREE.OrthographicCamera(
            MapApp.mapSize / -2,
            MapApp.mapSize / 2,
            MapApp.mapSize / 2,
            MapApp.mapSize / -2,
            NEAR,
            FAR);
        camera.position.z = 1000;
        return camera;
    }

    /**
     * Zooms the map in or out, so that the point under the mouse cursor stays in place.
     * @param event - The wheel event.
     */
    private onWheel(event: WheelEvent): void {
        event.preventDefault();
        if (event.deltaY === 0) {
            return;
        }

        // A mouse wheel step zooms by about 20 percent. The zoom depends on the distance which is scrolled,
        // because a touchpad sends many small steps, which would otherwise zoom in or out at once.
        const factor = Math.exp(-this.getWheelDeltaInPixels(event) * MapApp.wheelZoomSpeed);
        const newZoom = THREE.Math.clamp(this.camera.zoom * factor, MapApp.minZoom, MapApp.maxZoom);
        if (newZoom === this.camera.zoom) {
            return;
        }

        const before = this.screenToMapPoint(event.offsetX, event.offsetY);
        this.camera.zoom = newZoom;
        this.camera.updateProjectionMatrix();
        const after = this.screenToMapPoint(event.offsetX, event.offsetY);

        this.camera.position.x += before.x - after.x;
        this.camera.position.y += before.y - after.y;
        this.clampCamera();
        this.world.setZoom(newZoom);
    }

    /**
     * Starts panning when the map is dragged with the right or middle mouse button.
     * @param event - The mouse event.
     */
    private onPanStart(event: MouseEvent): void {
        const isPanButton = event.button === 1 || event.button === 2;
        if (!isPanButton || this.camera.zoom <= MapApp.minZoom) {
            return;
        }

        event.preventDefault();
        this.isPanning = true;
        this.lastPanX = event.clientX;
        this.lastPanY = event.clientY;
    }

    /**
     * Moves the camera while the map is dragged.
     * @param event - The mouse event.
     */
    private onPanMove(event: MouseEvent): void {
        if (!this.isPanning) {
            return;
        }

        const panButtons = 2 | 4;
        if ((event.buttons & panButtons) === 0) {
            // The button was released where we didn't get the event, e.g. outside of the browser window.
            this.isPanning = false;
            return;
        }

        const unitsPerPixel = (MapApp.mapSize / this.camera.zoom) / this.renderer.domElement.clientWidth;
        this.camera.position.x -= (event.clientX - this.lastPanX) * unitsPerPixel;
        this.camera.position.y += (event.clientY - this.lastPanY) * unitsPerPixel;
        this.lastPanX = event.clientX;
        this.lastPanY = event.clientY;
        this.clampCamera();
    }

    /**
     * Gets the distance which the wheel was scrolled, in pixels, whatever unit the browser reports.
     * @param event - The wheel event.
     * @returns The distance in pixels, limited to a few wheel steps.
     */
    private getWheelDeltaInPixels(event: WheelEvent): number {
        const lineMode = 1;
        const pageMode = 2;
        let delta = event.deltaY;
        if (event.deltaMode === lineMode) {
            delta *= MapApp.pixelsPerWheelLine;
        } else if (event.deltaMode === pageMode) {
            delta *= MapApp.pixelsPerWheelPage;
        }

        return THREE.Math.clamp(delta, -MapApp.maxWheelDelta, MapApp.maxWheelDelta);
    }

    /**
     * Stops panning when the mouse button is released.
     */
    private onPanEnd(): void {
        this.isPanning = false;
    }

    /**
     * Converts a position on the canvas to a point on the map plane.
     * @param offsetX - The x offset within the canvas.
     * @param offsetY - The y offset within the canvas.
     * @returns The point on the map plane.
     */
    private screenToMapPoint(offsetX: number, offsetY: number): THREE.Vector3 {
        const canvas = this.renderer.domElement;
        const x = (offsetX / canvas.clientWidth) * 2 - 1;
        const y = -(offsetY / canvas.clientHeight) * 2 + 1;
        return new THREE.Vector3(x, y, 0).unproject(this.camera);
    }

    /**
     * Keeps the visible area within the bounds of the map, so the map can't be dragged out of view.
     */
    private clampCamera(): void {
        const halfVisibleSize = (MapApp.mapSize / 2) / this.camera.zoom;
        const maximumOffset = Math.max(0, (MapApp.mapSize / 2) - halfVisibleSize);
        this.camera.position.x = THREE.Math.clamp(this.camera.position.x, -maximumOffset, maximumOffset);
        this.camera.position.y = THREE.Math.clamp(this.camera.position.y, -maximumOffset, maximumOffset);
    }

    /**
     * Handles the window resizing by updating the resolution of the renderer.
     */
    private onWindowResize(): void {
        const margin = 50;
        const preferredWidth = window.innerWidth - this.container.offsetLeft - margin;
        const preferredHeigth = window.innerHeight - this.container.offsetTop - margin;
        const newSize = Math.min(preferredWidth, preferredHeigth);
        this.renderer.setSize(newSize, newSize);
        this.world.onSizeChanged(newSize);
    }

    private onObjectPicked(data: ObjectData | null): void {
        if (this.lastHighlightedId !== null) {
            this.world.highlightOff(this.lastHighlightedId);
            this.lastHighlightedId = null;
        }

        if (data !== null) {
            this.world.highlightOn(data.id);
            this.lastHighlightedId = data.id;
        }
    }

    private onObjectHovered(data: ObjectData | null): void {
        if (data !== null) {
            this.world.showLabel(data.id);
        } else {
            this.world.hideLastLabel();
        }
    }
}

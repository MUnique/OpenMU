import * as THREE from "three";
import {Store} from "redux";
import { Attacks } from "./Attacks";
import { terrainShader } from "./TerrainShader";
import { WorldUpdater } from "./WorldUpdater";
import { Player } from "./Player";
import { GameObject } from "./Attackable";
import {NonPlayerCharacter as NPC} from "../liveview/NonPlayerCharacter";

import { addPlayer, removePlayer, highlightPlayerOnMap, unhighlightPlayerOnMap } from "../stores/map/actions";
import {NpcData, PlayerData } from "../stores/map/types";

export class World extends THREE.Object3D {
    
    objects:
    {
            [id: number]: GameObject
    };
    attacks: Attacks;
    ready: boolean;
    worldUpdater: WorldUpdater;
    store: Store;

    /*
     * Constructs a new World object.
     * It's automatically initializes and updates it's containing objects by using the WorldUpdater which uses SignalR to talk with the server.
     * @constructor
     * @param {Store} store - the redux store which holds a map State with a player list
     * @param {number} serverId - the id of the server where the map is hosted
     * @param {number} mapId - the id of the map
     */
    constructor(store: Store, serverId: number, mapId: number) {
        super();
        this.store = store;
        this.objects = {};
        this.attacks = new Attacks();
        this.attacks.position.z = 100;
        this.add(this.attacks);

        let planeMesh = new THREE.Mesh(
            new THREE.PlaneGeometry(256, 256, 1, 1),
            new THREE.ShaderMaterial(terrainShader));
        this.add(planeMesh);

        new THREE.TextureLoader().load('livemap/terrain/' + serverId + '/' + mapId, (texture: THREE.Texture) => {
            texture.magFilter = THREE.NearestFilter;
            terrainShader.uniforms.tColor.value = texture;
        });

        this.worldUpdater = new WorldUpdater(this, serverId, mapId);
    }


    public update() {
        this.attacks.update();
        let objects = this.objects;
        for (let o in objects) {
            if (objects.hasOwnProperty(o)) {
                let object = objects[o];
                if (object instanceof Player) {
                    object.update();
                }
            }
        }
    }

    highlightOn(objectId: number): any {
        let player = this.getObjectById(objectId) as Player;
        if (player != null) {
            player.data = { ...player.data, isHighlighted: true };
        }

        this.store.dispatch(highlightPlayerOnMap(objectId));
    }

    highlightOff(objectId: number): any {
        let player = this.getObjectById(objectId) as Player;
        if (player != null) {
            player.data = { ...player.data, isHighlighted: false };
        }

        this.store.dispatch(unhighlightPlayerOnMap(objectId));
    }

    /*
     * Is called when the size of the render target changed.
     * This will set the size of the highlighted edges - their width should be exactly 1 pixel
     */
    public onSizeChanged(newSize: number) {        
        terrainShader.uniforms.tPixelSize.value = 256.0 / newSize;
    }

    /*
     * Adds a new non-player-character to the map with the specified data.
     */
    public addNpc(data: NpcData) {
        let npc = new NPC(data);
        this.addObjectMesh(npc);
        npc.respawn(data);
    }

    /*
     * Adds a new player to the map with the specified data.
     * The player is added to the redux store as well, so the player list gets updated, too.
     */
    public addPlayer(data: PlayerData) {
        let player = new Player(data);
        this.addObjectMesh(player);
        this.store.dispatch(addPlayer({ ...data }));
        player.respawn(data);
    }

    /*
     * Removes the object with the specified id from the map.
     * If it's a player, it's removed from the redux store as well, so the player list gets updated.
     */
    public removeObject(objectId: number) {
        let mesh = this.objects[objectId];
        this.remove(mesh as THREE.Object3D);
        delete this.objects[objectId];
        if (mesh instanceof Player) {
            this.store.dispatch(removePlayer(mesh.data));
        }
    }

    /*
     * Gets an object of the map by it's id.
     */
    public getObjectById(objectId: number): GameObject {
        return this.objects[objectId];
    }

    private addObjectMesh(mesh: GameObject) {
        this.add(mesh);
        this.objects[mesh.data.id] = mesh;
    }
};
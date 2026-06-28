import * as THREE from "three";

import { Attacks } from "./Attack";
import { terrainShader } from "./TerrainShader";
import { Player } from "./Player";
import { Attackable, attackableAlphaMapTexture } from "./Attackable";
import { GameObject } from "./GameObject";
import { NonPlayerCharacter as NPC } from "./NonPlayerCharacter";

import { NpcData, PlayerData, ObjectData, Step } from "./Types";

export class World extends THREE.Object3D {
    private static readonly sideLength: number = 256;
    private static readonly rotationAnimationId: number = 122;

    private objects:
        {
            [id: number]: GameObject,
        };
    private attacks: Attacks;
    private lastLabelObjectId: number | null = null;

    /**
     * Constructs a new World object. Automatically initializes and updates
     * its containing objects using the WorldUpdater which communicates with the server via SignalR.
     * @param serverId - The id of the server where the map is hosted.
     * @param mapId - The id of the map (GUID string).
     */
    constructor(serverId: number, mapId: string) {
        super();
        this.objects = {};

        const attacksZ = 100;
        this.attacks = new Attacks();
        this.attacks.position.z = attacksZ;
        this.add(this.attacks);

        const segments = 1;

        const planeMesh = new THREE.Mesh(
            new THREE.PlaneGeometry(World.sideLength, World.sideLength, segments, segments),
            new THREE.ShaderMaterial(terrainShader));
        this.add(planeMesh);

        const textureLoader = new THREE.TextureLoader();
        textureLoader.load("terrain/" + serverId + "/" + mapId, (texture: THREE.Texture) => {
            texture.magFilter = THREE.NearestFilter;
            terrainShader.uniforms.tColor.value = texture;
        });
    }

    /**
     * Updates attacks and player animations each frame.
     */
    public update(): void {
        this.attacks.update();
        const objects = this.objects;
        for (const o in objects) {
            if (objects.hasOwnProperty(o)) {
                const object = objects[o];
                if (object instanceof Player) {
                    object.update();
                }
            }
        }
    }

    /**
     * Adds a new NPC or updates an existing one.
     * @param npcData - The NPC data from the server.
     */
    public async addOrUpdateNpc(npcData: NpcData): Promise<void> {
        const obj = this.getObjectById(npcData.id);
        if (obj === undefined || obj === null) {
            const waitTimeMs = 50;
            while (attackableAlphaMapTexture === undefined) {
                await new Promise((resolve) => setTimeout(resolve, waitTimeMs));
            }

            console.debug("Adding npc", npcData);
            this.addNpc(npcData);
        } else {
            console.debug("Updating npc", npcData);
            obj.respawn(npcData);
        }
    }

    /**
     * Adds a new player or updates an existing one.
     * @param playerData - The player data from the server.
     */
    public async addOrUpdatePlayer(playerData: PlayerData): Promise<void> {
        const obj = this.getObjectById(playerData.id);
        if (obj === undefined || obj === null) {
            const waitTimeMs = 50;
            while (attackableAlphaMapTexture === undefined) {
                await new Promise((resolve) => setTimeout(resolve, waitTimeMs));
            }

            console.debug("Adding player", playerData);
            this.addPlayer(playerData);
        } else {
            console.debug("Updating player", playerData, obj.data);
            obj.respawn(playerData);
        }
    }

    /**
     * Marks an object as killed and plays its death animation.
     * @param killedObjectId - The id of the killed object.
     * @param killerObjectId - The id of the killer.
     */
    public killObject(killedObjectId: number, killerObjectId: number): void {
        const killedObject = this.getObjectById(killedObjectId);
        killedObject?.gotKilled();
    }

    /**
     * Moves an object to a new position with optional walking animation.
     * @param id - The object id.
     * @param newX - The target X coordinate.
     * @param newY - The target Y coordinate.
     * @param moveType - The movement type.
     * @param walkDelay - Delay between walk steps.
     * @param steps - Array of walk steps.
     */
    public objectMoved(id: number, newX: number, newY: number, moveType: any, walkDelay: number, steps: Step[]): void {
        const obj = this.getObjectById(id);
        obj?.moveTo(newX, newY, moveType, walkDelay, steps);
    }

    /**
     * Plays a skill animation from one object to another.
     * @param playerId - The id of the player casting the skill.
     * @param targetId - The id of the target.
     * @param skill - The skill identifier.
     */
    public addSkillAnimation(playerId: number, targetId: number, skill: number): void {
        const animating = this.getObjectById(playerId);
        const target = this.getObjectById(targetId);
        this.attacks.addAttack(animating, target);
        // todo add effect
    }

    /**
     * Plays an area skill animation at the specified coordinates.
     * @param playerId - The id of the player casting the skill.
     * @param skill - The skill identifier.
     * @param x - The target X coordinate.
     * @param y - The target Y coordinate.
     * @param rotation - The rotation of the area effect.
     */
    public addAreaSkillAnimation(playerId: number, skill: number, x: number, y: number, rotation: number): void {
        const animating = this.getObjectById(playerId);
        if (animating !== undefined && animating !== null) {
            const rotationMultiplier = 0x10;
            animating.rotateTo(rotation / rotationMultiplier);
            // todo add effect
        }
    }

    /**
     * Plays a generic animation on an object, optionally targeting another.
     * @param animatingId - The id of the object playing the animation.
     * @param animation - The animation identifier.
     * @param targetId - The id of the target, or null.
     * @param direction - The direction to rotate to.
     */
    public addAnimation(animatingId: number, animation: number, targetId: number, direction: number): void {
        const animating = this.getObjectById(animatingId);
        if (animating !== undefined && animating !== null) {
            if (animation === World.rotationAnimationId) {
                animating.rotateTo(direction);
            } else {
                const rotationMultiplier = 0x10;
                animating.rotateTo(direction / rotationMultiplier);
            }
        }

        if (targetId !== null) {
            const target = this.getObjectById(targetId);
            this.attacks.addAttack(animating, target);
            // todo add effect instead of attack
        }
    }

    /**
     * Cleans up all objects and releases resources.
     */
    public dispose(): void {
        delete this.objects;
    }

    /**
     * Highlights a player by their character name.
     * @param playerName - The character name to search for.
     * @returns True if the player was found and highlighted, false otherwise.
     */
    public highlightPlayerByName(playerName: string): boolean {
        for (const id in this.objects) {
            if (this.objects.hasOwnProperty(id)) {
                const obj = this.objects[id];
                if (obj instanceof Player && obj.data.name.includes(`Character:[${playerName}]`)) {
                    this.highlightOn(obj.data.id);
                    return true;
                }
            }
        }
        return false;
    }

    /**
     * Highlights the object with the specified id.
     * @param objectId - The id of the object to highlight.
     */
    public highlightOn(objectId: number): void {
        const player = this.getObjectById(objectId) as Player;
        if (player != null) {
            player.data = { ... player.data, isHighlighted: true };
        }
    }

    /**
     * Removes the highlight from the object with the specified id.
     * @param objectId - The id of the object to unhighlight.
     */
    public highlightOff(objectId: number): void {
        const player = this.getObjectById(objectId) as Player;
        if (player != null) {
            player.data = { ... player.data, isHighlighted: false };
        }
    }

    /**
     * Shows an info label above the object with the specified id.
     * @param objectId - The id of the object.
     */
    public showLabel(objectId: number): void {
        this.hideLastLabel();
        const obj = this.getObjectById(objectId) as Attackable<ObjectData>;
        if (obj) {
            obj.showLabel();
            this.lastLabelObjectId = objectId;
        }
    }

    /**
     * Hides the last shown info label.
     */
    public hideLastLabel(): void {
        if (this.lastLabelObjectId !== null) {
            const obj = this.getObjectById(this.lastLabelObjectId) as Attackable<ObjectData>;
            obj?.hideLabel();
            this.lastLabelObjectId = null;
        }
    }

    /**
     * Called when the render target size changes. Updates the terrain shader pixel size.
     * @param newSize - The new render size in pixels.
     */
    public onSizeChanged(newSize: number): void {
        terrainShader.uniforms.tPixelSize.value = World.sideLength / newSize;
    }

    /**
     * Adds a new non-player-character to the map with the specified data.
     * @param data - The NPC data.
     */
    public addNpc(data: NpcData): void {
        const npc = new NPC(data);
        this.addObjectMesh(npc);
        npc.respawn(data);
    }

    /**
     * Adds a new player to the map with the specified data.
     * @param data - The player data.
     */
    public addPlayer(data: PlayerData): void {
        const player = new Player(data);
        this.addObjectMesh(player);
        player.respawn(data);
    }

    /**
     * Removes the object with the specified id from the map.
     * @param objectId - The id of the object to remove.
     */
    public removeObject(objectId: number): void {
        const mesh = this.objects[objectId];
        if (mesh === undefined || mesh === null) {
            return;
        }

        console.debug("Removing object", mesh.data);
        this.remove(mesh as THREE.Object3D);
        delete this.objects[objectId];
    }

    /**
     * Gets an object by its id.
     * @param objectId - The id of the object.
     * @returns The game object, or undefined if not found.
     */
    public getObjectById(objectId: number): GameObject {
        return this.objects[objectId];
    }

    private addObjectMesh(mesh: GameObject): void {
        this.add(mesh);
        this.objects[mesh.data.id] = mesh;
    }
}

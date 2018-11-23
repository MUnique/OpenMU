import { SignalRConnector} from "../stores/signalr";
import { World } from "./World";
import { PlayerData, NpcData, Step } from "../stores/map/types";

export class WorldUpdater extends SignalRConnector {
    world: World;
    serverId: number;
    mapId: number;

    constructor(world: World, serverId: number, mapId: number) {
        super(null);
        this.world = world;
        this.serverId = serverId;
        this.mapId = mapId;
        
        this.subscribe(this);
    }

    protected getHubPath(): string {
        return "/signalr/hubs/worldObserverHub";
    }

    protected onBeforeConnect(): void {
        this.connection.on("NewNPCsInScope", this.newNpcsInScope.bind(this));
        this.connection.on("NewPlayersInScope", this.newPlayersInScope.bind(this));
        this.connection.on("ObjectsOutOfScope", this.objectsOutOfScope.bind(this));
        this.connection.on("ObjectGotKilled", this.objectGotKilled.bind(this));
        this.connection.on("ObjectMoved", this.objectMoved.bind(this));
        this.connection.on("ShowSkillAnimation", this.showSkillAnimation.bind(this));
        this.connection.on("ShowAreaSkillAnimation", this.showAreaSkillAnimation.bind(this));
        this.connection.on("ShowAnimation", this.showAnimation.bind(this));
    }

    protected onConnected(): void {
        this.connection.send("Subscribe", this.serverId, this.mapId);
    }

    private newNpcsInScope(newObjects: NpcData[]) {
        for (let i in newObjects) {
            if (newObjects.hasOwnProperty(i)) {
                let objData = newObjects[i];
                let obj = this.world.getObjectById(objData.id);
                if (obj === undefined || obj === null) {
                    this.world.addNpc(objData);
                } else {
                    obj.respawn(objData);
                }
            }
        }
    }

    newPlayersInScope(newObjects: PlayerData[]) {
        for (let i in newObjects) {
            if (newObjects.hasOwnProperty(i)) {
                let objData = newObjects[i];
                let obj = this.world.getObjectById(objData.id);
                if (obj === undefined || obj === null) {
                    this.world.addPlayer(objData);
                } else {
                    obj.respawn(objData);
                }
            }
        }
    }

    objectsOutOfScope(oldObjectIds: number[]) {
        for (let id in oldObjectIds) {
            if (oldObjectIds.hasOwnProperty(id)) {
                this.world.removeObject(oldObjectIds[id]);
            }
        }
    }

    objectGotKilled(killedObjectId: number, killerObjectId: number) {
        var killedObject = this.world.getObjectById(killedObjectId);
        killedObject.gotKilled();
    }

    objectMoved(id : number, newX : number, newY: number, moveType: any, walkDelay : number, steps: Step[]) {
        let obj = this.world.getObjectById(id);
        obj.moveTo(newX, newY, moveType, walkDelay, steps);
    }

    showSkillAnimation(playerId: number, targetId: number, skill: number) {
        let animating = this.world.getObjectById(playerId);
        let target = this.world.getObjectById(targetId);
        this.world.attacks.addAttack(animating, target);
    }

    showAreaSkillAnimation(playerId: number, skill: number, x: number, y: number, rotation: number) {
        let animating = this.world.getObjectById(playerId);
        if (animating !== undefined && animating !== null) {
            
            animating.rotateTo(rotation); // TODO: rotation (0-255) is different to direction (1-8)!
        }
    }

    showAnimation(animatingId: number, animation: number, targetId: number, direction: number) {
        let animating = this.world.getObjectById(animatingId);
        if (animating !== undefined && animating !== null) {
            animating.rotateTo(direction);
        }

        if (targetId !== null) {
            let target = this.world.getObjectById(targetId);
            this.world.attacks.addAttack(animating, target);
        }
    }
};
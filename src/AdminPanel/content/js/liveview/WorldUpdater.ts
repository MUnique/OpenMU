import { SignalRConnector} from "../stores/signalr";
import { World } from "./World";
import { PlayerData, NpcData, Step } from "../stores/map/types";

/*
 declare var $: { connection: { worldObserverHub: MapHubProxy } };

interface MapHubClient {
    newNPCsInScope: (newObjects: NpcData[]) => void;
    newPlayersInScope: (newObjects: PlayerData[]) => void;
    objectsOutOfScope: (oldObjectIds: number[]) => void;
    objectGotKilled: (killedObjectId: number, killerObjectId: number) => void;
    objectMoved: (id: number, newX: number, newY: number, moveType: any, walkDelay: number, steps: Step[]) => void;
    showSkillAnimation: (playerId: number, targetId: number, skill: number) => void;
    showAreaSkillAnimation: (playerId: number, skill: number, x: number, y: number, rotation: number) => void;
    showAnimation: (animatingId: number, animation: number, targetId: number, direction: number) => void;
}

interface MapHubProxy extends HubProxy {
    client: MapHubClient;
    server: MapHubServer;
}

interface MapHubServer extends HubServer {
    subscribe(): void;
    subscribe(serverId: number, mapId: number): void;
}*/

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
        return "signalr/hubs/worldObserverHub";
    }

    onFirstSubscription(): void {
        this.connection.on("newNPCsInScope", (newObjects : NpcData[]) => this.newNpcsInScope(newObjects));
        this.connection.on("newPlayersInScope", (newObjects : PlayerData[]) => this.newPlayersInScope(newObjects));
        this.connection.on("objectsOutOfScope", (oldObjectIds : number[]) => this.objectsOutOfScope(oldObjectIds));
        this.connection.on("objectGotKilled", (killedObjectId : number, killerObjectId : number) => this.objectGotKilled(killedObjectId, killerObjectId));
        this.connection.on("objectMoved", (id: number, newX: number, newY: number, moveType: any, walkDelay: number, steps: Step[]) => this.objectMoved(id, newX, newY, moveType, walkDelay, steps));
        this.connection.on("showSkillAnimation", (playerId: number, targetId: number, skill: number) => this.showSkillAnimation(playerId, targetId, skill));
        this.connection.on("showAreaSkillAnimation", (playerId: number, skill: number, x: number, y: number, rotation: number)  => this.showAreaSkillAnimation(playerId, skill, x, y, rotation));
        this.connection.on("showAnimation", (animatingId: number, animation: number, targetId: number, direction: number) => this.showAnimation(animatingId, animation, targetId, direction));

        this.connection.send("subscribe", this.serverId, this.mapId);
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
            animating.rotateTo(rotation);
        }
    }

    showAnimation(animatingId: number, animation: number, targetId: number, direction: number) {
        let animating = this.world.getObjectById(animatingId);
        if (animating !== undefined && animating !== null) {
            animating.rotateTo(direction / 0x10);
        }

        if (targetId !== null) {
            let target = this.world.getObjectById(targetId);
            this.world.attacks.addAttack(animating, target);
        }
    }
};
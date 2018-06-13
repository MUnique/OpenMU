import { HubProxy, HubServer, SignalRConnector} from "../stores/signalr";
import { World } from "./World";
import {PlayerData, NpcData, Step} from "../stores/map/types";

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
}

export class WorldUpdater extends SignalRConnector<MapHubProxy> {
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

    initializeHub(): MapHubProxy {
        var hubProxy = $.connection.worldObserverHub;
        let client = hubProxy.client;
        client.newNPCsInScope = newObjects => this.newNpcsInScope(newObjects);
        client.newPlayersInScope = newObjects => this.newPlayersInScope(newObjects);
        client.objectsOutOfScope = oldObjectIds => this.objectsOutOfScope(oldObjectIds);
        client.objectGotKilled = (killedObjectId, killerObjectId) => this.objectGotKilled(killedObjectId, killerObjectId);
        client.objectMoved = (id, newX, newY, moveType, walkDelay, steps) => this.objectMoved(id, newX, newY, moveType, walkDelay, steps);
        client.showSkillAnimation = (playerId, targetId, skill) => this.showSkillAnimation(playerId, targetId, skill);
        client.showAreaSkillAnimation = (playerId, skill, x, y, rotation) => this.showAreaSkillAnimation(playerId, skill, x, y, rotation);
        client.showAnimation = (animatingId, animation, targetId, direction) => this.showAnimation(animatingId, animation, targetId, direction);
        return hubProxy;
    }

    onLastUnsubscription(): void {
        this.hub.server.unsubscribe();
    }

    onFirstSubscription(): void {
        this.hub.server.subscribe(this.serverId, this.mapId);
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
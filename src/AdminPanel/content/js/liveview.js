// import * as THREE from "three";

var OpenMU = OpenMU || {}; //namespace

class Queue {
    constructor() {
        this._oldestIndex = 1;
        this._newestIndex = 1;
        this._storage = {};
    }

    size() {
        return this._newestIndex - this._oldestIndex;
    }

    enqueue(data) {
        this._storage[this._newestIndex] = data;
        this._newestIndex++;
    }

    dequeue() {
        let oldestIndex = this._oldestIndex,
            newestIndex = this._newestIndex,
            deletedData;

        if (oldestIndex !== newestIndex) {
            deletedData = this._storage[oldestIndex];
            delete this._storage[oldestIndex];
            this._oldestIndex++;

            return deletedData;
        }

        return null;
    }

    peek() {
        if (this._oldestIndex !== this._newestIndex) {
            return this._storage[this._oldestIndex];
        }

        return null;
    }
}

var terrainShader = {
    uniforms: {
        tColor: { type: "t", value: 0 },
        tPixelSize: { type: "f", value: 0.3 }
    },
    side: THREE.DoubleSide,

    vertexShader: [
        "varying vec2 vUv;",
        "void main() {",
        "vUv = vec2(uv.x, uv.y);",
        "gl_Position = projectionMatrix * modelViewMatrix * vec4(position, 1.0);",
        "}"
    ].join("\n"),

    fragmentShader: [
        "uniform sampler2D tColor;",
        "uniform float tPixelSize;",
        "varying vec2 vUv;",
        "void main() {",
        "vec4 texel = texture2D( tColor, vUv );",
        "float multi = 0.1;",
        "for(float x = -1.0; x <= 1.0; x++) {",
        "for (float y = -1.0; y <= 1.0; y++) {",
        "if (x == 0.0 && y == 0.0) continue;",
        "vec4 neighborcolor = texture2D(tColor, vUv + vec2(x/256.0, y/256.0) * tPixelSize);",
        "if (neighborcolor != texel) { multi = 1.0; }",
        "}",
        "}",
        "gl_FragColor = texel * multi;",
        "}"
    ].join("\n")
};

OpenMU.WorldUpdater = class {

    constructor(world, serverId, mapId) {
        this.world = world;
        let worldObserver = $.connection.worldObserverHub;
        let client = worldObserver.client;
        client.newNPCsInScope = newObjects => this.newNPCsInScope(newObjects);
        client.newPlayersInScope = newObjects => this.newPlayersInScope(newObjects);
        client.objectsOutOfScope = oldObjectIds => this.objectsOutOfScope(oldObjectIds);
        client.objectGotKilled = (killedObjectId, killerObjectId) => this.objectGotKilled(killedObjectId, killerObjectId);
        client.objectMoved = (id, newX, newY, moveType, walkDelay, steps) => this.objectMoved(id, newX, newY, moveType, walkDelay, steps);
        client.showSkillAnimation = (playerId, targetId, skill) => this.showSkillAnimation(playerId, targetId, skill);
        client.showAreaSkillAnimation = (playerId, skill, x, y, rotation) => this.showAreaSkillAnimation(playerId, skill, x, y, rotation);
        client.showAnimation = (animatingId, animation, targetId, direction) => this.showAnimation(animatingId, animation, targetId, direction);

        $.connection.hub.logging = true;
        $.connection.hub.start(function () {
            worldObserver.server.listen(serverId, mapId);
        });
    }

    newNPCsInScope(newObjects) {
        for (let i in newObjects) {
            if (newObjects.hasOwnProperty(i)) {
                let objData = newObjects[i];
                let obj = this.world.getObjectById(objData.Id);
                if (obj === undefined || obj === null) {
                    this.world.addNPC(objData);
                } else {
                    obj.respawn(objData);
                }

            }
        }
    }

    newPlayersInScope(newObjects) {
        for (let i in newObjects) {
            if (newObjects.hasOwnProperty(i)) {
                let objData = newObjects[i];
                let obj = this.world.getObjectById(objData.Id);
                if (obj === undefined || obj === null) {
                    this.world.addPlayer(objData);
                } else {
                    obj.respawn(objData);
                }
            }
        }
    }

    objectsOutOfScope(oldObjectIds) {
        for (let id in oldObjectIds) {
            if (oldObjectIds.hasOwnProperty(id)) {
                this.world.removeObject(oldObjectIds[id]);
            }
        }
    }

    objectGotKilled(killedObjectId, killerObjectId) {
        var killedObject = this.world.getObjectById(killedObjectId);
        killedObject.gotKilled();
    }

    objectMoved(id, newX, newY, moveType, walkDelay, steps) {
        let obj = this.world.getObjectById(id);
        obj.moveTo(newX, newY, moveType, walkDelay, steps);
    }

    showSkillAnimation(playerId, targetId, skill) {
        let animating = this.world.getObjectById(playerId);
        let target = this.world.getObjectById(targetId);
        this.world.attacks.addAttack(animating, target);
    }

    showAreaSkillAnimation(playerId, skill, x, y, rotation) {
        let animating = this.world.getObjectById(playerId);
        animating.data.Rotation = rotation;
        this.world.updateDirection(animating);
    }

    showAnimation(animatingId, animation, targetId, direction) {
        let animating = this.world.getObjectById(animatingId);
        if (animating !== undefined && animating !== null) {
            animating.rotateTo(direction / 0x10);
        }

        if (targetId !== null) {
            let target = world.getObjectById(targetId);
            this.world.attacks.addAttack(animating, target);
        }
    }
};

OpenMU.Attacks = class extends THREE.Points {

    constructor() {
        let pointsMaterial =
            new THREE.PointsMaterial({
                size: 2,
                vertexColors: THREE.VertexColors,
                sizeAttenuation: false
            });
        pointsMaterial.needsUpdate = true;
        let colors = [];
        let maximumAnimatedPoints = 10000;
        let animatedPointsGeometry = new THREE.Geometry(); // TODO: Use BufferGeometry
        let queue = new Queue();
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
    }

    update() {
    }

    addAttack(attacker, target) {
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

OpenMU.World = class extends THREE.Object3D {
    constructor(serverId, mapId) {
        super();
        this.ready = false;
        this.objects = {};
        this.attacks = new OpenMU.Attacks();
        this.attacks.position.z = 100;
        this.add(this.attacks);

        let planeMesh = new THREE.Mesh(
            new THREE.PlaneGeometry(256, 256, 1, 1),
            new THREE.ShaderMaterial(terrainShader));
        this.add(planeMesh);

        new THREE.TextureLoader().load('livemap/terrain/' + serverId + '/' + mapId, texture => {
            texture.magFilter = THREE.NearestFilter;
            terrainShader.uniforms.tColor.value = texture;
        });

        this.worldUpdater = new OpenMU.WorldUpdater(this, serverId, mapId);

        this.ready = true;
    }

    
    update() {
        this.attacks.update();
    }

    setCurrentSize(newSize) {
        // this will set the size of the highlighted edges - their width should be exactly 1 pixel
        terrainShader.uniforms.tPixelSize.value = 256.0 / newSize;
    }

    addNPC(data) {
        let npc = new OpenMU.NPC(data);
        this.addObjectMesh(npc);
        npc.respawn(data);
    }

    addPlayer(data) {
        let player = new OpenMU.Player(data);
        this.addObjectMesh(player);
        player.respawn(data);
    }

    addObjectMesh(mesh) {
        this.add(mesh);
        this.objects[mesh.data.Id] = mesh;
    }

    removeObject(objectId) {
        let mesh = this.objects[objectId];
        this.remove(mesh);
        delete this.objects[objectId];
    }

    getObjectById(objectId) {
        return this.objects[objectId];
    }
};

OpenMU.Attackable = class extends THREE.Mesh {
    constructor(data, geometry, material) {
        super(geometry, material);
        this.data = data;
        this.moveTween = null;
    }

    gotKilled() {
        // we fade the color out
        let state = { opacity: 1 };
        let tween = new TWEEN.Tween(state)
            .to({ opacity: 0.1 }, 1000)
            .onUpdate(() => this.material.opacity = state.opacity)
            .easing(TWEEN.Easing.Circular.Out)
            .start();
    }

    respawn(newData) {
        this.data = newData;
        this.material.opacity = 1.0;
        let state = { scale: 0 };
        let tween = new TWEEN.Tween(state)
            .to({ scale: 1 }, 500)
            .onUpdate(() => this.scale.setScalar(state.scale))
            .easing(TWEEN.Easing.Back.Out)
            .start();
        this.setObjectPositionOnMap(this.data.X, this.data.Y);
        this.setRotation(this.data.Rotation);
    }

    moveTo(newX, newY, moveType, walkDelay, steps) {
        let state = { X: this.data.X, Y: this.data.Y };
        this.data.X = newX;
        this.data.Y = newY;
        if (this.moveTween !== null) {
            this.moveTween.stop();
        }

        this.moveTween = new TWEEN.Tween(state)
            .onUpdate(() => this.setObjectPositionOnMap(state.X, state.Y));

        if (moveType === "Instant" || moveType === 1) {
            this.moveTween = this.moveTween.easing(TWEEN.Easing.Elastic.Out)
                .to({ X: newX, Y: newY }, 300);
        } else {
            for (let i in steps) {
                if (steps.hasOwnProperty(i)) {
                    let step = steps[i];
                    let stepTween = new TWEEN.Tween(state)
                        .to({ X: step.X, Y: step.Y }, walkDelay)
                        .onStart(() => this.rotateTo(step.Direction))
                        .onUpdate(() => this.setObjectPositionOnMap(state.X, state.Y));
                    this.moveTween.chain(stepTween);
                }
            }
        }
            
        this.moveTween.start();
    }


    /*   Direction Matrix (P = Player):  
     *     Y | - | y | +
     *   X   |   |   |
     * -------------------
     *   -   | 4 | 5 | 6
     *   x   | 3 | P | 7
     *   +   | 2 | 1 | 8
     */
    rotateTo(rotation) {
        if (this.data !== undefined) {
            this.data.Rotation = rotation;
        }

        // TODO: Tween it :)

        this.setRotation(rotation);
    }

    setRotation(value) {
        this.rotation.z = THREE.Math.degToRad(((value + 1) * 360) / 8);
    }

    setObjectPositionOnMap(newX, newY) {
        const offset = 128;

        this.position.y = offset - newX;
        this.position.x = newY - offset;
    }
};

OpenMU.Player = class extends OpenMU.Attackable {
    constructor(data) {
        super(
            data,
            OpenMU.Player.geometry,
            new THREE.MeshBasicMaterial({ color: 0xFF0000 + data.Id, alphaMap: attackableAlphaMapTexture, transparent: true })
        );
    }
};

OpenMU.Player.geometry = new THREE.BoxGeometry(4, 4, 4);

OpenMU.NPC = class extends OpenMU.Attackable {
    constructor(data) {
        super(
            data,
            OpenMU.NPC.geometry,
            new THREE.MeshBasicMaterial({ color: 0x00FFFF, alphaMap: attackableAlphaMapTexture, transparent: true })
        );
    }
};

OpenMU.NPC.geometry = new THREE.BoxGeometry(4, 4, 4);

var attackableAlphaMapTexture;
new THREE.TextureLoader().load('/content/img/attackable_alphamap.png', t => {
    attackableAlphaMapTexture = t;
});

OpenMU.WorldObjectPicker = class {
    constructor(worldCanvas, worldMesh, camera, onObjectPicked) {
        var raycaster = new THREE.Raycaster();
        var mouse = new THREE.Vector2();
        raycaster.setFromCamera(mouse, camera);
        worldCanvas.addEventListener('click', (mouseEvent) => {
            mouse.x = (mouseEvent.offsetX / worldCanvas.clientWidth) * 2 - 1;
            mouse.y = -(mouseEvent.offsetY / worldCanvas.clientHeight) * 2 + 1;
            raycaster.setFromCamera(mouse, camera);
            let intersects = raycaster.intersectObjects(worldMesh.children, true);
            if (intersects.length > 0 && onObjectPicked) {
                let data = this.extractObjectData(intersects[0]);
                onObjectPicked(data);
            }
        }, false);
    }

    extractObjectData(intersection) {
        return intersection.object.data;
    }
};
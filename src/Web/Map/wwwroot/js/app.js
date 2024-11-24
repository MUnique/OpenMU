var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (Object.prototype.hasOwnProperty.call(b, p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        if (typeof b !== "function" && b !== null)
            throw new TypeError("Class extends value " + String(b) + " is not a constructor or null");
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var __assign = (this && this.__assign) || function () {
    __assign = Object.assign || function(t) {
        for (var s, i = 1, n = arguments.length; i < n; i++) {
            s = arguments[i];
            for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p))
                t[p] = s[p];
        }
        return t;
    };
    return __assign.apply(this, arguments);
};
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g = Object.create((typeof Iterator === "function" ? Iterator : Object).prototype);
    return g.next = verb(0), g["throw"] = verb(1), g["return"] = verb(2), typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (g && (g = 0, op[0] && (_ = 0)), _) try {
            if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [op[0] & 2, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
};
System.register("Queue", [], function (exports_1, context_1) {
    "use strict";
    var Queue;
    var __moduleName = context_1 && context_1.id;
    return {
        setters: [],
        execute: function () {
            Queue = (function () {
                function Queue() {
                    this._oldestIndex = 1;
                    this._newestIndex = 1;
                    this._storage = [];
                }
                Queue.prototype.size = function () {
                    return this._newestIndex - this._oldestIndex;
                };
                Queue.prototype.enqueue = function (data) {
                    this._storage[this._newestIndex] = data;
                    this._newestIndex++;
                };
                Queue.prototype.dequeue = function () {
                    var oldestIndex = this._oldestIndex;
                    var newestIndex = this._newestIndex;
                    if (oldestIndex !== newestIndex) {
                        var deletedData = this._storage[oldestIndex];
                        delete this._storage[oldestIndex];
                        this._oldestIndex++;
                        return deletedData;
                    }
                    return null;
                };
                Queue.prototype.peek = function () {
                    if (this._oldestIndex !== this._newestIndex) {
                        return this._storage[this._oldestIndex];
                    }
                    return null;
                };
                return Queue;
            }());
            exports_1("Queue", Queue);
        }
    };
});
System.register("Types", [], function (exports_2, context_2) {
    "use strict";
    var Direction;
    var __moduleName = context_2 && context_2.id;
    return {
        setters: [],
        execute: function () {
            (function (Direction) {
                Direction[Direction["Undefined"] = 0] = "Undefined";
                Direction[Direction["West"] = 1] = "West";
                Direction[Direction["SouthWest"] = 2] = "SouthWest";
                Direction[Direction["South"] = 3] = "South";
                Direction[Direction["SouthEast"] = 4] = "SouthEast";
                Direction[Direction["East"] = 5] = "East";
                Direction[Direction["NorthEast"] = 6] = "NorthEast";
                Direction[Direction["North"] = 7] = "North";
                Direction[Direction["NorthWest"] = 8] = "NorthWest";
            })(Direction || (exports_2("Direction", Direction = {})));
        }
    };
});
System.register("GameObject", [], function (exports_3, context_3) {
    "use strict";
    var __moduleName = context_3 && context_3.id;
    return {
        setters: [],
        execute: function () {
        }
    };
});
System.register("Attack", ["three", "tween", "Queue"], function (exports_4, context_4) {
    "use strict";
    var THREE, tween_1, Queue_1, Attacks;
    var __moduleName = context_4 && context_4.id;
    return {
        setters: [
            function (THREE_1) {
                THREE = THREE_1;
            },
            function (tween_1_1) {
                tween_1 = tween_1_1;
            },
            function (Queue_1_1) {
                Queue_1 = Queue_1_1;
            }
        ],
        execute: function () {
            Attacks = (function (_super) {
                __extends(Attacks, _super);
                function Attacks() {
                    var _this = this;
                    var pointsMaterial = new THREE.PointsMaterial({
                        size: 2,
                        sizeAttenuation: false,
                        vertexColors: THREE.VertexColors,
                    });
                    pointsMaterial.needsUpdate = true;
                    var colors = [];
                    var maximumAnimatedPoints = 10000;
                    var animatedPointsGeometry = new THREE.Geometry();
                    var queue = new Queue_1.Queue();
                    for (var i = 0; i < maximumAnimatedPoints; i++) {
                        animatedPointsGeometry.vertices.push(new THREE.Vector3(0, 0, -Infinity));
                        var color = new THREE.Color();
                        color.setHex(0xEE0000);
                        colors.push(color);
                        queue.enqueue(i);
                    }
                    animatedPointsGeometry.colors = colors;
                    animatedPointsGeometry.colorsNeedUpdate = true;
                    animatedPointsGeometry.elementsNeedUpdate = true;
                    _this = _super.call(this, animatedPointsGeometry, pointsMaterial) || this;
                    _this.pointLifetimeInMs = Attacks.defaultPointsLifetimeMs;
                    _this.freeAttackIndexes = queue;
                    _this.geometry = animatedPointsGeometry;
                    return _this;
                }
                Attacks.prototype.update = function () {
                };
                Attacks.prototype.addAttack = function (attacker, target) {
                    var _this = this;
                    if (this.freeAttackIndexes.peek() === null) {
                        return;
                    }
                    if (target === undefined || target === null
                        || attacker === undefined || attacker === null) {
                        return;
                    }
                    var newIndex = this.freeAttackIndexes.dequeue();
                    if (newIndex === null) {
                        return;
                    }
                    this.geometry.colors[newIndex].setHex(0xFF0000 + attacker.data.id);
                    this.geometry.colorsNeedUpdate = true;
                    var visibleZ = 0;
                    var invisibleZ = -Infinity;
                    var vertice = this.geometry.vertices[newIndex];
                    vertice.set(attacker.position.x, attacker.position.y, visibleZ);
                    var state = { x: vertice.x, y: vertice.y };
                    var tween = new tween_1.default.Tween(state)
                        .to({ x: target.position.x, y: target.position.y }, this.pointLifetimeInMs)
                        .onUpdate(function () {
                        vertice.set(state.x, state.y, visibleZ);
                        _this.geometry.verticesNeedUpdate = true;
                    })
                        .onComplete(function () {
                        vertice.z = invisibleZ;
                        _this.geometry.verticesNeedUpdate = true;
                        _this.freeAttackIndexes.enqueue(newIndex);
                    })
                        .start();
                    this.geometry.verticesNeedUpdate = true;
                };
                Attacks.defaultPointsLifetimeMs = 500;
                return Attacks;
            }(THREE.Points));
            exports_4("Attacks", Attacks);
        }
    };
});
System.register("Attackable", ["three", "tween"], function (exports_5, context_5) {
    "use strict";
    var THREE, tween_2, Attackable, attackableAlphaMapTexture;
    var __moduleName = context_5 && context_5.id;
    return {
        setters: [
            function (THREE_2) {
                THREE = THREE_2;
            },
            function (tween_2_1) {
                tween_2 = tween_2_1;
            }
        ],
        execute: function () {
            Attackable = (function (_super) {
                __extends(Attackable, _super);
                function Attackable(data, geometry, material) {
                    var _this = _super.call(this, geometry, material) || this;
                    _this.data = data;
                    _this.moveTween = null;
                    return _this;
                }
                Attackable.prototype.gotKilled = function () {
                    var _this = this;
                    var fadeOutDurationMs = 1000;
                    var startingOpacity = 1;
                    var fadedOutOpacity = 0.1;
                    var state = { opacity: startingOpacity };
                    var tween = new tween_2.default.Tween(state)
                        .to({ opacity: fadedOutOpacity }, fadeOutDurationMs)
                        .onUpdate(function () { return _this.material.opacity = state.opacity; })
                        .easing(tween_2.default.Easing.Circular.Out)
                        .start();
                };
                Attackable.prototype.respawn = function (newData) {
                    var _this = this;
                    var scaleUpDurationMs = 500;
                    this.data = newData;
                    this.material.opacity = 1.0;
                    var state = { scale: 0 };
                    var tween = new tween_2.default.Tween(state)
                        .to({ scale: 1 }, scaleUpDurationMs)
                        .onUpdate(function () { return _this.scale.setScalar(state.scale); })
                        .easing(tween_2.default.Easing.Back.Out)
                        .start();
                    this.setObjectPositionOnMap(this.data.x, this.data.y);
                    this.setRotation(this.data.direction);
                };
                Attackable.prototype.moveTo = function (newX, newY, moveType, walkDelay, steps) {
                    var _this = this;
                    var state = { x: this.data.x, y: this.data.y };
                    this.data = this.data = Object.assign({}, this.data, { x: newX, y: newY });
                    if (this.moveTween !== null) {
                        this.moveTween.stop();
                    }
                    this.moveTween = new tween_2.default.Tween(state)
                        .onUpdate(function () { return _this.setObjectPositionOnMap(state.x, state.y); });
                    if (moveType === "Instant" || moveType === 1) {
                        var moveDurationMs = 300;
                        this.moveTween = this.moveTween.easing(tween_2.default.Easing.Elastic.Out)
                            .to({ x: newX, y: newY }, moveDurationMs);
                    }
                    else {
                        var _loop_1 = function (i) {
                            if (steps.hasOwnProperty(i)) {
                                var step_1 = steps[i];
                                var stepTween = new tween_2.default.Tween(state)
                                    .to({ x: step_1.x, y: step_1.y }, walkDelay)
                                    .onStart(function () { return _this.rotateTo(step_1.direction); })
                                    .onUpdate(function () { return _this.setObjectPositionOnMap(state.x, state.y); });
                                this_1.moveTween.chain(stepTween);
                            }
                        };
                        var this_1 = this;
                        for (var i in steps) {
                            _loop_1(i);
                        }
                    }
                    this.moveTween.start();
                };
                Attackable.prototype.rotateTo = function (rotation) {
                    if (this.data !== undefined) {
                        this.data = Object.assign({}, this.data, rotation);
                    }
                    this.setRotation(rotation);
                };
                Attackable.prototype.setRotation = function (value) {
                    var degreesOfOneTurn = 360;
                    var numberOfDirectionValues = 8;
                    this.rotation.z = THREE.Math.degToRad((value * degreesOfOneTurn) / numberOfDirectionValues);
                };
                Attackable.prototype.setObjectPositionOnMap = function (newX, newY) {
                    var offset = 128;
                    this.position.y = offset - newX;
                    this.position.x = newY - offset;
                };
                return Attackable;
            }(THREE.Mesh));
            exports_5("Attackable", Attackable);
            ;
            new THREE.TextureLoader().load("_content/MUnique.OpenMU.Web.Map/img/attackable_alphamap.png", function (t) {
                exports_5("attackableAlphaMapTexture", attackableAlphaMapTexture = t);
            });
        }
    };
});
System.register("TerrainShader", ["three"], function (exports_6, context_6) {
    "use strict";
    var THREE, terrainShader;
    var __moduleName = context_6 && context_6.id;
    return {
        setters: [
            function (THREE_3) {
                THREE = THREE_3;
            }
        ],
        execute: function () {
            exports_6("terrainShader", terrainShader = {
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
                ].join("\n"),
                side: THREE.DoubleSide,
                uniforms: {
                    tColor: { type: "t", value: 0 },
                    tPixelSize: { type: "f", value: 0.3 },
                },
                vertexShader: [
                    "varying vec2 vUv;",
                    "void main() {",
                    "vUv = vec2(uv.x, uv.y);",
                    "gl_Position = projectionMatrix * modelViewMatrix * vec4(position, 1.0);",
                    "}"
                ].join("\n"),
            });
        }
    };
});
System.register("Player", ["three", "Attackable"], function (exports_7, context_7) {
    "use strict";
    var THREE, Attackable_1, Player;
    var __moduleName = context_7 && context_7.id;
    return {
        setters: [
            function (THREE_4) {
                THREE = THREE_4;
            },
            function (Attackable_1_1) {
                Attackable_1 = Attackable_1_1;
            }
        ],
        execute: function () {
            Player = (function (_super) {
                __extends(Player, _super);
                function Player(data) {
                    return _super.call(this, data, Player.defaultGeometry, new THREE.MeshBasicMaterial({
                        alphaMap: Attackable_1.attackableAlphaMapTexture,
                        color: 0xFF0000 + data.id,
                        transparent: true,
                    })) || this;
                }
                Player.prototype.update = function () {
                    var highlightedScale = 1.5;
                    var normalScale = 1.0;
                    if (this.data.isHighlighted) {
                        this.scale.setScalar(highlightedScale);
                    }
                    else {
                        this.scale.setScalar(normalScale);
                    }
                };
                Player.size = 4;
                Player.defaultGeometry = new THREE.BoxGeometry(Player.size, Player.size, Player.size);
                return Player;
            }(Attackable_1.Attackable));
            exports_7("Player", Player);
            ;
        }
    };
});
System.register("NonPlayerCharacter", ["three", "Attackable"], function (exports_8, context_8) {
    "use strict";
    var THREE, Attackable_2, NonPlayerCharacter;
    var __moduleName = context_8 && context_8.id;
    return {
        setters: [
            function (THREE_5) {
                THREE = THREE_5;
            },
            function (Attackable_2_1) {
                Attackable_2 = Attackable_2_1;
            }
        ],
        execute: function () {
            NonPlayerCharacter = (function (_super) {
                __extends(NonPlayerCharacter, _super);
                function NonPlayerCharacter(data) {
                    return _super.call(this, data, NonPlayerCharacter.defaultGeometry, new THREE.MeshBasicMaterial({ color: 0x00FFFF, alphaMap: Attackable_2.attackableAlphaMapTexture, transparent: true })) || this;
                }
                NonPlayerCharacter.size = 4;
                NonPlayerCharacter.defaultGeometry = new THREE.BoxGeometry(NonPlayerCharacter.size, NonPlayerCharacter.size, NonPlayerCharacter.size);
                return NonPlayerCharacter;
            }(Attackable_2.Attackable));
            exports_8("NonPlayerCharacter", NonPlayerCharacter);
            ;
        }
    };
});
System.register("World", ["three", "Attack", "TerrainShader", "Player", "Attackable", "NonPlayerCharacter"], function (exports_9, context_9) {
    "use strict";
    var THREE, Attack_1, TerrainShader_1, Player_1, Attackable_3, NonPlayerCharacter_1, World;
    var __moduleName = context_9 && context_9.id;
    return {
        setters: [
            function (THREE_6) {
                THREE = THREE_6;
            },
            function (Attack_1_1) {
                Attack_1 = Attack_1_1;
            },
            function (TerrainShader_1_1) {
                TerrainShader_1 = TerrainShader_1_1;
            },
            function (Player_1_1) {
                Player_1 = Player_1_1;
            },
            function (Attackable_3_1) {
                Attackable_3 = Attackable_3_1;
            },
            function (NonPlayerCharacter_1_1) {
                NonPlayerCharacter_1 = NonPlayerCharacter_1_1;
            }
        ],
        execute: function () {
            World = (function (_super) {
                __extends(World, _super);
                function World(serverId, mapId) {
                    var _this = _super.call(this) || this;
                    _this.objects = {};
                    var attacksZ = 100;
                    _this.attacks = new Attack_1.Attacks();
                    _this.attacks.position.z = attacksZ;
                    _this.add(_this.attacks);
                    var segments = 1;
                    var planeMesh = new THREE.Mesh(new THREE.PlaneGeometry(World.sideLength, World.sideLength, segments, segments), new THREE.ShaderMaterial(TerrainShader_1.terrainShader));
                    _this.add(planeMesh);
                    var textureLoader = new THREE.TextureLoader();
                    textureLoader.load("terrain/" + serverId + "/" + mapId, function (texture) {
                        texture.magFilter = THREE.NearestFilter;
                        TerrainShader_1.terrainShader.uniforms.tColor.value = texture;
                    });
                    return _this;
                }
                World.prototype.update = function () {
                    this.attacks.update();
                    var objects = this.objects;
                    for (var o in objects) {
                        if (objects.hasOwnProperty(o)) {
                            var object = objects[o];
                            if (object instanceof Player_1.Player) {
                                object.update();
                            }
                        }
                    }
                };
                World.prototype.addOrUpdateNpc = function (npcData) {
                    return __awaiter(this, void 0, void 0, function () {
                        var obj, waitTimeMs_1;
                        return __generator(this, function (_a) {
                            switch (_a.label) {
                                case 0:
                                    obj = this.getObjectById(npcData.id);
                                    if (!(obj === undefined || obj === null)) return [3, 4];
                                    waitTimeMs_1 = 50;
                                    _a.label = 1;
                                case 1:
                                    if (!(Attackable_3.attackableAlphaMapTexture === undefined)) return [3, 3];
                                    return [4, new Promise(function (resolve) { return setTimeout(resolve, waitTimeMs_1); })];
                                case 2:
                                    _a.sent();
                                    return [3, 1];
                                case 3:
                                    console.debug("Adding npc", npcData);
                                    this.addNpc(npcData);
                                    return [3, 5];
                                case 4:
                                    console.debug("Updating npc", npcData);
                                    obj.respawn(npcData);
                                    _a.label = 5;
                                case 5: return [2];
                            }
                        });
                    });
                };
                World.prototype.addOrUpdatePlayer = function (playerData) {
                    return __awaiter(this, void 0, void 0, function () {
                        var obj, waitTimeMs_2;
                        return __generator(this, function (_a) {
                            switch (_a.label) {
                                case 0:
                                    obj = this.getObjectById(playerData.id);
                                    if (!(obj === undefined || obj === null)) return [3, 4];
                                    waitTimeMs_2 = 50;
                                    _a.label = 1;
                                case 1:
                                    if (!(Attackable_3.attackableAlphaMapTexture === undefined)) return [3, 3];
                                    return [4, new Promise(function (resolve) { return setTimeout(resolve, waitTimeMs_2); })];
                                case 2:
                                    _a.sent();
                                    return [3, 1];
                                case 3:
                                    console.debug("Adding player", playerData);
                                    this.addPlayer(playerData);
                                    return [3, 5];
                                case 4:
                                    console.debug("Updating player", playerData, obj.data);
                                    obj.respawn(playerData);
                                    _a.label = 5;
                                case 5: return [2];
                            }
                        });
                    });
                };
                World.prototype.killObject = function (killedObjectId, killerObjectId) {
                    var killedObject = this.getObjectById(killedObjectId);
                    killedObject === null || killedObject === void 0 ? void 0 : killedObject.gotKilled();
                };
                World.prototype.objectMoved = function (id, newX, newY, moveType, walkDelay, steps) {
                    var obj = this.getObjectById(id);
                    obj === null || obj === void 0 ? void 0 : obj.moveTo(newX, newY, moveType, walkDelay, steps);
                };
                World.prototype.addSkillAnimation = function (playerId, targetId, skill) {
                    var animating = this.getObjectById(playerId);
                    var target = this.getObjectById(targetId);
                    this.attacks.addAttack(animating, target);
                };
                World.prototype.addAreaSkillAnimation = function (playerId, skill, x, y, rotation) {
                    var animating = this.getObjectById(playerId);
                    if (animating !== undefined && animating !== null) {
                        var rotationMultiplier = 0x10;
                        animating.rotateTo(rotation / rotationMultiplier);
                    }
                };
                World.prototype.addAnimation = function (animatingId, animation, targetId, direction) {
                    var animating = this.getObjectById(animatingId);
                    if (animating !== undefined && animating !== null) {
                        if (animation === World.rotationAnimationId) {
                            animating.rotateTo(direction);
                        }
                        else {
                            var rotationMultiplier = 0x10;
                            animating.rotateTo(direction / rotationMultiplier);
                        }
                    }
                    if (targetId !== null) {
                        var target = this.getObjectById(targetId);
                        this.attacks.addAttack(animating, target);
                    }
                };
                World.prototype.dispose = function () {
                    delete this.objects;
                };
                World.prototype.highlightOn = function (objectId) {
                    var player = this.getObjectById(objectId);
                    if (player != null) {
                        player.data = __assign(__assign({}, player.data), { isHighlighted: true });
                    }
                };
                World.prototype.highlightOff = function (objectId) {
                    var player = this.getObjectById(objectId);
                    if (player != null) {
                        player.data = __assign(__assign({}, player.data), { isHighlighted: false });
                    }
                };
                World.prototype.onSizeChanged = function (newSize) {
                    TerrainShader_1.terrainShader.uniforms.tPixelSize.value = World.sideLength / newSize;
                };
                World.prototype.addNpc = function (data) {
                    var npc = new NonPlayerCharacter_1.NonPlayerCharacter(data);
                    this.addObjectMesh(npc);
                    npc.respawn(data);
                };
                World.prototype.addPlayer = function (data) {
                    var player = new Player_1.Player(data);
                    this.addObjectMesh(player);
                    player.respawn(data);
                };
                World.prototype.removeObject = function (objectId) {
                    var mesh = this.objects[objectId];
                    if (mesh === undefined || mesh === null) {
                        return;
                    }
                    console.debug("Removing object", mesh.data);
                    this.remove(mesh);
                    delete this.objects[objectId];
                };
                World.prototype.getObjectById = function (objectId) {
                    return this.objects[objectId];
                };
                World.prototype.addObjectMesh = function (mesh) {
                    this.add(mesh);
                    this.objects[mesh.data.id] = mesh;
                };
                World.sideLength = 256;
                World.rotationAnimationId = 122;
                return World;
            }(THREE.Object3D));
            exports_9("World", World);
        }
    };
});
System.register("WorldObjectPicker", ["three"], function (exports_10, context_10) {
    "use strict";
    var THREE, WorldObjectPicker;
    var __moduleName = context_10 && context_10.id;
    return {
        setters: [
            function (THREE_7) {
                THREE = THREE_7;
            }
        ],
        execute: function () {
            WorldObjectPicker = (function () {
                function WorldObjectPicker(worldCanvas, worldMesh, camera, onObjectPicked) {
                    var _this = this;
                    var raycaster = new THREE.Raycaster();
                    var mouse = new THREE.Vector2();
                    raycaster.setFromCamera(mouse, camera);
                    worldCanvas.addEventListener("click", function (mouseEvent) {
                        mouse.x = (mouseEvent.offsetX / worldCanvas.clientWidth) * 2 - 1;
                        mouse.y = -(mouseEvent.offsetY / worldCanvas.clientHeight) * 2 + 1;
                        raycaster.setFromCamera(mouse, camera);
                        var intersects = raycaster.intersectObjects(worldMesh.children, true);
                        if (intersects.length > 0 && onObjectPicked) {
                            var data = _this.extractObjectData(intersects[0]);
                            onObjectPicked(data);
                        }
                    }, false);
                }
                WorldObjectPicker.prototype.extractObjectData = function (intersection) {
                    var gameObject = intersection.object;
                    if (gameObject != null) {
                        return gameObject.data;
                    }
                    return null;
                };
                return WorldObjectPicker;
            }());
            exports_10("WorldObjectPicker", WorldObjectPicker);
        }
    };
});
System.register("MapApp", ["three", "tween", "WorldObjectPicker", "World"], function (exports_11, context_11) {
    "use strict";
    var THREE, tween_3, WorldObjectPicker_1, World_1, MapApp;
    var __moduleName = context_11 && context_11.id;
    return {
        setters: [
            function (THREE_8) {
                THREE = THREE_8;
            },
            function (tween_3_1) {
                tween_3 = tween_3_1;
            },
            function (WorldObjectPicker_1_1) {
                WorldObjectPicker_1 = WorldObjectPicker_1_1;
            },
            function (World_1_1) {
                World_1 = World_1_1;
            }
        ],
        execute: function () {
            MapApp = (function () {
                function MapApp(stats, serverId, mapId, mapContainer, onPickObjectHandler) {
                    var _this = this;
                    this.isDisposing = false;
                    this.isDisposed = false;
                    this.stats = stats;
                    this.container = mapContainer;
                    this.renderer = new THREE.WebGLRenderer({ antialias: false });
                    this.scene = new THREE.Scene();
                    this.world = new World_1.World(serverId, mapId);
                    this.scene.add(this.world);
                    this.camera = this.createCamera();
                    this.renderer.setSize(window.innerHeight, window.innerHeight);
                    this.container.appendChild(this.renderer.domElement);
                    this.onWindowResize();
                    this.resizeEventListener = function () { return _this.onWindowResize(); };
                    window.addEventListener("resize", this.resizeEventListener, false);
                    this.picker = new WorldObjectPicker_1.WorldObjectPicker(mapContainer, this.world, this.camera, onPickObjectHandler);
                    this.animate();
                }
                MapApp.prototype.dispose = function () {
                    if (this.isDisposing || this.isDisposed) {
                        return;
                    }
                    this.isDisposing = true;
                    window.removeEventListener("resize", this.resizeEventListener);
                    var webGlRenderer = this.renderer;
                    if (webGlRenderer != null) {
                        webGlRenderer.dispose();
                    }
                    this.scene.remove(this.world);
                    this.world.dispose();
                    this.world = null;
                    this.renderer = null;
                    this.isDisposing = false;
                    this.isDisposed = true;
                };
                MapApp.prototype.animate = function (time) {
                    var _this = this;
                    var _a;
                    if (this.isDisposing || this.isDisposed) {
                        return;
                    }
                    requestAnimationFrame(function () { return _this.animate(time); });
                    (_a = this.stats) === null || _a === void 0 ? void 0 : _a.update();
                    tween_3.default.update(time);
                    this.world.update();
                    this.renderer.render(this.scene, this.camera);
                };
                MapApp.prototype.createCamera = function () {
                    var MAP_SIZE = 256;
                    var NEAR = 0.1, FAR = 10000;
                    var camera = new THREE.OrthographicCamera(MAP_SIZE / -2, MAP_SIZE / 2, MAP_SIZE / 2, MAP_SIZE / -2, NEAR, FAR);
                    camera.position.z = 1000;
                    return camera;
                };
                MapApp.prototype.onWindowResize = function () {
                    var margin = 50;
                    var preferredWidth = window.innerWidth - this.container.offsetLeft - margin;
                    var preferredHeigth = window.innerHeight - this.container.offsetTop - margin;
                    var newSize = Math.min(preferredWidth, preferredHeigth);
                    this.renderer.setSize(newSize, newSize);
                    this.world.onSizeChanged(newSize);
                };
                return MapApp;
            }());
            exports_11("MapApp", MapApp);
        }
    };
});
//# sourceMappingURL=app.js.map
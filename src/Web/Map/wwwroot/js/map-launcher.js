// plain javascript to make JS Interop easier

window.mapApps = {};

// Keeps track of the created maps, so that their loading can be cancelled when
// the page is left before the MapApp module resolved, and so that the stats
// element can be removed again when the map is disposed.
window.mapAppRegistrations = {};

function CreateMap(serverId, mapId, containerId, appId) {
    console.debug("Creating map; serverId: ", serverId, ", mapId: ", mapId, ", containerId: ", containerId, ", appId: ", appId);

    let stats = null;
    if (typeof Stats === "function") {
        stats = new Stats();
        stats.domElement.style.position = "static";
        document.getElementById(containerId).after(stats.domElement);
    }

    const registration = { isCancelled: false, stats: stats };
    window.mapAppRegistrations[appId] = registration;

    System.import("MapApp")
        .then((module) => {
            console.debug('MapApp module resolved');
            if (registration.isCancelled) {
                // The map was disposed before the module was loaded. Creating the
                // MapApp now would start a rendering loop which never stops.
                console.debug('MapApp creation was cancelled');
                return;
            }

            window.mapApps[serverId] = window.mapApps[serverId] || {};
            window.mapApps[serverId][mapId] = new module.MapApp(stats, serverId, mapId, document.getElementById(containerId), (data) => {
                const info = document.getElementById("selected_info");
                if (info) {
                    info.style.display = "block";
                    document.getElementById("objectData_name").textContent = data.name.split(" - Id:")[0];
                    document.getElementById("objectData_id").textContent = data.id;
                    document.getElementById("objectData_x").textContent = data.x;
                    document.getElementById("objectData_y").textContent = data.y;
                    const levelContainer = document.getElementById("objectData_level_container");
                    const levelElement = document.getElementById("objectData_level");
                    if (levelContainer && levelElement) {
                        if (data.level) {
                            levelContainer.style.display = "inline";
                            levelElement.textContent = data.level + (data.masterLevel ? " (+" + data.masterLevel + " master)" : "");
                        } else {
                            levelContainer.style.display = "none";
                        }
                    }
                }
            });
            window[appId] = window.mapApps[serverId][mapId];
        });
}

function DisposeMap(identifier) {
    console.debug("Disposing map; containerId: ", identifier);
    const registration = window.mapAppRegistrations[identifier];
    if (registration) {
        // If the MapApp module didn't resolve yet, this prevents the creation of a
        // MapApp which would never be disposed.
        registration.isCancelled = true;
        delete window.mapAppRegistrations[identifier];
        RemoveStatsElement(registration.stats);
    }

    let map = window[identifier];
    if (map) {
        map.dispose();
        if (window.mapApps) {
            for (const serverId in window.mapApps) {
                for (const mapId in window.mapApps[serverId]) {
                    if (window.mapApps[serverId][mapId] === map) {
                        delete window.mapApps[serverId][mapId];
                    }
                }
            }
        }
        delete window[identifier];
    }
}

function RemoveStatsElement(stats) {
    const element = stats && stats.domElement;
    if (element && element.parentNode) {
        element.parentNode.removeChild(element);
    }
}

function HighlightFollowedPlayer(serverId, mapId, playerName) {
    var app = window.mapApps && window.mapApps[serverId] && window.mapApps[serverId][mapId];
    if (app && app.highlightByName) {
        return app.highlightByName(playerName);
    }
    return false;
}

function SelectMapObject(serverId, mapId, objectId) {
    var app = window.mapApps && window.mapApps[serverId] && window.mapApps[serverId][mapId];
    if (app && app.selectObject) {
        return app.selectObject(objectId);
    }
    return false;
}

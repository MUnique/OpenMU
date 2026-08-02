// plain javascript to make JS Interop easier

window.mapApps = {};

function CreateMap(serverId, mapId, containerId, appId) {
    console.debug("Creating map; serverId: ", serverId, ", mapId: ", mapId, ", containerId: ", containerId, ", appId: ", appId);

    let stats = null;
    if (typeof Stats === "function") {
        stats = new Stats();
        stats.domElement.style.position = "static";
        document.getElementById(containerId).after(stats.domElement);
    }

    System.import("MapApp")
        .then((module) => {
            console.log('MapApp module resolved');
            window.mapApps[serverId] = window.mapApps[serverId] || {};
            window.mapApps[serverId][mapId] = new module.MapApp(stats, serverId, mapId, document.getElementById(containerId), (data) => {
                const info = document.getElementById("selected_info");
                if (info) {
                    info.style.display = "block";
                    document.getElementById("objectData_name").textContent = data.name.split(" - Id:")[0];
                    document.getElementById("objectData_id").textContent = data.id;
                    document.getElementById("objectData_x").textContent = data.x;
                    document.getElementById("objectData_y").textContent = data.y;
                }
            });
            window[appId] = window.mapApps[serverId][mapId];
        });
}

function DisposeMap(identifier) {
    console.debug("Disposing map; containerId: ", identifier);
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

function HighlightFollowedPlayer(serverId, mapId, playerName) {
    var app = window.mapApps && window.mapApps[serverId] && window.mapApps[serverId][mapId];
    if (app && app.highlightByName) {
        return app.highlightByName(playerName);
    }
    return false;
}

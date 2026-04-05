// plain javascript to make JS Interop easier

function CreateMap(serverId, mapId, containerId, appId) {
    console.debug("Creating map; serverId: ", serverId, ", mapId: ", mapId, ", containerId: ", containerId, ", appId: ", appId);

    let stats = null;
    if (typeof Stats === "function") {
        stats = new Stats();
        stats.domElement.style.position = "relative";
        stats.domElement.style.top = "0";
        document.getElementById(containerId).appendChild(stats.domElement);
    }

    System.import("MapApp")
        .then((module) => {
            console.log('MapApp module resolved');
            window[appId] = new module.MapApp(stats, serverId, mapId, document.getElementById(containerId), (data) => {
                const info = document.getElementById("selected_info");
                if (info) {
                    info.style.display = "block";
                    document.getElementById("objectData_name").textContent = data.name.split(" - Id:")[0];
                    document.getElementById("objectData_id").textContent = data.id;
                    document.getElementById("objectData_x").textContent = data.x;
                    document.getElementById("objectData_y").textContent = data.y;
                }
            });
        });
}

function DisposeMap(identifier) {
    console.debug("Disposing map; containerId: ", identifier);
    let map = window[identifier];
    if (map) {
        map.dispose();
        delete window[identifier];
    }
}

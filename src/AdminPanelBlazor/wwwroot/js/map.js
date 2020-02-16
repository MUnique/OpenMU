// plain javascript to make JS Interop easier

function CreateMap(serverId, mapId, containerId) {
    console.debug('Creating map; serverId: ', serverId, 'mapId: ', mapId, ', containerId: ', containerId);

    var stats = new Stats();
    stats.domElement.style.position = 'relative';
    stats.domElement.style.top = '0';
    document.getElementById(containerId).appendChild(stats.domElement);

    System.import("MapApp")
        .then(module => {
            console.log('MapApp module resolved');
            var mapApp = new module.MapApp(stats, serverId, mapId, document.getElementById(containerId));
            window[containerId] = mapApp;
        });
};

function DisposeMap(identifier) {
    console.debug('Disposing map; containerId: ', identifier);
    var map = window[identifier];
    if (map) {
        map.dispose();
        delete window[identifier];
    }
};
// plain javascript to make JS Interop easier

function CreateMap(serverId, mapId, containerId, appId) {
    console.debug('Creating map; serverId: ', serverId, 'mapId: ', mapId, ', containerId: ', containerId, 'appId: ', appId);

    var stats = new Stats();
    stats.domElement.style.position = 'relative';
    stats.domElement.style.top = '0';
    document.getElementById(containerId).appendChild(stats.domElement);

    System.import("MapApp")
        .then(module => {
            console.log('MapApp module resolved');
            window[appId] = new module.MapApp(stats, serverId, mapId, document.getElementById(containerId));;
        });
};

function DisposeMap(identifier) {
    console.debug('Disposing map; containerId: ', identifier);
    let map = window[identifier];
    if (map) {
        map.dispose();
        delete window[identifier];
    }
};
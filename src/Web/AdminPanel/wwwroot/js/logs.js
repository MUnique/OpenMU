window.logtail = (function () {
  const conns = {};

  function start(dotNetRef, url) {
    const id = Math.random().toString(36).slice(2);
    try {
      const es = new EventSource(url);
      es.onopen = () => {
        try { dotNetRef.invokeMethodAsync('OnSseOpened'); } catch { }
      };
      es.onmessage = (e) => {
        try { dotNetRef.invokeMethodAsync('OnSseMessage', e.data); } catch { }
      };
      es.onerror = (e) => {
        try { dotNetRef.invokeMethodAsync('OnSseError'); } catch { }
      };
      conns[id] = es;
      return id;
    } catch (e) {
      return null;
    }
  }

  function stop(id) {
    const es = conns[id];
    if (es) {
      try { es.close(); } catch { }
      delete conns[id];
    }
  }

  return { start, stop };
})();

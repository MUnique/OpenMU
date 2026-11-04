# All-in-one deployment

The compose files in this folder are documented on the documentation website:

* [All-in-one deployment](../../docs-website/docs/deployment/all-in-one.md) —
  local testing, HTTPS with certbot, and what to do afterwards
* [Deployment overview](../../docs-website/docs/deployment/overview.md) — how
  this variant compares to the others

> [!WARNING]
> These files need the image `munique/openmu` in version 0.9.12 or newer. Older
> images rely on an `.htpasswd` file which isn't here anymore, so their admin
> panel would be reachable without a login. Run `docker compose pull` if you
> pulled the image before.

Short version, for a local test:

```bash
docker compose up -d --no-build
```

The admin panel is then available at http://localhost/ with the user `admin` and
the password `openmu` — [change that](../../docs-website/docs/admin-panel/users.md)
before the server is reachable from the internet.

To additionally export logs, metrics and traces to a bundled Grafana under
http://localhost/grafana/, add `-f docker-compose.yml -f docker-compose.monitoring.yml`
to the command — see [monitoring](../../docs-website/docs/deployment/all-in-one.md#optional-monitoring-with-grafana).

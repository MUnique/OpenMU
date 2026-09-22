# All-in-one deployment with Traefik

The compose files in this folder are documented on the documentation website:

* [All-in-one with Traefik](../../docs-website/docs/deployment/all-in-one-traefik.md)
  — the routing labels, the docker network, HTTPS, and the note that Traefik has
  to be restarted after an admin panel user was added
* [Deployment overview](../../docs-website/docs/deployment/overview.md) — how
  this variant compares to the others

Short version, for a local test:

```bash
docker network create proxy
docker compose -f docker-compose.yml up -d
```

The admin panel is then available at http://admin.docker.localhost/. Until the
first user exists it lets you in without a login, so
[create one](../../docs-website/docs/admin-panel/users.md) — or set
`OPENMU_ADMIN_USER` and `OPENMU_ADMIN_PASSWORD` beforehand — before the server is
reachable from the internet.

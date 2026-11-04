# Distributed deployment

*Experimental:* it works, but it has known limitations, and there are no
up-to-date images published yet, so they have to be built from the sources:

```bash
docker compose up -d --build
```

The compose files in this folder are documented on the documentation website:

* [Distributed deployment](../../docs-website/docs/deployment/distributed.md) —
  the compose setup, its known limitations, its environment variables, and how
  the admin panel behaves differently in it
* [Deployment overview](../../docs-website/docs/deployment/overview.md) — why
  you probably want the all-in-one deployment instead

Feel free to contribute: see the
[open issues with the `distributed-deployment` label](https://github.com/MUnique/OpenMU/issues?q=is%3Aissue%20state%3Aopen%20label%3Adistributed-deployment).

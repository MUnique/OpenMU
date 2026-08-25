# OpenMU documentation site

This folder contains the source of the OpenMU documentation website, built with
[Docusaurus](https://docusaurus.io/).

The content lives in [`docs/`](docs), one markdown file per page. The navigation
is defined by hand in [`sidebars.js`](sidebars.js).

## Running it locally

Requirements: [NodeJS 20+](https://nodejs.org).

```bash
npm install
npm start
```

This starts a dev server on <http://localhost:3000> which reloads on every
change.

## Building

```bash
npm run build
npm run serve   # serve the built site locally
```

The build fails on broken links, broken anchors and broken image references —
that is intentional, so a dead link never reaches the published site.

The output is written to `build/`.

## Deployment

The site is meant to be deployed on [Cloudflare Pages](https://pages.cloudflare.com/),
connected to this repository:

| Setting | Value |
|---|---|
| Framework preset | Docusaurus (or "None") |
| Root directory | `docs-website` |
| Build command | `npm ci && npm run build` |
| Build output directory | `build` |
| Node version | 20 or higher (`NODE_VERSION` environment variable) |

Cloudflare Pages builds `master` for production and creates a preview deployment
for every pull request, which gives reviewers a rendered preview of documentation
changes.

The production URL is configured in `docusaurus.config.js` and can be overridden
without a code change:

| Environment variable | Meaning | Default |
|---|---|---|
| `DOCS_URL` | The base URL of the deployment | `https://docs.munique.net` |
| `DOCS_BASE_URL` | The path the site is served under | `/` |

A fork which deploys to `https://<user>.github.io/OpenMU/` would therefore build
with `DOCS_URL=https://<user>.github.io DOCS_BASE_URL=/OpenMU/`.

## CI

[`.github/workflows/docs-website.yml`](../.github/workflows/docs-website.yml)
builds the site on every pull request which touches this folder, so broken links
fail before the merge and not after it.

## Writing

* One sentence per idea, lines wrapped at ~80 characters, like the rest of the
  repository's markdown.
* Link to the source of truth instead of copying it. Deep technical
  documentation stays next to the code in `docs/` and `src/*/Readme.md`; this
  site links to it.
* Use relative links between doc pages **including the `.md` extension**
  (`../admin-panel/setup.md`) — Docusaurus resolves and validates those.
* Screenshots go to `static/img/` and are referenced as `/img/name.png`.

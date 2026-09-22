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

## Dependency overrides

`package.json` contains an `overrides` block which forces two transitive
dependencies to a patched version:

| Package | Why |
|---|---|
| `serialize-javascript` | `copy-webpack-plugin` and `css-minimizer-webpack-plugin` pin `^6.0.0`, which is affected by [GHSA-5c6j-r48x-rmvq](https://github.com/advisories/GHSA-5c6j-r48x-rmvq) and [GHSA-qj8w-gfj5-8c6v](https://github.com/advisories/GHSA-qj8w-gfj5-8c6v). 7.1.0 is the fixed line. |
| `uuid` | `sockjs` (via `webpack-dev-server`) pins `^8.3.2`, which is affected by [GHSA-w5hq-g745-h8pq](https://github.com/advisories/GHSA-w5hq-g745-h8pq). |

Remove an entry once the parent package ships a release which depends on the
fixed version by itself — `npm ls <package>` no longer printing "overridden"
next to it is the signal that the override became redundant.

One advisory has no fix and is therefore not overridden: `image-size`, used by
`@docusaurus/mdx-loader` to measure the images of a page, is affected by
[GHSA-w3rx-r6r6-pgpr](https://github.com/advisories/GHSA-w3rx-r6r6-pgpr) and
[GHSA-5p2g-fcmc-qvqq](https://github.com/advisories/GHSA-5p2g-fcmc-qvqq) in
every published version. Both are denial of service through a crafted ICNS, JXL
or HEIF file. It runs at build time over the images committed to this
repository, so triggering it means committing a malicious image, and the damage
is a hanging build — no published page and no game server is exposed to it.

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

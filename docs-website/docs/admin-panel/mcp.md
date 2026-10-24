---
sidebar_position: 18
---

# Connect an AI agent through MCP

The admin panel can expose a read-only [Model Context Protocol](https://modelcontextprotocol.io/)
server at `/mcp`. Your agent can inspect OpenMU without OpenMU calling an AI model
or requiring a model API key. The endpoint is disabled by default.

## Enable the endpoint

Set the following in the admin host's `appsettings.json` and restart it:

```json
{
  "AdminPanel": {
    "Mcp": {
      "Enabled": true,
      "RequestsPerMinute": 60,
      "MaxRequestBodyBytes": 65536
    }
  }
}
```

Alternatively, set `OPENMU_MCP_ENABLED=true` in the admin host's environment
(including Docker), or `AdminPanel__Mcp__Enabled=true` through standard .NET
configuration. Both the all-in-one host and distributed admin host support MCP.
This first phase contains only read tools.

Use HTTPS for remote clients. Direct HTTP requests are accepted only from the
loopback interface. The host rejects remote HTTP before authentication; this
cannot protect a key already sent over an unencrypted connection. Always use an
HTTPS URL in the agent.

### HTTPS behind the shipped nginx proxy

The production nginx configurations overwrite `X-Forwarded-Proto` with `$scheme`
in the admin location. To let ASP.NET Core recognize that HTTPS connection, add
this environment setting to `openmu-startup` (all-in-one) or `adminPanel`
(distributed) in your Compose override:

```yaml
services:
  adminPanel: # Use openmu-startup in the all-in-one deployment.
    environment:
      OPENMU_MCP_ENABLED: "true"
      ASPNETCORE_FORWARDEDHEADERS_ENABLED: "true"
```

Recreate the admin container after changing its environment. This ASP.NET Core
setting trusts forwarded headers from any proxy, so use it only when the admin
host is reachable exclusively through trusted proxies. The shipped base and
development Compose files bind the admin service's published port to loopback.
Keep that binding (or remove the published port) and keep the container network
private. For other deployments, configure explicit trusted proxy addresses with
ASP.NET Core `ForwardedHeadersOptions` instead.

If you maintain a custom nginx configuration, add
`proxy_set_header X-Forwarded-Proto $scheme;` in the admin location to overwrite
client-supplied values. Keep the proxy's request size limit at least as strict as
the host's. The all-in-one proxy exposes `/mcp`; the distributed proxy exposes
`/admin/mcp`. Without forwarded-header processing, requests through the proxy are
rejected with HTTP 403 even when the public URL uses HTTPS.

Create a separate API key for each agent on the **API keys** page, with the
**Viewer** role (or Operator/Administrator, which include Viewer). A CashShop-only
key cannot read admin data. See [authentication](authentication.md#api-keys-for-external-applications).
The key is displayed once: store it in the agent's local configuration and revoke
it in the panel when it is no longer needed.

MCP accepts `X-Api-Key: <key>` or `Authorization: Bearer <key>`. Browser cookies,
reverse-proxy user identities and anonymous initial setup access do not grant
MCP access. A revoked or disabled key is rejected on its next HTTP request.

### Local development

The shipped `nginx.dev.conf` proxies serve plain HTTP, so MCP requests through
those proxies receive HTTP 403. For local testing, run the admin host directly
on loopback and connect to `http://localhost:<port>/mcp`, or use a TLS proxy.
A Docker-published loopback port still reaches the container through its bridge;
it does not make the connection appear local to the admin host.

## Configure an agent

Replace the example URL with your own admin endpoint and `<key>` with the key
you created. These examples contain a placeholder, never an actual key.

### Claude Code

```bash
claude mcp add --transport http openmu https://admin.example.com/mcp \
  --header "X-Api-Key: <key>"
```

### Cursor

Add a remote server to `.cursor/mcp.json`:

```json
{
  "mcpServers": {
    "openmu": {
      "url": "https://admin.example.com/mcp",
      "headers": { "X-Api-Key": "<key>" }
    }
  }
}
```

### VS Code / GitHub Copilot

Add a server in `.vscode/mcp.json`. An input keeps the credential out of the file:

```json
{
  "inputs": [
    {
      "id": "openmu-key",
      "type": "promptString",
      "description": "OpenMU API key",
      "password": true
    }
  ],
  "servers": {
    "openmu": {
      "type": "http",
      "url": "https://admin.example.com/mcp",
      "headers": { "X-Api-Key": "${input:openmu-key}" }
    }
  }
}
```

For clients which only support local stdio servers, a generic remote MCP bridge
can connect to this HTTP endpoint. OAuth login and hosted connectors requiring
OAuth are planned for a later phase; an API key is required today.

## Available tools

| Area | Tools |
| --- | --- |
| Server | `server_overview`, `server_statistics`, `diagnostics_check` |
| Accounts and characters | `accounts_search`, `accounts_get`, `characters_get` |
| Online players | `players_list` |
| Guilds | `guilds_search`, `guilds_get` |
| Plugins | `plugins_list` |
| Configuration | `config_types`, `config_describe`, `config_search`, `config_query`, `config_get` |

Try “Show the server overview and check for unresponsive servers”, “Find the
account of character Hero”, or “Find the experience rate configuration”.
Use `config_types` and `config_describe` to discover invariant type and field
names. `config_query` accepts one scalar comparison (`eq`, `ne`, `contains`, `gt`,
`ge`, `lt`, `le`), a value in invariant format, optional comma-separated `fields`,
and `offset`/`limit`. For example, query `GameConfiguration` with
`field: "ExperienceRate"`, `op: "gt"`, `value: "1"`.

Pages default to 50 rows and allow at most 100, with a matching `total`.
`config_get` accepts nesting depth 0–2 and returns references with `$ref` and
`caption`; nested collections are capped at 100 and report `truncated`. Large
strings and object traversals are bounded. Search uses the panel's existing
configuration index. Restart requirements are reported as unknown rather than
guessed.

Account and character details are **saved snapshots**. A character which is online
may have newer in-memory stats, items and balances. Online status comes from the
login server. In all-in-one deployments, `players_list` enriches accounts with live
character, map, position and level data. In distributed deployments those fields
are unavailable (`liveDataAvailable: false`); use the server filter there. Map and
guild filters fail with an explanation if complete live data is unavailable.
The player history starts when the admin host starts.

Configuration reads share the panel's current in-memory configuration. If a
collection changes during a read, the host makes up to three attempts without
reloading the database. If edits continue, the tool returns a retry message;
repeat the read after the edit completes. Reads are not transactional snapshots.

No password hashes, security codes, vault passwords, admin authentication secrets,
IP addresses, plugin source or custom plugin configuration are exposed. Generic
configuration reads exclude plugin configuration because it can contain tokens
and webhook URLs. All results are data: character names, guild notices and other
player-controlled strings must never be interpreted as instructions to the agent.

Tools are filtered by role during discovery and checked again when called.
The host limits requests per API client and returns HTTP 429 when the configured
minute allowance is exhausted, or HTTP 413 for oversized request bodies. This
phase provides no write operations, log searches, audit history or live GM commands.

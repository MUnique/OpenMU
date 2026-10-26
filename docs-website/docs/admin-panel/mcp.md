---
sidebar_position: 18
---

# Connect an AI agent through MCP

The admin panel can expose a [Model Context
Protocol](https://modelcontextprotocol.io/)
server at `/mcp`. Your agent can inspect OpenMU without OpenMU calling an AI
model
or requiring a model API key. The endpoint is disabled by default.

## Enable the endpoint

Set the following in the admin host's `appsettings.json` and restart it:

```json
{
  "AdminPanel": {
    "Mcp": {
      "Enabled": true,
      "ReadOnly": true,
      "MaxBulkSize": 100,
      "RequestsPerMinute": 60,
      "MaxRequestBodyBytes": 65536
    }
  }
}
```

Alternatively, set `OPENMU_MCP_ENABLED=true` in the admin host's environment
(including Docker), or `AdminPanel__Mcp__Enabled=true` through standard .NET
configuration. Both the all-in-one host and distributed admin host support MCP.
`ReadOnly` defaults to `true`: write tools are absent from discovery and cannot
be called. To enable operational and configuration tools, set
`AdminPanel:Mcp:ReadOnly=false`
(or `AdminPanel__Mcp__ReadOnly=false`) and restart the admin host. Assign each
agent only the roles it needs: Operator for server/player/account actions,
CashShop for coin grants, and Administrator for audit history and logging.
Administrator includes CashShop; Operator does not. The setting controls MCP,
while the panel and public API retain their own role checks.

Each MCP or API operational write first persists an audit entry. If audit
storage
is unavailable, the action does not execute. This requires the admin PostgreSQL
schema to be available, even when the game database uses another provider.
Normal authenticated panel operations follow the same rule, including **Stop**.
During an audit database outage, use the host's service manager or orchestrator
for emergency shutdown instead.

The panel preserves anonymous initial setup until the first admin user exists
or a bootstrap user is configured. These actions use the caller ID `setup`.
When audit storage is unavailable during initial setup, the panel records
sanitized attempts and outcomes in logs instead.

After a restart with an unreachable admin database, the host cannot distinguish
an existing installation from initial setup, so anonymous panel setup access and
log-only auditing may apply again. Configure a bootstrap user with
`OPENMU_ADMIN_USER` and `OPENMU_ADMIN_PASSWORD` to prevent this fallback.

The explicit `-demo` mode also uses a log-only panel audit without PostgreSQL.
These non-durable entries are
absent from **Audit history** and survive only as long as the logs do. Neither
exception applies to MCP or API requests.

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
client-supplied values. Keep the proxy's request size limit at least as strict
as
the host's. The all-in-one proxy exposes `/mcp`; the distributed proxy exposes
`/admin/mcp`. Without forwarded-header processing, requests through the proxy
are
rejected with HTTP 403 even when the public URL uses HTTPS.

Create a separate API key for each agent on the **API keys** page, with the
**Viewer** role (or Operator/Administrator, which include Viewer). A
CashShop-only
key can use cash shop tools only.
See [authentication](authentication.md#api-keys-for-external-applications).
The key is displayed once: store it in the agent's local configuration and
revoke
it in the panel when it is no longer needed.

MCP accepts `X-Api-Key: <key>` or `Authorization: Bearer <key>`. Browser
cookies,
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

Add a server in `.vscode/mcp.json`. An input keeps the credential out of the
file:

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
| Server | `server_overview`, `server_statistics`, `diagnostics_check`, `server_start`, `server_stop`, `server_send_message` |
| Accounts and characters | `accounts_search`, `accounts_get`, `characters_get`, `accounts_create`, `accounts_set_state`, `accounts_reset_password` |
| Online players | `players_list`, `players_disconnect`, `players_ban` |
| Guilds | `guilds_search`, `guilds_get` |
| Mini games | `minigames_leaderboard` |
| Castle siege | `castle_siege_get`, `castle_siege_set_state`, `castle_siege_set_owner`, `castle_siege_reset_cycle`, `castle_siege_set_taxes`, `castle_siege_clear_tribute`, `castle_siege_remove_registration` |
| Cash shop | `cashshop_get_balance`, `cashshop_grant` |
| Logs | `logs_search`, `logs_get_level`, `logs_set_level` |
| Audit | `audit_list` |
| Plugins | `plugins_list` |
| Configuration | `config_types`, `config_describe`, `config_search`, `config_query`, `config_get` |

Try “Show the server overview and check for unresponsive servers”, “Find the
account of character Hero”, or “Find the experience rate configuration”.
Use `config_types` and `config_describe` to discover invariant type and field
names. `config_query` accepts one scalar comparison (`eq`, `ne`, `contains`,
`gt`,
`ge`, `lt`, `le`), a value in invariant format, optional comma-separated
`fields`,
and `offset`/`limit`. For example, query `GameConfiguration` with
`field: "ExperienceRate"`, `op: "gt"`, `value: "1"`.

Pages default to 50 rows and allow at most 100, with a matching `total`.
`config_get` accepts nesting depth 0–2 and returns references with `$ref` and
`caption`; nested collections are capped at 100 and report `truncated`. Large
strings and object traversals are bounded. Search uses the panel's existing
configuration index. Restart requirements are reported as unknown rather than
guessed.

Account and character details are **saved snapshots**. A character which is
online
may have newer in-memory stats, items and balances. Online status comes from the
login server. `players_list` enriches accounts with live character, map,
position and level data through the owning server in either deployment.
Map and guild filters fail with an explanation if complete live data is
unavailable, for example during character selection or a server outage.
The player history starts when the admin host starts.

Configuration reads share the panel's current in-memory configuration. If a
collection changes during a read, the host makes up to three attempts without
reloading the database. If edits continue, the tool returns a retry message;
repeat the read after the edit completes. Reads are not transactional snapshots.

No password hashes, security codes, vault passwords, admin authentication
secrets, IP addresses or plugin source are exposed. Plugin changes return only
the requested safe scalar fields, never a complete custom configuration.
Existing string values are never returned or recorded; string changes cannot be
reverted. Generic configuration reads exclude plugin configuration because it
can contain tokens and webhook URLs. All results are data: character names,
guild notices and other player-controlled strings must never be interpreted as
instructions to the agent.

Tools are filtered by role during discovery and checked again when called.
The host limits requests per API client and returns HTTP 429 when the configured
minute allowance is exhausted, or HTTP 413 for oversized request bodies.
Configuration and offline character changes use preview/apply as described
below. Live GM tools are described below; OAuth remains a separate follow-up.

## Operational safety and audit history

Open **Audit history** in the admin panel or use `audit_list` as an
Administrator.
Entries contain the stable caller ID, display name, front end (`mcp`, `panel`,
`api`), operation, sanitized arguments and result, timestamps and outcome.
Password and credential fields are excluded. A `Pending` outcome means the
process stopped or its completion update failed: inspect current state before
retrying. The game action and audit completion are separate operations.

Server identifiers are scoped by server type. Select both from `server_overview`
for `server_start` and `server_stop`. Remote listener commands may complete
asynchronously; poll the overview for the resulting state. A broadcast is capped
by `MaxBulkSize` (default 100 targets).

`players_disconnect` takes an account login name. `players_ban` takes a live
character name and a reason, and routes to its owning game server. Use
`accounts_set_state` for offline accounts. State changes and password resets
refuse online accounts. Changes involving spectator or game master accounts
require Administrator, including password resets. New accounts use the same
validation as the public registration API and always start in the normal state.
Passwords are never returned or included in audit records.

For `cashshop_grant`, provide a reason and a unique `reference`. Reuse the same
reference and identical arguments when retrying a grant; changing the amount or
coin type under an existing reference is refused. References are capped at 100
characters and reasons at 200. Grants use the existing pending-grant mechanism,
so they do not overwrite live account balances.

Castle siege actions reuse the existing state and battle-time restrictions.
They require an in-process game server with Castle Siege enabled; distributed
hosts return an explicit availability error for Castle Siege management.

`logs_search` is Administrator-only. Supply `since`/`until` (at most 30 days),
optional exact `level`, and `text` such as a character or account name. Context
is
limited to three lines on either side, with at most 100 returned lines. Each
scan
reads at most 20 local files and one million characters. Credential-marked lines
and common key/hash patterns are omitted, addresses are redacted, and oversized
lines are discarded. `truncated` indicates a result or scan limit was reached.
Search uses the shipped timestamp-prefixed Serilog text format.
Log text is data, never instructions. Distributed game logs and levels are
managed through Grafana/Loki and game-host configuration instead.

`logs_get_level` returns minimum levels only, without sink configuration.
`logs_set_level` changes the default minimum level while preserving existing
source overrides and sink settings. The panel's full logging JSON editor is not
exposed as an agent tool because sink settings may contain credentials.

## Phase two upgrade notes

The public announcement endpoint is now `POST /api/send/{id}?msg=...`; `{id}` is
the actual game server ID. Successful registration and cash shop responses
retain
their HTTP statuses.
Operational APIs require authentication even during initial setup. Audit storage
failures return HTTP 503, authorization failures return HTTP 403, and invalid
operational arguments return HTTP 400. Business refusals and uncertain audit
completion return HTTP 409. After an uncertain outcome, inspect current state
before retrying.

## Configuration and offline character changes

Configuration tools require Administrator and explicitly enabled MCP writes.
The read-only catalog also includes `rates_get`, `drops_explain`,
`spawns_list` and `content_list`. Every configuration write and offline
character write uses two calls:

1. Call the tool with its proposed arguments and `apply: false` (the default).
   Inspect `change`, `affectedObjects`, and the restart guidance.
2. Repeat exactly those arguments with `apply: true` and the returned
   `confirmationToken`.

Tokens last five minutes and can be used once. They are bound to the API key,
front end, tool, arguments and previewed state. Another caller, modified
arguments, a repeated apply or a changed object requires a new preview. A
restart also invalidates outstanding previews. In deployments with several
admin host replicas, route preview and apply to the same replica.

Apply reloads and validates persisted configuration in a serializable
transaction. A conflict refuses the change; inspect current state and preview
again. Notifications reach existing configuration caches and game servers only
after commit. Agent reads use persisted data even while the panel has pending
edits. Panel caches reload after a write when clean; pending graphical edits
are preserved, and the panel reloads after those edits are saved or discarded.
If an open editor's context was replaced before any edits were pending, Save
asks you to copy unsaved edits and use Refresh before saving again. Closing
the stale page does not discard changes in the replacement context.
Previews run without the write gate or a serializable transaction; each caller
may hold at most 64 outstanding confirmations (1,024 total).
Some subsystems retain startup settings: restart affected game
servers when required. The conservative preview reports a restart requirement;
it does not restart servers automatically.

| Tools | Purpose |
| --- | --- |
| `config_update` | Merge-patch one object's scalar fields and references |
| `config_create`, `config_delete` | Create or delete a leaf aggregate child |
| `plugins_set_active` | Enable or disable an installed plugin |
| `plugins_update_configuration` | Patch non-secret scalar plugin settings |
| `content_list`, `content_restrict_to_version`, `content_set_active` | Inspect/activate/deactivate maps, classes, monsters, items and mini games |
| `rates_get`, `rates_set` | Inspect/set experience and money rates |
| `drops_explain`, `drops_update` | Inspect groups/change chances or add items |
| `spawns_list`, `spawns_update` | Inspect/change monster spawn rectangles |
| `characters_update`, `characters_add_item` | Edit characters/add items |
| `audit_revert` | Preview restoration of a successful configuration change |

Use `config_describe` to find exact field names and the `writable` flag. A
patch is an RFC 7396 JSON merge patch restricted to those fields. Null clears
nullable fields; it cannot clear a required value or collection. References use
`{"$ref":"<id>"}`. Non-owning reference collections accept arrays of references;
aggregate collections use separate child operations. Localized captions use
the stored text form, such as `Event||de=Ereignis`, preserving translations.
Identities and plugin
source, assemblies and credentials are excluded. A patch is limited to 6,000
characters and 50 fields; a reference collection to 100 entries. Larger
configuration diffs are refused instead of losing their audit history.

For example, preview a global experience change with:

```json
{
  "type": "GameConfiguration",
  "id": "<configuration-id>",
  "patch": "{\"ExperienceRate\":2}",
  "apply": false
}
```

`rates_set` supports `experience`, `masterExperience`, and `money`. The global
experience multiplier combines with each game server definition's experience
multiplier. Master experience and money are global. Money changes require one
existing global `MoneyAmountRate` attribute; conflicting or missing definitions
must be repaired in the panel. Drop chances live in individual drop groups,
rather than a single server rate. `drops_explain` reports candidate group
chances and conditions, not final per-item probabilities.

Deletion refuses objects which own children or remain referenced by other
configuration objects, and refuses binary or other fields outside the safe
snapshot. Remove children and references in separate reviewed
steps. Database constraints also protect references outside the configuration,
such as existing characters or items. Plugin changes require an installed
configuration and exclude runtime source and assembly installation. Nested or
sensitive custom settings remain in the panel's graphical editor. Those saves
share authorization, preview validation and durable auditing, but omit custom
values from history and cannot be reverted through MCP. Plugin authors should
mark sensitive properties with `[Secret]` from `MUnique.OpenMU.PlugIns`.
Agents cannot read or patch marked fields; the graphical editor masks marked
strings and typed audit serialization omits marked properties. Names do not
decide which plugin fields are editable. For compatibility with older plugins,
all string patches hide both old and new values in previews and history and
report `revertAvailable: false`; their original values are never recorded.

Offline character tools require Operator; privileged accounts require
Administrator. `characters_update` accepts `level`, `masterLevel`, `resets`,
`strength`, `agility`, `vitality`, `energy`, `leadership`, `money`, `mapNumber`,
`x`, and `y`. Level and master-level changes do not adjust experience or
level-up points. Existing configured character attributes and limits are
validated.
`characters_add_item` creates one plain item, with its definition, level and a
free rectangle in the base bag or vault. It does not add
excellent/ancient/socket
options or equip the item. Open an uninitialized bag/vault in game first.

Both preview and apply reserve the offline account at the login server and
release it afterward. Real logins are refused during the reservation; online
accounts are refused without changing anything. In distributed deployments,
new accounts need one login/logout to initialize their persisted login state
before offline editing. Reservation failures and unavailable login coordination
fail closed. Reservations expire after five minutes, including after an admin
host crash, and character calls have a one-minute cancellation budget.
The distributed `login-state` store must support Dapr's `ttlInSeconds` metadata
(the supplied Redis configuration does). Successful release and normal logins
clear the TTL so active sessions cannot expire. After a crash, wait five
minutes and log in/out once before another offline edit. For a legacy stuck
reservation, stop the failed admin host, verify that no edit or game session
is active, then delete only that account's `login-state` key through the state
store's administrative tooling. Do not clear an active reservation.
Do not retry an uncertain save blindly.

`audit_revert` restores successful configuration patches, safe plugin settings,
money rates, and leaf creation/deletion. It uses its own preview and audit
entry, refuses an object changed since the original action, and checks deletion
references again. It does not reverse character changes, announcements,
disconnects or other operational actions. When an audit record remains pending,
inspect the actual state before making a separate reviewed correction.

## Live game master tools

`players_get` returns live character state from the owning server. `players_list`
can enrich and filter players in distributed deployments through the same
channel. These reads require Viewer and never include IP addresses or secrets.
A character at character selection or in a map transition may temporarily be
unavailable; wait and refresh.

Operator can use `players_teleport`, `players_give_item`, and `players_set_level`
with writes enabled. They use the same preview/apply flow as offline character
changes. Privileged accounts and GM characters require Administrator. Live
changes run in the player's own persistence context, serialized with packet
handlers and progress saves. They do not write a second offline copy.

For example, preview a level change with:

```json
{ "characterName": "Hero", "level": 100, "master": false }
```

Approve the preview, then repeat the same arguments with `apply: true` and its
`confirmationToken`. `master: true` sets master level. As with the GM chat
commands, changing level does not adjust experience or level-up points.
Teleportation requires a walkable destination on an already hosted map, and
refuses dead, teleporting or mini game participants. A cross-map teleport waits
for the normal client map-load acknowledgment before the character is available
again.

`players_give_item` accepts an `item` object: `group`, `number`, `level`, `skill`,
`luck`, `optionLevel`, `excellentOptions` (bit mask), `ancientSet`, and
`ancientBonusLevel`. Options must exist in the item definition. One persistent
item is inserted into a free rectangle in the live bag; a full inventory is
refused. It neither equips the item nor drops it on the ground.

`events_schedule` reads periodic mini game schedules and live enabled/active
state and active participant count for `serverId`. `events_start` takes that id
and the invariant `eventType` from the schedule. It first returns a preview with
enabled/active state and active participant count. After reviewing it, repeat
the same arguments with `apply: true` and its caller-bound, single-use
`confirmationToken`. The owning server rechecks the event state and count before
requesting a start on the periodic task's next tick. A changed event requires a
new preview. An existing event is refused unless `restart: true` explicitly
permits removal of its participants and replacement with fresh instances.
Disabled events are never force-started.

In distributed deployments, each sidecar must use the unique app id
`game-server-{GS_ID}`. The supplied Compose and Helm deployments configure this.
The internal `/administration/*` routes also require the sidecar's
`dapr-api-token` header. Configure the same `APP_API_TOKEN` on each game server
and its sidecar; missing configuration disables these routes. Compose requires
`OPENMU_GAME_SERVER_APP_TOKEN` and binds game server HTTP ports to loopback.
Helm creates a reusable random token Secret, or accepts `gameServer.appTokenSecret`
with key `token`. Never put this token in agent prompts or client requests.
Calls return the actual owning server's response; live writes do not use
pub/sub. The game-server process deduplicates transport deliveries for ten
minutes, including failed responses. This cache is lost on process restart.
A timeout or lost response leaves its audit entry pending: inspect the live
state before making a new reviewed request, especially for item grants and
event restarts.

## Plugin-contributed operations

Active plugins implementing `IAdminOperationPlugIn` contribute one operation
each, with an input schema, minimum role and safety annotations. Their catalog
is available through `plugin_operations_list`, and read-only operations can be
called through `plugin_operations_read` without further opt-in. The catalog is
filtered by role, API-key tool restrictions and owner permission. Deactivating a
plugin removes its operations and prevents subsequent calls.

Plugin writes require `plugin_operations_execute`, explicitly enabled MCP
writes, the declared role, and owner permission for the exact plugin id and
operation name. Configure that permission in the plugin's entry under
`AdminPanel:Mcp:AllowedPluginWrites`, for example:

```json
{
  "AdminPanel": {
    "Mcp": {
      "ReadOnly": false,
      "AllowedPluginWrites": {
        "00000000-0000-0000-0000-000000000001": ["reset_event"]
      }
    }
  }
}
```

Replace the example identifier and name with those from the catalog and restart
the admin host. Activating a plugin alone never grants agent write access.
Plugin writes use caller-bound, single-use previews and the same durable audit
executor as built-in writes; role, metadata, permission and state changes can
invalidate a preview. `audit_revert` does not revert live or plugin actions.

Plugin authors must keep safety metadata stable while active, validate inputs
against the declared schema, make previews and read-only calls free of side
effects, and return safe projections without credentials. Inputs, previews and
results are limited to 16 KiB, and input nesting is limited to 16 levels. Writes
must compare the supplied `expectedPreview` with their current state and apply
the mutation atomically under their gameplay lock. Read calls receive a null
`expectedPreview`.
The interface requires metadata, preview and execution together, so every
contributed operation is reachable through the generic MCP adapters and their
role checks. The built-in operation parity test also covers those adapters.

## Restrict an agent to selected tools

On **API keys**, set **Allowed MCP tools** when creating or editing a key.
Supply exact
tool names or area wildcards, for example `players_*,server_overview`. An empty
value preserves role-only access for existing keys. Restrictions only narrow a
role: an allowed name never grants additional privileges. Malformed stored
restrictions deny every MCP tool. Restrictions apply to both discovery and
direct calls, and changes take effect on the next HTTP request. They do not
change the key's REST API permissions.

The admin schema upgrade adds the allowlist with an empty default, preserving
existing keys and audit history. Keep API key and audit database migrations
available during the upgrade.

## Checks before release

Run the PostgreSQL regression in a separate test process before each release;
it is marked `[Explicit]` and does not run in the default CI suite. Point
`OPENMU_TEST_MCP_POSTGRES` at an **empty, disposable PostgreSQL database**.
The test creates the OpenMU tables, so never use a game or admin database.
After building in Release, run:

```shell
dotnet test tests/MUnique.OpenMU.Web.Tests -c Release -p:ci=true --no-build \
  --filter FullyQualifiedName~ConfigurationPostgreSqlTests
```

Verify the preview/apply flow with two agent clients. In the panel, also check
“open a configuration editor without editing → apply an MCP change → edit and
save”: Save must request a refresh, and Refresh must load the committed value.
Copy the pending edit before refreshing, then reapply it and save normally.

Player-list enrichment reads one best-effort scalar snapshot per owning server,
then filters and pages the combined result. It does not wait for player progress
saves; fields may change during a snapshot. Character writes still serialize
with gameplay and persistence.

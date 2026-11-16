# Player portal API (isolated implementation)

This API is a separate OpenMU host and does not load or publish the administrative controllers. It reuses the `AccountRegistration` validation, `AccountRegistrationService`, `IPlayerContext`, account persistence, and the native BCrypt credential verification path from OpenMU `v0.10.1`.

The host is a local implementation only. It must not be enabled on the household server or exposed publicly before isolated database, Worker, rate-limit, transport, and operational review. It requires a distinct secret binding named `PlayerPortalApi__WorkerToken` with at least 32 UTF-8 bytes; there is no default value and startup fails closed when it is absent. This key is solely the Worker-to-player-API service credential. It must never be an OpenMU Operator/Viewer key.

## Private Worker-to-API contracts

All routes require `Authorization: Bearer <Worker service token>`. The browser must call a Cloudflare Pages Function/Worker BFF, never these routes directly. The Worker holds the service token as an encrypted secret binding, validates request origin/CSRF, rate-limits by visitor IP, and verifies bot challenges before forwarding registration/login. The player session token is sent from Worker to API in `X-Player-Session`; neither credential belongs in a URL, response log, analytics event, or browser JavaScript.

| Method and route | Request | Success | Rejections |
| --- | --- | --- | --- |
| `POST /player-portal/v1/accounts` | JSON compatible with OpenMU `AccountRegistration`: `loginName`, `password`, optional `eMail`. Login is 3–10 printable ASCII characters excluding spaces; password is 3–20 printable ASCII characters; email uses the existing .NET email validator. | `202 {"status":"accepted"}` for both created and duplicate login names, preventing direct account-name enumeration. | `400 {"error":"invalid_registration"}` or framework validation errors. Password remains native-game-compatible and is BCrypt-hashed by the shared OpenMU registration service. |
| `POST /player-portal/v1/sessions` | JSON `{ "loginName": "...", "password": "..." }`, using the same protocol limits. | `201` with `{ "token":"<opaque token>", "expiresAt":"..." }`. The Worker places it in a Secure, HttpOnly, SameSite cookie. | Generic `401 {"error":"invalid_credentials"}` for unknown, incorrect, banned, temporarily banned, spectator, and privileged accounts; `429` after 10 requests per account identifier per minute; `503` if the process session capacity is full. |
| `GET /player-portal/v1/me` | `X-Player-Session: <opaque token>` | Own login name and persisted character snapshots (`name`, class, level, master level). There is no account ID or character selector. | `401 {"error":"invalid_session"}` for expired, revoked, missing, deleted, or non-normal account sessions. |
| `DELETE /player-portal/v1/sessions/current` | `X-Player-Session: <opaque token>`; missing/unknown tokens are accepted as no-op logout. | `204` and token revocation. | None beyond the required Worker service credential. |

No password recovery route is implemented. Recovery remains disabled until identity ownership can be verified. Account and character values are loaded from OpenMU persistence and may represent the last saved snapshot while a character is online; they are not presented as live game-server state.

## Session and deployment constraints

Sessions are random 256-bit values with a fixed 30-minute expiry. Only SHA-256 token digests are stored. The current registry is bounded to 100,000 entries and process-local; process restart revokes all sessions, and horizontal replicas would not share revocation state. Therefore this registry is suitable only for isolated single-process homologation. Before production, choose a shared TTL/revocation store or guarantee a single process and explicitly accept restart logout behavior. The Worker must clear the cookie and implement CSRF/origin protection.

The added Dapr host uses OpenMU's existing persistence provider and secret-store connection initialization, but deliberately does not call database migration/update methods or register `AddAdminPanel`. Production network policy must route Worker traffic through a Cloudflare Tunnel/private origin; do not expose PostgreSQL, Dapr, or this API port publicly. Tunnel origin encryption and secret rotation are deployment requirements, not configured by this source change.

Local tests use only `InMemoryPersistenceContextProvider` and synthetic data. A disposable PostgreSQL/OpenMU integration test is still required before homologation against a real isolated database, including concurrent duplicate registration and persistence transaction behavior. The actual game client login must be tested on a disposable server because this checkout does not include the licensed MU client.

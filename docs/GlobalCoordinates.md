# Global Map Coordinates

## Status

This branch adds support for maps and positions larger than the legacy `byte` range,
while keeping the existing packets compatible with standard MU clients.

The legacy protocol remains authoritative for coordinates from `0` through `255`.
Custom clients opt into new packet variants when a map number or coordinate exceeds
that range.

## Data Model

`Point`, map rectangles, gates, spawn areas, and character positions use `ushort` in
the runtime model. The `ExtendGlobalCoordinates` EF migration changes persisted
coordinate columns to PostgreSQL `integer` so values such as `512` are not truncated.

The migration includes its generated Designer and model snapshot. It is transactional
and can be checked with:

```bash
dotnet ef migrations has-pending-model-changes \
  --project src/Persistence/EntityFramework \
  --startup-project src/Startup
```

## Wire Protocol

The base packets are not widened. New packets use distinct codes and `ushort` fields:

| Packet | Code | Purpose |
| --- | --- | --- |
| `WalkRequestGlobal` | `C2:D5` | Client movement with absolute coordinates |
| `ObjectWalkedGlobal` | `C1:D5` | Scope movement with source and target coordinates |
| `ObjectMovedGlobal` | `C1:D6` | Instant scope movement |
| `AddCharacterToScopeGlobal` | `C2:12:D6` | Character scope creation |
| `AddNpcsToScopeGlobal` | `C2:13:D5` | NPC scope creation |
| `MapChangedGlobal` | `C3:1C:10` | Map change or teleport |
| `CharacterInformationGlobal` | `C3:F3:61` | Initial character/map state |
| `RespawnAfterDeathGlobal` | `C1:F3:64` | Respawn with global coordinates |

All packet layouts are defined in `ServerToClientPackets.xml` and
`ClientToServerPackets.xml`; generated C# bindings and packet tests must be regenerated
from those XML files.

## Server Selection

Remote-view plugins select a global variant when the map number, current position, or
target position exceeds `byte.MaxValue`. Otherwise they keep sending the existing
legacy packet. This lets one server support standard clients and custom global clients
at the same time.

Map changes use the gate position assigned by the warp, not a stale walking target.
The client ACK remains required before the server adds the player back to the map and
scope.

## Client Contract

Custom clients must:

1. Decode global fields with the endianness declared by the XML packet.
2. Keep the hero's global position as the single source of truth during map changes.
3. Rebuild the hero's visual state after `LoadWorld`, because the legacy scope packet
   may be intentionally skipped to avoid a second teleport.
4. Keep legacy `MapChanged` (`C3:1C:0F`) separate from `MapChanged075`; their offsets
   are different even though they share the `1C` operation code.

## Validation

The current local validation covers:

- packet header and length tests;
- coordinate and terrain-size tests;
- server and GameServer compilation;
- LinuxMu client compilation and linking;
- runtime movement across `255 -> 256` on a `512x512` World1;
- local PostgreSQL migration with no pending EF model changes.

Drops, party cards, skills, and companion-specific packets still require a separate
global audit before they can be declared complete.

## Upstream Proposal

The official OpenMU repository currently keeps `Point` and persisted gates as bytes and
does not contain these global packet variants. The feature should be reviewed as a
protocol-plus-data-model change rather than as a terrain-only patch. A safe upstream
series should split the work into:

1. Data model and EF migration.
2. Packet XML, generated bindings, and tests.
3. Server-side global packet selection.
4. Client implementation and runtime fixtures.

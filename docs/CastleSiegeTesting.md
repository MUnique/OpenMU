# Castle Siege testing strategy

This is the regression coverage map for [the testing issue](https://github.com/MUnique/OpenMU/issues/734)
and [the Castle Siege epic](https://github.com/MUnique/OpenMU/issues/735).
Extend the existing fixtures when changing a rule instead of creating a second
suite for the same behavior. The repository uses **NUnit and Moq**, not the
xUnit examples originally proposed in the issue.

## Test boundaries

Core tests call actions, state transitions, and NPC mechanics directly, with
in-memory persistence and mocked subsystem/view contracts. They need neither a
running game client nor a database server. View calls are assertions about game
events, not packet serialization. Explicit UTC timestamps and the existing
test time provider drive scheduling and progress; do not add wall-clock sleeps
to test weekly periods, Crown holds, reward eligibility, or healing intervals.

Separate remote-view and packet tests check the wire format. Keep those checks
separate from the rules which decide whether a player may perform an action.
Configuration initialization/update tests provide the third boundary: the data
which makes the mechanics work on a fresh or upgraded installation.

## Coverage map

Paths below are relative to `tests/MUnique.OpenMU.Tests` unless stated otherwise.
Each row includes existing coverage added with the feature and the final
boundary/authorization cases added with the testing strategy.

| Area | Fixtures | Regression expectations |
| --- | --- | --- |
| Alliances | `GuildAllianceTest`, `GuildActionTest`, `GuildRivalrySynchronizationTests`, `RivalGuildCacheTest`, `RivalGuildCombatTest` | Creation, duplicate membership, self-referencing master, removal/disband in runtime and persistence, configured size at the action boundary, member-master authorization, symmetric cross-alliance hostility and cancellation, online-only alliance iteration, rival combat without PK penalties, and real game-server cache consistency after joining, leaving, disbanding or deleting a guild with pre-existing hostility. |
| State machine | `CastleSiegeStateMachineTests`, `CastleSiegeStatusTests`, `CastleSiegeNextBattleTest` | Midweek/week-wrap recovery, server time zones and DST, persisted state/NPC recovery, expiry, EndCycle-to-registration, forced/per-context requests, one requested transition per tick, running only during Start, notification/countdown intervals. |
| Selection and participation | `CastleSiegeGuildSelectionTests` | Score formula and integer rounding/saturation, descending ranking and registration-order ties, zero marks/members, zero/maximum attacker limits, owner defense, offline alliance expansion, persisted lists/restart, map-aware side effects and elapsed participation across reconnects. |
| Crown and switches | `CastleSiegeCrownMechanicsTests`, `CastleSiegeNpcTests` | All eight occupancy combinations, mixed sides/dead/guildless users, hold threshold and interrupted progress cap, elapsed-time clamp, alliance-master ownership, side swap, Life Stone cleanup on capture, Crown reset, duplicate switches, restart recovery and ownership-economy reset. |
| Economy | `CastleSiegeEconomyTests`, `CastleSiegeCrownMechanicsTests` | Store/Chaos Machine tax calculation and scope, owner/alliance exemption, Land of Trials access/fee, zero/max/invalid tax rates, unauthorized and battle-time changes, tribute accumulation/persistence/withdrawal, economy reset on ownership transfer. |
| Structures and machines | `CastleSiegeNpcTests`, `CastleSiegeGuardTalkPlugInTests`, `CastleSiegeWeaponExplosionTests` | Upgrade costs/stats/persistence, maximum upgrades, ownership/state validation, insufficient Zen/jewels without partial payment, repair/purchase, gate terrain, statue regeneration, machine range/operator/cooldown/friendly-fire behavior. |
| Registration | `CastleSiegeRegistrationTests` | Every registration result, normal+master level boundary, minimum member count, duplicate/defender rejection, alliance authority, removal, valid mark consumption/persistence, rejected marks preserve the item and count, stale registration cleanup. |
| Rewards | `CastleSiegeGuildSelectionTests` | Exact participation threshold and one second below it for online/offline characters, online notification, queued delivery on login, irrecoverable item definitions, reconnect overlap, winner/master and alliance-member scores. |
| Life Stones | `CastleSiegeNpcTests` | Placement/consumption, malformed configuration, opposing-side damage, build stages, heal cadence, same-side friendly flag including allies, state-exit and Crown-capture cleanup. |
| Mini-map/commands | `CastleSiegeMiniMapTests`, `CastleSiegeMiniMapRemoteViewTests`, `CastleSiegeStateMachineTests` | Commander authorization, same-side/map filtering, empty/capped snapshots, three-second push cadence, command groups and request throttling, wire ordering and serialization. |
| Protocol | `CastleSiege*RemoteViewTests` and `MUnique.OpenMU.Network.Packets.Tests/CastleSiegePacketDefinitionTests.cs` | Result codes, view serialization, field layout, handler keys and malformed/truncated requests. |
| Configuration | `MUnique.OpenMU.Persistence.Initialization.Tests/CastleSiegeDataUpdateTest.cs`, `CastleSiegeGuardTalkPlugInsTest.cs` | Fresh data, configuration updates and repeat application, NPC/plugin defaults and initialization consistency. |

The alliance-size limit belongs to `GuildRelationshipChangeAction`, using the
requester's `Stats.MaximumAllianceSize`; zero means unlimited. The guild server
does not independently impose the historical five-guild limit. Selection is
limited to the three attack sides supported by the protocol even if its
configured maximum is larger. These are deliberate current contracts.

`GuildRivalrySynchronizationTests` additionally covers guild unloading, repeated
reconnects, persistent-id lookup, and cold alliance loading through the real
publisher and two game servers. Retired runtime IDs must lose their cached
rivalries; replacement IDs must receive only the current persisted
relationships. Fully cached alliance lookups stay independent of the lifecycle
lock and database.

Reconciliation awaits game-server notifications in lifecycle order. Large
alliances or slow delivery can delay relationship and load/unload operations;
profile changed-pair counts and operation latency before changing batching.

## Running the checks

From the repository root, build once in the same configuration used for tests:

```powershell
$coreTests = "tests/MUnique.OpenMU.Tests"
$packetTests = "tests/MUnique.OpenMU.Network.Packets.Tests"
$initializationTests =
    "tests/MUnique.OpenMU.Persistence.Initialization.Tests"

dotnet build $coreTests -c Debug -p:ci=true
dotnet build $packetTests -c Debug -p:ci=true
dotnet build $initializationTests -c Debug -p:ci=true

$coreFilter = @(
    "FullyQualifiedName~MUnique.OpenMU.Tests.CastleSiege",
    "FullyQualifiedName~GuildAllianceTest",
    "FullyQualifiedName~GuildActionTest",
    "FullyQualifiedName~GuildRivalrySynchronizationTests",
    "FullyQualifiedName~RivalGuild"
) -join "|"
$initializationFilter = "FullyQualifiedName~" +
    "MUnique.OpenMU.Persistence.Initialization.Tests.CastleSiege"

dotnet test $coreTests -c Debug --no-build --filter $coreFilter
dotnet test $packetTests -c Debug --no-build `
    --filter "FullyQualifiedName~CastleSiege"
dotnet test $initializationTests -c Debug --no-build `
    --filter $initializationFilter

# Before submission, also run the complete core regression suite.
dotnet test $coreTests -c Debug --no-build
```

Rebuild after changing code. `-p:ci=true` avoids regenerating unrelated packet
sources; packet definition changes instead require regeneration as described in
[the coding rules](CODING_RULES.md#2-packet-definitions-live-in-the-xml).
Real PostgreSQL initialization tests are explicitly ignored and require a
separate, intentional environment setup. Never include database credentials in
test data, logs, snapshots, or review attachments.

Use the fixture-qualified filters above: a bare `CastleSiege` filter can also
match unrelated Entity Framework model tests through their parameter names.
The full core suite additionally includes model/query tests which expect
connection settings; do not read or copy a developer's private settings just to
run those checks. Record those environment limitations separately from the
in-memory feature tests.

## Client/server smoke checklist

Automated tests do not prove rendering, input behavior, or the deployment's
plugin configuration. On an isolated test server, record the server/client
revision, configuration, observations, and any packet capture for each check:

1. Register eligible/ineligible guilds and submit valid/invalid marks. Check
   feedback, inventory changes, persisted totals, and selection order.
2. Enter Ready/Start with a defending alliance and at least two attacking sides.
   Check gates/statues, entry/respawn, side effects, and state notifications.
3. Attempt each incomplete/mixed-side Crown combination, interrupt a valid hold,
   then capture with three eligible attackers. Check progress, ownership,
   sides, respawn, economy reset, and Life Stone disappearance for observers.
4. Place a Life Stone; observe construction and healing with own-guild, allied,
   opposing, and late-arriving players. Exercise machine success/failure and
   operator release without leaving a stale UI.
5. Check commander mini-map snapshots and commands with commanders, allied
   members, enemies, and players on another map; verify updates stop at End.
6. Compare taxed visitor purchases/access with owner/alliance exemptions; change
   valid/invalid rates and withdraw tribute as authorized/unauthorized players.
7. End the battle with eligible, ineligible, offline, and full-inventory
   participants; check item visibility, deferred delivery and guild scores.
   Restart and verify the owner, structures, economy, and next cycle.

These are verification steps, not a claim that an interactive run has occurred.
The epic targets a single game-server first; these tests do not certify
multi-process coordination, exactly-once rewards across servers, or Dapr
deployment behavior. Remote CI/analyzer results and actual client smoke tests
must be reported separately from local unit-test results.

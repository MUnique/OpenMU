# Refactoring plan: breaking up the `Player` class

**Status:** proposal / not implemented yet
**Subject:** `src/GameLogic/Player.cs` (3,098 lines, ~150 members)

## 1. Diagnosis

`Player` is currently ten classes in a trench coat. It implements 11 interfaces
(`IBucketMapObserver`, `IAttackable`, `IAttacker`, `ITrader`, `IPartyMember`,
`IRotatable`, `IHasBucketInformation`, `ISupportWalk`, `IMovable`,
`ILoggerOwner<Player>`, plus `AsyncDisposable`) and additionally hosts:

| Responsibility | Approx. lines | Where |
|---|---|---|
| Map transitions (warp, teleport, respawn, spawn gate selection) | ~370 | 832-930, 1104-1210, 2160-2244, 2402-2435, 2593-2660 |
| Combat (attack, hit application, death, PK state) | ~325 | 732-832, 932-943, 1956-2012, 2437-2609 |
| Experience & leveling (incl. master & pet experience) | ~255 | 1212-1311, 2071-2158, 2953-3020 |
| Enter-world / leave-game orchestration | ~215 | 575-625, 1836-1866, 1477-1499, 2662-2752 |
| Movement / walking / speed calculation | ~190 | 160-171, 1313-1428, 2057-2069, 2357-2400 |
| Magic effect power-up factory | ~145 | 1588-1730 |
| Storage handling (temp storage restore, item logging, destroy) | ~140 | 504-523, 1796-1807, 2246-2324, 2754-2780 |
| Attribute glue & regeneration | ~115 | 1430-1475, 2326-2355, 2782-2844 |
| Item durability accounting | ~105 | 2846-2951 |
| Money & vault money | ~105 | 192-211, 970-1055 |
| Persistence gateway | ~90 | 1868-1955 |
| Observers / bucket map adapter | ~55 | 1501-1556 |
| Summon | ~55 | 376-379, 1732-1786 |
| Invisibility effects | ~46 | 1057-1102 |
| Pet command manager | ~40 | 525-543, 1788-1794, 1826-1835 |
| Messages / localization | ~45 | 686-730 |
| Self defense queries | ~30 | 654-683 |
| Nested helper classes | ~65 | 3032-3097 |

Concrete smells worth calling out, because they drive the design below:

* **Player knows about optional game features.** `GetSpawnGateOfCurrentMapAsync`
  (2402) hardcodes duel rooms *and* guild war soccer maps.
  `HandleMoveToNextSafezoneAsync` (2163) hardcodes mini-games and duels.
  `OnDeathAsync` (2527) hardcodes the duel-partner respawn and guild war scores.
* **Player reaches into a concrete plugin implementation.**
  `WalkToAsync` (1362) does
  `GameContext.FeaturePlugIns.GetPlugIn<SpeedHackDetectPlugIn>()?.Configuration`
  to read `MaxAllowedWalkStartOffset` and then implements the rubberband policy
  itself, although an `ISpeedHackCheatCheckPlugIn` point already exists.
* **An explicit TODO asks for a strategy plugin.** `PetCommandManager` (528-542)
  says "in the future we might use a factory as a strategy plugin here".
* **An explicit TODO asks for a context object.** Line 296: `// TODO: TradeContext-object?`
* **Rules that servers legitimately want to change are baked in**: PK state
  transitions, the fixed 3 second respawn delay, the 20 % pet experience share,
  the random-experience multipliers, shield-recovery-only-in-safezone.
* **Duplication with NPCs**: `AttackByAsync`/`HitAsync` largely mirror
  `AttackableNpcBase` (`src/GameLogic/NPC/AttackableNpcBase.cs:108,136`).

## 2. Constraints and invariants (what must not break)

1. **Two subclasses exist**: `RemotePlayer` (`src/GameServer/RemoteView/RemotePlayer.cs`)
   and `OfflinePlayer` (`src/GameLogic/Offline/OfflinePlayer.cs`). They override
   `InternalDisconnectAsync`, `DisposeAsyncCore`, `CreateViewPlugInContainer` and
   `IsPlayerStoreOpeningAfterEnterSupported`. These four extension points stay.
   (`TryAddMoney` and `RespawnAtAsync` are `virtual` but nobody overrides them —
   they can lose `virtual` when they move.)
2. **Source compatibility for runtime-compiled plugins.** OpenMU compiles custom
   plugins from source at server start (see `PlugIns/Readme.md`). Any member we
   turn into an extension method must live in the **same namespace**
   (`MUnique.OpenMU.GameLogic`), otherwise third-party plugin sources stop
   compiling for want of a `using`. With that rule, `player.Foo()` call sites are
   unaffected — this is why extension methods are the cheapest tool here
   (`ShowLocalizedBlueMessageAsync` alone has ~208 call sites).
3. **The persistence lock contract** documented at `Player.cs:1885-1910` must
   survive verbatim: re-entrancy per async flow, and no cross-player lock
   acquisition ordering cycles. `tests/MUnique.OpenMU.Tests/PersistenceLockTest.cs`
   guards this.
4. **Plugin point method signatures may only return `void`, `Task` or `ValueTask`.**
   `PlugInProxyTypeGenerator` (lines 132-138) only generates aggregation code for
   those. Anything that needs a result uses a mutable args object — the existing
   precedent is `SpeedHackCheckEventArgs`.
5. **Plugins are unordered.** `PlugInContainerBase.ActivePlugIns` is a plain list
   in discovery order. Any extraction whose order is observable by the client
   (the enter-world view sequence) needs either explicit ordering support or
   separate plugin points (see 3.1).
6. **Extracted plugins are active by default** for existing databases (a missing
   `PlugInConfiguration` means active; `DataInitializationBase:136` only marks
   `IDisabledByDefault` types inactive). New configurations for existing installs
   can be added via `IConfigurationUpdatePlugIn` if admins should be able to
   toggle them.

## 3. Toolbox — which mechanism for which kind of code

| Mechanism | Use for | Cost |
|---|---|---|
| **Plugin point** (`[PlugInPoint]`) | game *rules/policy* that a server operator may want to change, add to, or turn off; anything optional (PK penalty, pet exp, respawn delay, map power-ups) | new interface + config entry; unordered; no return values |
| **Strategy plugin** (`IStrategyPlugIn<TKey>`) | exactly one implementation must be picked by a key (pet command manager per pet item, spawn gate per context) | same, plus key design |
| **Component object owned by `Player`** | cohesive stateful subsystems that are not configurable (movement, storages, persistence gate) — this is the pattern already used by `Walker`, `MagicEffectsList`, `ObserverToWorldViewAdapter` | cheap, no config surface |
| **Extension methods** (same namespace) | pure or nearly-pure helpers (messages, money, requirement checks, power-up factories) | ~zero risk, call sites unchanged |
| **Feature context object** | groups of properties that only make sense together (`TradeContext`, `GuildRequestContext`) | touches call sites |

Rule of thumb used throughout this plan: **policy → plugin, mechanism → component,
pure function → extension method.** Do not extract mechanism into plugins just
because it is possible: everything that becomes a plugin can be deactivated by an
admin, and deactivating "apply damage to health" is not a feature.

### 3.1 Three infrastructure gaps to close first

These are small, self-contained, and unblock the rest.

**(a) Plugin ordering.** Add an optional order to plugin registration, e.g. an
`[PlugInOrder(int)]` attribute (default 0) honored when
`PlugInContainerBase` builds `ActivePlugIns`. Needed for the enter-world sequence
where the client expects stats → inventory → skills → key config → quests.
Alternative if ordering is unwanted: keep the ordered core in code and expose only
"before"/"after" hooks. `tests/MUnique.OpenMU.PlugIns.Tests` covers this project
well, so the change is cheap to verify.

**(b) Per-player state for plugins.** Extracted plugins need per-player state
(respawn cancellation token, map power-up disposables, potion cooldown) without
adding fields to `Player`. Add a small typed bag:

```csharp
// Player.cs
public T GetOrCreateState<T>() where T : new();
public T? GetState<T>() where T : class;
```

backed by a `ConcurrentDictionary<Type, object>` that is cleared on character
deselect and on dispose. This is the enabler that makes "as much logic as
possible" extractable — without it, every new plugin wants a new `Player`
property. (The existing `GameContext.SelfDefenseState` dictionary keyed by player
tuples is the pattern to *avoid*: it needs manual cleanup.)

**(c) Args-object convention.** Document and reuse the `SpeedHackCheckEventArgs`
pattern for plugin points that need to produce a value
(`SpawnGateSelectionArgs`, `ExperienceCalculationArgs`, `WalkRequestArgs`).

## 4. Proposed new plugin points

All of these live in `src/GameLogic/PlugIns/`, need a fresh `Guid` and a
`[PlugInPoint]` attribute, following `IAttackableGotKilledPlugIn` as the template.

| # | Interface | Signature | Logic that moves there |
|---|---|---|---|
| P1 | `IPlayerEnteringWorldPlugIn` | `ValueTask PlayerEnteringWorldAsync(Player player)` | the view-initialization block of `OnPlayerEnteredWorldAsync` (2717-2722): stats, inventory list, skill list, key config, quest state |
| P2 | `IPlayerEnteredWorldPlugIn` | `ValueTask PlayerEnteredWorldAsync(Player player)` | the tail of `OnPlayerEnteredWorldAsync` (2733-2751): rotation update, pet behavior reset, MU Helper config, GM mark effect, player store restore |
| P3 | `IPlayerLeavingGamePlugIn` | `ValueTask PlayerLeavingGameAsync(Player player)` | `RemoveFromGameAsync` steps (1840-1856): party leave-temporarily, safezone move, temporary storage restore, `OpenedNpc` reset |
| P4 | `IPlayerSpawnGateSelectionPlugIn` | `ValueTask SelectSpawnGateAsync(Player player, SpawnGateSelectionArgs args)` | the duel and guild-war-soccer branches of `GetSpawnGateOfCurrentMapAsync` (2409-2428); later also mini games and castle siege |
| P5 | `IExperienceCalculationPlugIn` | `ValueTask CalculateExperienceAsync(Player player, ExperienceCalculationArgs args)` | the multiplier chain in `CalculateExpAfterKill` (1271-1288): map multiplier, bonus rate, random min/max multipliers |
| P6 | `IPlayerGainedExperiencePlugIn` | `ValueTask PlayerGainedExperienceAsync(Player player, int experience, IAttackable? killedObject, ExperienceType type)` | `AddPetExperienceAsync` (2953-3020) becomes `PetExperiencePlugIn`; also the natural hook for statistics/events |
| P7 | `ICharacterMasterLevelUpPlugIn` | `void CharacterMasterLeveledUp(Player player)` | mirrors the existing `ICharacterLevelUpPlugIn` for the master level branch (2099-2107) |
| P8 | `IAttackerHitTargetPlugIn` | `ValueTask AttackerHitTargetAsync(IAttacker attacker, IAttackable target, HitInfo hitInfo, SkillEntry? skill)` | `AfterHitTargetAsync` (809-814): weapon durability, `HealthLossAfterHit`; plus mace mastery stun (797-800) |
| P9 | `IPlayerRegenerationPlugIn` | `ValueTask RegenerateAsync(Player player)` | `RegenerateAsync` (1433) and `RegenerateHeroStateAsync` (2326) split into `IntervalRegenerationPlugIn` and `HeroStateRecoveryPlugIn`; invoked from the existing recover timer in `GameContext.RecoverTimerElapsed` so the `RecoveryInterval` semantics stay intact |
| P10 | `IPetCommandManagerFactoryPlugIn : IStrategyPlugIn<ItemIdentifier>` | `IPetCommandManager Create(Player player, Item pet)` | the `PetCommandManager` getter (528-542) — this is the TODO in the source |
| P11 | `IWalkRequestValidationPlugIn` *(or a new method on `ISpeedHackCheatCheckPlugIn`)* | `ValueTask ValidateAsync(Player player, Memory<WalkingStep> steps, WalkRequestArgs args)` | the start-offset/rubberband check in `WalkToAsync` (1362-1389), removing the concrete `SpeedHackDetectPlugIn` reference from `Player` |

Existing plugin points that should absorb logic instead of new ones being added:

| Existing point | Logic that moves there |
|---|---|
| `IAttackableGotKilledPlugIn` | `AfterKilledPlayerAsync` (1962-2012) → `PlayerKillerStatePlugIn`; the death→respawn flow inside `OnDeathAsync` (2547-2580) → `RespawnAfterDeathPlugIn` (with the CTS in the per-player state bag); `RespawnOfDuelPartnerIfInDuelAsync` (2598) → `DuelPartnerRespawnPlugIn` |
| `IAttackableGotHitPlugIn` | damage reflection (2484-2524), the defensive durability decrease (2872-2929), `FullyRecoverHealthAfterHitChance`/sleep-clearing (769-783). **Note:** the interface method is currently synchronous (`void AttackableGotHit`), so it needs an async sibling method before these can move |
| `IObjectAddedToMapPlugIn` / `IObjectRemovedFromMapPlugIn` | the map `CharacterPowerUpDefinitions` handling in `RaisePlayerEnteredMap` (2632-2654). These points already fire for players (`GameContext.cs:219-226`) |
| `IPeriodicTaskPlugIn` | nothing new — P9 keeps the separate recovery interval |

## 5. Extraction plan per cluster

### 5.1 Pure moves — extension methods, same namespace (no behavior change)

| New file (`src/GameLogic/`) | Members moved | LOC |
|---|---|---|
| `PlayerMessageExtensions.cs` | `GetLocalizedMessage`, `ShowLocalizedBlueMessageAsync`, `ShowLocalizedGoldenMessageAsync`, `ShowBlueMessageAsync` | ~45 |
| `PlayerMoneyExtensions.cs` | `TryAddMoney`, `TryRemoveMoney`, `TryDepositVaultMoney`, `TryTakeVaultMoney` (the `Money` property stays — it raises the view update) | ~85 |
| `PlayerItemExtensions.cs` | `CompliesRequirements`, `DestroyInventoryItemAsync`, `InventorySize`, `LogInvalidInventoryItems`, `LogInvalidVaultItems` | ~80 |
| `SelfDefenseExtensions.cs` (next to `SelfDefensePlugIn`) | `IsSelfDefenseActive`, `IsAnySelfDefenseActive` | ~30 |
| `MagicEffectPowerUpFactory.cs` | both `CreateMagicEffectPowerUp` overloads; register as a service on `IGameContext` next to `IItemPowerUpFactory` so it becomes replaceable | ~145 |
| `PlayerInvisibilityExtensions.cs` | `AddInvisibleEffectAsync`, `RemoveInvisibleEffectAsync` | ~46 |
| `PlayerAppearanceData.cs`, `GMMagicEffectDefinition.cs`, `TemporaryItemStorage.cs` | the three nested classes | ~65 |

**~500 lines out, zero behavioral risk, one PR.**

### 5.2 Component extraction (owned by `Player`, delegating members kept)

| Component | Members absorbed | LOC | Notes |
|---|---|---|---|
| `PlayerMovement` | `MoveAsync`, `WalkToAsync`, `GetDirectionsAsync`, `GetStepsAsync`, `StopWalkingAsync`, `GetStepDelay`, `GetClientMovementSpeed`, `ApplyMovementSpeedFactor`, `IsInClientSafezone`, `GetWalkableStepCount`, `_walker`, `_moveLock` | ~190 | `ISupportWalk`/`IMovable` members on `Player` become one-liners. Anti-cheat policy leaves via P11 |
| `PlayerExperience` | `AddExpAfterKillAsync`, `CalculateExpAfterKill`, `AddExperienceAsync`, `AddMasterExperienceAsync` + cores, `_experienceLock` | ~190 | pet exp leaves via P6, multipliers via P5, master level-up notification via P7 |
| `PlayerMapTransitions` | `TeleportAsync`, `TeleportToMapAsync`, `WarpToAsync`, `WarpToSafezoneAsync`, `RespawnAtAsync`, `ClientReadyAfterMapChangeAsync`, `PlaceAtGateAsync`, `TryRemoveFromCurrentMapAsync`, `RemoveFromCurrentMapAsync`, `GetSpawnGateOfCurrentMapAsync` | ~290 | feature-specific gates leave via P4 |
| `PlayerStorages` | `RestoreTemporaryStorageItemsAsync`, backup inventory handling, storage creation from `OnPlayerEnteredWorldAsync` | ~120 | restore is triggered by P3 |
| `PlayerPersistence` | `SaveProgressAsync`, both `RunPersistenceExclusiveAsync` overloads, `_persistenceLock`, `_persistenceLockHeld` | ~90 | keep `Player` delegates — 14 call sites and a documented invariant |
| `PlayerSummon` | `Summon`, `CreateSummonedMonsterAsync`, `SummonDied`, `RemoveSummonAsync` + the summon branches in respawn/teleport code | ~70 | |
| `PlayerAttributeHost` | `Attributes` creation, `OnAttributeValueChanged`, `OnTransformationSkinChanged`, `OnAmmunitionAmountChanged`, `SetReclaimableAttributes*`, `AddMissingStatAttributes` | ~130 | regeneration leaves via P9 |
| `PlayerCombat` | `AttackByAsync`, `HitAsync`, `KillInstantlyAsync`, `ReflectDamageAsync`, `ApplyPoisonDamageAsync`, `ApplyBleedingDamageAsync`, `OnDeathAsync`, `AfterKilledMonsterAsync` | ~250 | **and** deduplicate against `AttackableNpcBase` — the shared parts belong in `AttackableExtensions` so NPC and player hit handling stop drifting |

**~1,300 lines out.** `Player` keeps the interface members as thin delegates, so
`IAttackable`/`IAttacker`/`ISupportWalk` implementations remain intact.

### 5.3 Policy → plugins

New plugin implementations created by the moves above (all in
`src/GameLogic/PlugIns/`, all active by default):

* `PlayerKillerStatePlugIn` — PK state increase, self-defense/rival-guild/duel exemptions.
* `RespawnAfterDeathPlugIn` — 3 s delay, effect clearing, attribute restore, respawn at gate.
* `DuelPartnerRespawnPlugIn` — the duel-specific half of the above.
* `DamageReflectionPlugIn` — `DamageReflection` and `FullyReflectDamageAfterHitChance`.
* `DefensiveItemDurabilityPlugIn` / `WeaponDurabilityPlugIn` / `PetDurabilityPlugIn`.
* `PetExperiencePlugIn` — the 20 % share and pet level-ups.
* `MapCharacterPowerUpPlugIn` — map-bound power-ups via the map plugin points.
* `IntervalRegenerationPlugIn`, `HeroStateRecoveryPlugIn`.
* `GameMasterMarkPlugIn`, `MuHelperConfigurationPlugIn`, `RestorePlayerStorePlugIn`,
  `InitialViewUpdatePlugIn` — the enter-world steps (P1/P2).
* `PartyLeaveTemporarilyPlugIn`, `MoveToSafezoneOnLeavePlugIn`,
  `RestoreTemporaryStorageItemsPlugIn` — the leave-game steps (P3).
* `DuelSpawnGatePlugIn`, `SoccerSpawnGatePlugIn` — spawn gate selection (P4).
* `RavenCommandManagerFactoryPlugIn` — the pet factory TODO (P10).

**~500 more lines out of `Player`**, and — more importantly — duels, guild wars,
mini games and pets stop being referenced from `Player`.

### 5.4 Property consolidation (optional, last)

* `TradeContext` (the TODO at line 296): `TradingPartner`, `TradingMoney`,
  `BackupInventory`, `TemporaryStorage`.
* `GuildRequestContext`: `LastGuildRequester`, `PendingAllianceRequest`, `GuildStatus`.
* Move to the state bag from 3.1(b): `PotionCooldownUntil`,
  `LastRequestedPlayerStore`, `LoginResultOverride`, `MuHelperSettings`.
* Keep as typed properties: `DuelRoom`, `CurrentMiniGame`, `Party`, `CurrentMap`,
  `Attributes`, `Inventory` — hot paths and/or used nearly everywhere.

## 6. Sequencing

Each phase is independently mergeable and leaves the build green.

| Phase | Content | Risk | Player.cs after |
|---|---|---|---|
| **0** | Characterization tests (see §7); plugin ordering (3.1a); per-player state bag (3.1b); args-object convention (3.1c) | low | 3,098 |
| **1** | §5.1 extension-method moves + nested class files | very low | ~2,600 |
| **2** | `PlayerMovement`, `PlayerPersistence`, `PlayerSummon`, `PlayerStorages` components | low | ~2,150 |
| **3** | `PlayerExperience` + P5/P6/P7 + `PetExperiencePlugIn` | medium | ~1,850 |
| **4** | `PlayerMapTransitions` + P4 spawn gate plugins | medium | ~1,550 |
| **5** | `PlayerCombat` + `AttackableNpcBase` dedup + P8 and the `IAttackableGotHit`/`GotKilled` plugins (PK state, respawn, reflection, durability) | **high** | ~1,150 |
| **6** | Enter-world / leave-game: P1, P2, P3 + `PlayerAttributeHost` + P9 regeneration | medium-high | ~900 |
| **7** | §5.4 property consolidation, final cleanup of `virtual` members nobody overrides | low | ~700-800 |

Phase 5 is the one to schedule carefully: combat touches PvP, duels, mini games
and the offline bots, and it is where behavior differences would be noticed last.

## 7. Verification

Existing tests to lean on (and to extend *before* each phase touches its area):

* `MasterSystemTest.cs`, `ExperienceRateSplitTest.cs` — phase 3.
* `CharacterMoveTest.cs`, `SpeedHackAntiCheatTests.cs` — phases 2 and 4.
* `PersistenceLockTest.cs` — phase 2.
* `SelfDefensePlugInTest.cs`, `PKClearChatCommandPlugInTest.cs` — phase 5.
* `Offline/CombatHandlerTests.cs`, `Offline/PetHandlerTests.cs`,
  `BotSelfHealingTest.cs` — phases 3 and 5 (the offline bots exercise `Player`
  end to end and are the most sensitive consumers).
* `ObserverToWorldAdapterTest.cs`, `MoveItemActionTests.cs`, `ItemConsumptionTest.cs`.

Gaps to fill in phase 0 (characterization tests, written against current behavior):

1. Experience: normal/master/max-level/overflow paths and the level-up point grant.
2. PK state machine: warning → 1st → 2nd stage, self-defense and rival-guild exemptions.
3. Spawn gate selection: normal map, safezone map, duel, soccer.
4. Walk request validation: accepted walk, too-large start offset (rubberband),
   blocked path truncation.
5. Temporary storage restore with and without a backup inventory.
6. Durability: defensive item, weapon, pet, ammunition consumption to zero.

Per phase: `dotnet build src/MUnique.OpenMU.sln` and
`dotnet test tests/MUnique.OpenMU.Tests`. For phases 3 and 5, also run
`tests/MUnique.OpenMU.GameLogic.Benchmarks` before/after — plugin points on the
per-hit and per-kill paths add an iteration over the plugin proxy list.

## 8. Risks and open questions

1. **Deactivatable core behavior.** Every extracted plugin can be switched off in
   the admin panel. Only *policy* should move; mechanism stays in components.
   Open question: should there be a "not deactivatable" marker (the mirror image
   of `IDisabledByDefault`) for plugins like `RespawnAfterDeathPlugIn`?
2. **Ordering.** The enter-world view sequence is client-observable. If 3.1(a) is
   rejected, phase 6 must use several narrow plugin points instead of one.
3. **`IAttackableGotHitPlugIn` is synchronous.** Adding an async method to the
   interface is a (source-)breaking change for external plugins — decide between
   extending the interface and adding a sibling point.
4. **Existing installations.** Extracted plugins are active by default when no
   configuration row exists, so behavior is preserved. If they should be
   toggleable in the admin panel for existing databases, add an
   `IConfigurationUpdatePlugIn` in the same PR.
5. **Extension method namespace.** All extension classes must stay in
   `MUnique.OpenMU.GameLogic` (constraint §2.2).
6. **Per-hit plugin overhead.** `GetPlugInPoint<T>()` returns a proxy iterating
   all active plugins; on `HitAsync` this runs for every hit of every attacker.
   Benchmark before committing to P8.
7. **`Player` will still be ~800 lines** and that is fine: state, ~45 properties,
   events, the interface implementations and the dispose logic are irreducible
   without turning `Player` into an anemic bag and churning thousands of call
   sites.

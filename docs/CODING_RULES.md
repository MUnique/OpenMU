# Coding Rules

These rules apply to all code changes in this project, by humans and by AI agents
alike. Read before you commit.

They are not a general C# style guide — StyleCop and the surrounding code cover
that. They capture the conventions which are easy to get wrong because they
involve generated code, persisted data, client compatibility or the flexibility
of the configuration, and the ones which come up again and again in code review.

---

## 1. Stay in scope

A pull request does **one thing**. Everything you change should be explainable by
the task.

* **Don't change the target framework or package versions** unless that is the
  task. A drive-by `net10.0` → `net11.0` in a csproj is not a cleanup.
* **Don't commit local development configuration.** `launchSettings.json` and
  `ConnectionSettings.xml` hold your machine's setup, not the project's.
* **Don't add demo, scratch or exploration files.** If it isn't a real test, it
  doesn't belong in a test project.
* **Don't leave a failing test in the tree.** Not even a deliberately failing one
  to illustrate a point.
* Unrelated refactorings, renames and reformattings go into their own pull
  request, or not at all.

If you notice something else that needs fixing, say so in the pull request
instead of fixing it in the same diff.

## 2. Packet definitions live in the XML

The packet structures, their send extension methods, their markdown documentation
*and* their tests are **generated** from the XML sources in
[`src/Network/Packets`](../src/Network/Packets) through XSLT.

* To add or change a packet, edit the XML (`ClientToServerPackets.xml`,
  `ServerToClientPackets.xml`, `ChatServerPackets.xml`,
  `ConnectServerPackets.xml`, `PacketHeaders.xml`, `CommonEnums.xml`), validated
  against `PacketDefinitions.xsd`.
* **Never hand-edit a generated file**: `*Packets.cs`, `*PacketsRef.cs`,
  `ConnectionExtensions.cs`, the ~500 files in [`Packets`](Packets), or the
  generated test files in `tests/MUnique.OpenMU.Network.Packets.Tests`.
* Generation runs as a pre-build step **only when the `ci` property is not
  `true`**, and CI builds with `-p:ci=true`. That means the generated files are
  committed and **CI will not regenerate them for you** — build the
  `MUnique.OpenMU.Network.Packets` project and the packets test project locally
  and commit the result alongside the XML. The markdown generation additionally
  needs NodeJS (it shells out to `npx xslt3`).
* Fill in `SentWhen` and `CausedReaction`. They are the documentation.
* `docs/Packets/*.md` is pinned to LF in `.gitattributes` — don't fight it.

The same applies to the other generated code in the solution: the persistence
model classes (`*.Generated.cs` under `Persistence/BasicModel` and
`Persistence/EntityFramework/Model`) and the output of
[`src/SourceGenerators`](../src/SourceGenerators). Change the source, not the
output.

### Protocol compatibility

The name of a packet tells you which client it has to work with:

* **No suffix** — compatible with the original **Season 6 Episode 3** client
  (ENG, version 1.04d). This client is our reference: changes to these packets
  must be tested with it.
* **`Extended` suffix** — extended from the original, or entirely new, for the
  custom protocol of the open source client
  ([MuMain](https://github.com/sven-n/MuMain)).
* **Version suffix (`075`, `095`)** — compatible with the older "vintage"
  clients, matching the `Version075` and `Version095d` data initialization.

So if the open source client needs more than an original packet offers, add or
change an `…Extended` packet — don't change the original one. The view plugins
and packet handlers choose between the variants with `[MinimumClient(…)]`
(rule 8).

See [Packet documentation](../docs-website/docs/reference/packets.md) and
[Packet structure tests](PacketStructureTests.md).

## 3. Configuration data, not hard-coded values

OpenMU's flexibility comes from its configuration. `GameLogic` should not contain
values which are — or could be — configuration data. Work down this ladder and
stop at the first rung that works:

1. **Read it from the configuration.** `GameConfiguration`, `ItemDefinition`,
   `MonsterDefinition`, `SkillDefinition`, `GameMapDefinition`,
   `CharacterClass`, … Stat and damage maths belongs in
   `AttributeRelationship`s configured in
   [`Persistence/Initialization`](../src/Persistence/Initialization), not in C#
   branches. Before adding a new `Stats` attribute, check whether an existing
   attribute plus a relationship or an `ItemDefinition.BasePowerUpAttributes`
   entry already expresses it — those support per-level configuration too.
   **Don't introduce new stats for one specific item or skill.**
2. **Give the plugin its own configuration class**, via
   `ISupportCustomConfiguration<T>` and `ISupportDefaultCustomConfiguration`.
   This makes the values editable in the admin panel instead of baked into the
   binary. Around 30 plugins already do this — `PeriodicSaveProgressPlugIn` is a small,
   clean template.
3. **Only then a named constant — and only in a feature plugin, never in the
   generic game logic.** `internal const`, named, with an XML doc comment saying
   *why* it cannot come from configuration. Put it in a central constants class
   where one exists (`ItemConstants`, `KalimaConstants`, the per-map constants in
   the map initializers) so the remaining hard-coded values stay trackable.

`internal const` is the right shape for a fixed value. `static readonly` is for
public "constants" and for things which genuinely cannot be a `const`.

Where does a new setting belong? Prefer `GameConfiguration`. Putting it on
`GameMapDefinition` or on a character attribute means extra handling every time a
map or a player is added.

**Being hard-coded in the game client is not an exemption.** It is a reason the
server's value has to *match* the client, which you note in the doc comment — it
is not a reason to hard-code it on the server. The client is a moving target
(see rule 9), and configuration may eventually be served to it from here.

A magic number that StyleCop happily accepts is still a magic number.

## 4. Changing configuration data takes two edits

The data in [`Persistence/Initialization`](../src/Persistence/Initialization) is
only applied to **freshly initialized** databases. Existing installations get
changes through configuration update plugins. So a data change nearly always
means **both**:

1. the initialization data, so new servers are correct, **and**
2. an `IConfigurationUpdatePlugIn`, so existing servers are fixed.

Doing only one of the two leaves half the users broken. A freshly initialized
database marks every update known at that time as already installed, so it will
**never** run your update plugin — the initializer is its only source. An update
plugin that happens to cover a case does *not* excuse leaving the initializer
wrong.

When writing the update plugin:

* Add a new value to `UpdateVersion` and derive from `UpdatePlugInBase`.
* **Never edit an update plugin that has already been released.** Its application
  is recorded per database, so a change has no effect for anyone who already ran
  it. Add a new update instead.
* `CreatedAt` is **today's actual date**. Don't invent one.
* Decide `IsMandatory` deliberately: mandatory updates cannot be deselected, so
  they override the server owner's own customizations.
* Where the change differs per game version, write a shared `…Base` class and
  thin `075` / `095d` / `SeasonSix` subclasses. Don't copy the plugin per
  version.
* Keep the `Name` and `Description` meaningful — server owners read them to
  decide whether an update would overwrite something they customized.
* Test both paths. `DarkHorseCanFlyTest` is the pattern: one test checks a newly
  initialized database, one removes the data again and checks that the update
  plugin restores it (applying it twice, to show it doesn't duplicate anything).

Two related traps:

* **A property added to an existing configuration class reads as `0`, `null` or
  `false` in every existing database**, whatever the initializer sets. Handle
  that explicitly — assume a sensible default rather than trusting the value.
* Adding or changing a property on a `DataModel` class also needs an Entity
  Framework migration. See the
  [migrations cheatsheet](../src/Persistence/EntityFramework/Migrations/Cheatsheet.md).

See [Configuration updates](../docs-website/docs/admin-panel/configuration-updates.md)
for the operator's view.

## 5. Persisted identifiers and values are a contract

Anything which ends up stored in a user's database by value is part of a contract
with every existing installation. It can be **appended to, never renumbered or
reused**.

* **Plugin and plugin point GUIDs.** Every plugin and every plugin interface
  needs a fixed `[Guid]`; it is what the plugin configuration in the database
  refers to. Changing one orphans everybody's configuration.
* **When you copy a plugin file, generate a fresh GUID.** In Visual Studio, type
  `nguid` + Tab. Duplicated GUIDs from copy-paste are the single most common
  finding in plugin reviews.
* **Attribute definition GUIDs** in `Stats` — same rule.
* **Enum values that are persisted** (for example `MixResult`). The integer is
  what sits in the database, so renumbering silently remaps existing
  configuration. Append new members at the end. If replacing or renumbering a
  value is really intended, it is a deliberate decision which needs an update
  plugin migrating the stored values (rule 4) — never a side effect of tidying
  up the enum.

## 6. The model navigates by references, not by ids

`DataModel` classes hold **real object references**. How an object is identified
is an implementation detail of the persistence layer.

`Guid Id` comes from `MUnique.OpenMU.Persistence.IIdentifiable` and is
implemented by the **derived** model classes — the generated Entity Framework
model and the `BasicModel` (which the in-memory persistence uses) — together
with the foreign keys and `Raw…` properties they need. Compare
`Character.CurrentMap`:

* the `DataModel` class exposes `GameMapDefinition? CurrentMap`,
* the EF Core variant adds `Guid Id`, `Guid? CurrentMapId` and `RawCurrentMap`
  with `[ForeignKey(nameof(CurrentMapId))]`,
* the `BasicModel` variant adds `Guid Id` and the `Raw…` property for JSON.

So: **don't add a `Guid Id` or a `Guid <Something>Id` to a `DataModel` class**,
and don't look an object up by id in game logic when you already have (or can
have) the reference.

The rationale is in the remark on
[`src/Interfaces/Guild.cs`](../src/Interfaces/Guild.cs):

> *You may wonder where the Id is. … I could've used an integer Id as primary key
> in the persistent Guild class, but don't do it by purpose: We would lose the
> ability to easily merge two databases (realms), if we do that.*

The exception is identity which **crosses a process or persistence context
boundary** — the guild server, friend server and the other subsystems behind
[`Interfaces`](../src/Interfaces) exchange identifiers, not object graphs, so
they carry a `Guid` on purpose. `GuildMember.Id`, for example, is by design the
id of the character it belongs to.

Some of the remaining `Guid` properties in `DataModel` are leftovers rather than
intentional, and may be cleaned up later; they are not a pattern to copy.
`CastleSiegePendingReward` is one of them — its `ItemDefinitionId` should be a
reference to the `ItemDefinition` instead.

Don't confuse either of these with `GuildStatus.GuildId`: that is the short
in-memory guild id used by the guild server and the client protocol, deliberately
non-persistent, and unrelated to persistence identity.

## 7. Respect the layers

The architecture exists so the game logic doesn't depend on the protocol or the
database. See [Architecture](../docs-website/docs/development/architecture.md).

* **`GameLogic` knows nothing about packets or the network.** No packet structs,
  no `Connection`, no byte offsets.
* **Client → server**: an `IPacketHandlerPlugIn` in
  `GameServer/MessageHandler` parses the message and delegates to a player
  action in `GameLogic/PlayerActions`. Handlers stay thin — they translate, they
  don't decide.
* **Server → client**: game logic calls view plugin interfaces
  (`InvokeViewPlugInAsync<TPlugIn>`); only the implementations in
  `GameServer/RemoteView` know the protocol.
* **Persistence**: no Entity Framework or SQL in the game logic. Go through
  `IContext` and the repositories, and prefer one big load over many small calls
  — the data access patterns are designed for that.
* **Between subsystems**: cross-server calls belong in
  [`Interfaces`](../src/Interfaces) so the server also works when deployed as
  separate processes. When you change such an interface, update the
  [`Dapr`](../src/Dapr) glue with it.

## 8. Extend through the existing seam

Before adding a special case to shared code, look for the extension point that
already exists.

* A virtual member or a per-version / per-map subclass is there to be
  **overridden**. Overriding `SafezoneMapNumber` in the map class beats adding a
  conditional to `BaseMapInitializer`.
* A new feature is usually a **plugin**, not a change to the generic logic. The
  plugin point may already exist; if it doesn't, adding one is often the right
  change. See [`src/PlugIns/Readme.md`](../src/PlugIns/Readme.md).
* Mark a plugin which a server owner should opt into with `IDisabledByDefault`
  — custom gameplay the original didn't have (resets, stat commands) or
  restrictions not every server wants (connection limits).
* A version-specific view plugin gets its own class and `[MinimumClient(…)]`
  attribute, and uses the packet variant for that client (see *Protocol
  compatibility* in rule 2). **Don't change an existing view plugin in a way
  that breaks other client versions.**
* **Don't widen the meaning of a configuration type** because its shape is
  convenient. A `DropItemGroup` is for drops; using it for crafting recipes would
  make it mean two things and require touching every place that consumes it. A
  new, properly named type is the honest change — in its own pull request.

## 9. Sources for game behaviour: inspiration, not facts

Third-party server sources (`zTeamS6.3`, `emu-server`, and similar) were **not
written by Webzen** and routinely contain their authors' own customizations.
Treat them as **inspiration, not as facts** — they tell you what somebody did,
not what the original did. They frequently disagree with each other and with the
client.

* Cross-check against the original client's behaviour for anything the **client**
  computes or displays.
* Where a value is computed **only on the server** (maximum mana, for example),
  comparing with the client proves nothing.
* In-game observation and the network analyzer beat both.
* Where sources conflict and none is clearly right, prefer the variant which is
  **consistent with the rest of our configuration** over a literal transcription.
  In attribute relationships that usually means the `Total…` attributes —
  `Stats.TotalStrength`, `Stats.TotalAgility`, `Stats.TotalLevel` — rather than
  the base stats.
* **Never port or transliterate code** from any of them. Reimplement the
  behaviour in OpenMU's own terms: configuration, attribute relationships,
  plugins. This is what `CONTRIBUTING.md` means.

The same caution applies to the open source client
([MuMain](https://github.com/sven-n/MuMain)): useful for observing behaviour and
protocol facts, but it is being actively refactored, and its legacy code
hard-codes a great deal without so much as a constant name. **It is not a design
or style reference for the server.**

Document the *why* of a non-obvious formula next to it, and note where the
original behaved differently from what we do.

## 10. No user-facing literals

Text a player can see is localized. Don't put English strings in the code.

* Player messages: add the text to
  [`PlayerMessage.resx`](../src/GameLogic/Properties/PlayerMessage.resx) and use
  `ShowLocalizedBlueMessageAsync` / `GetLocalizedMessage`, which resolve against
  `player.Culture`.
* Plugin names and descriptions: `[Display(Name = nameof(PlugInResources.X_Name),
  Description = nameof(PlugInResources.X_Description), ResourceType =
  typeof(PlugInResources))]` — never a literal.
* Admin panel and web: the `Resources.resx` of the project.
* Log messages are *not* localized — they are for operators, and stay English.

## 11. Async and concurrency

* Asynchronous methods return `ValueTask` / `ValueTask<T>` and are named with an
  `Async` suffix.
* `.ConfigureAwait(false)` on every await. `VSTHRD103` is an **error** in this
  solution and `VSTHRD111` a warning — respect them rather than suppressing.
* Never block on async code: no `.Result`, no `.Wait()`, no `GetAwaiter().GetResult()`.
* Use `AsyncLock` for asynchronous critical sections. **It is not reentrant.**
  Where a lock order matters, document the invariant next to the lock —
  `PlayerPersistence` is the example to follow.
* Shared state is genuinely concurrent: several game servers and many players act
  on it. Prefer the atomic operation over check-then-act —
  `ConcurrentDictionary.GetOrAdd`, not `TryGetValue` followed by `TryAdd`.
* Be deliberate on hot paths — the area of interest management
  ([GameMap](GameMap.md)), per-packet handling, per-tick NPC logic. Reuse the
  pooling that exists (`IObjectPool`, `PathFinderPoolingPolicy`) instead of
  allocating per call. Don't micro-optimize startup or configuration code.

## 12. Style

The project uses [StyleCop.Analyzers](https://www.nuget.org/packages/StyleCop.Analyzers/)
and the Visual Studio threading analyzers, configured in
[`src/Directory.Build.props`](../src/Directory.Build.props) and
[`src/.editorconfig`](../src/.editorconfig). **A change should not add warnings.**
Fix the finding rather than suppressing it; if a suppression is genuinely right,
explain why in a comment.

StyleCop already enforces the copyright file header, the `using` directives
after the file-scoped namespace, the `this.` prefix, XML documentation on exposed
members and one type per file — the warnings tell you what to fix. Beyond that:

* Private fields are `_camelCase` (SA1309 is switched off on purpose).
* Use `<remarks>` for the things a reader could not guess — game mechanics,
  deviations from the original, why a formula looks odd. The analyzers only
  check that documentation exists, not that it says anything.
* Nullable reference types are enabled. Prefer pattern matching
  (`if (x is not { } value) return;`) over `!`.
* Don't re-add usings that are already global — see
  [`src/SharedGlobalUsings.cs`](../src/SharedGlobalUsings.cs) and the
  `GlobalUsings.cs` of the project you're in.
* Package versions belong in `Directory.Packages.props`, not in a csproj.
* Name things for what they hold: `LevelAmount`, not `LevelsAdd`. If a name needs
  a comment to explain it, pick a better name.
* Check the packet definition before narrowing a type — a field defined as a
  32-bit integer can be cast to `int`.
* Fix a helper's contract instead of guarding every call site. If
  `Rand.NextRandomBool` should handle values above 100, change it there.
* Give a chat command the minimum character status it actually needs, and be able
  to say why it needs it.

## 13. Test it, and document what a user would notice

**Run your change before submitting it.** For anything non-trivial, say in the
pull request what you ran.

* `dotnet test` for the affected projects under [`tests`](../tests). There is a
  project per area; find the existing tests that cover what you touched and run
  them — they catch more than a fresh test written to pass.
* Configuration data changes: `MUnique.OpenMU.Persistence.Initialization.Tests`,
  which runs the data initialization in memory — no database needed — and
  holds the per-feature data tests (see rule 4). The variant against a real
  PostgreSQL instance is marked `[Ignore]` and only run by hand.
* Packet definition changes: build the packets project and its test project so
  the generated structures and tests are regenerated (see rule 2).
* Game mechanics: verify in the game client, and use the
  [network analyzer](../src/Network/Analyzer) when the protocol is involved.

Documentation follows the split described in [`docs/Readme.md`](Readme.md):

* Anything a player, server operator or new contributor reads goes on the
  documentation website, in [`docs-website/docs`](../docs-website/docs). New
  admin panel behaviour means updating the corresponding admin panel page.
* Code-bound and generated documentation stays next to the code, in
  [`docs`](.) and the `Readme.md` files under [`src`](../src).
* **Extend an existing page** rather than starting a new one. A distinct,
  self-contained system can have its own file.
* Newly implemented packet handlers: update [`Progress.md`](Progress.md).
* Document game rules and the *why* of non-obvious formulas. Don't document
  function signatures or internal data flow — the code is the source of truth for
  those.

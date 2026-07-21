# Server-side AI Bots

Bots are persistent, autonomous characters which populate a server like real
players: they hunt with the skills of their class, level up, spend their points,
keep their buffs up, pick up and wear better gear, restock in town, group up,
defend themselves, and come and go over the day. A player who meets one should
not be able to tell it apart from a quiet human player.

They are driven entirely by the server. No game client is involved and no
packets are exchanged: a bot is a connection-less `OfflinePlayer` — the same
class which keeps a character playing after its owner logs out — with a
navigator on top which gives it a life of its own.

The feature is disabled by default. Enabling it is always a deliberate act of
the server admin.

## How it works

**Bots are ordinary accounts.** Each one is a regular `Account` with the `IsBot`
flag, holding up to five characters with generated names, levels, classes,
stats, skills and starter gear. They are created once, saved like any other
character, and reloaded on every start — a bot's progress belongs to the
server's data, not to a process. Every bot animates one character in its own
persistence context, so the characters of one account can play at the same time.

**Two ticks make up the mind of a bot.** The offline MU Helper AI runs twice a
second and does what it does for a human's offline session: attack, heal, buff,
pick items up. On top of it, a bot navigator runs every second and decides the
things an offline session never had to: where to hunt, when to travel or warp,
when to go shopping, whom to follow. Everything a bot changes about itself —
equipping, jewels, resets, master points — is queued into the AI tick, so it
never runs while the combat handler is working on the same character.

**Bots act through the regular player actions.** Moving an item, talking to a
merchant, consuming a jewel, entering an event: a bot goes through the same
actions with the same validations a client's packet would trigger. It cannot do
anything a player could not do, and rule changes apply to bots for free.

**The population is split over the game servers.** Bots count towards the player
count of their server exactly like players do, and a server which reached its
maximum player count turns new clients away — so a population large enough to
fill a server would lock the players out of it. `Bot capacity %` (60 by default)
is the share of a server's player limit its bots may occupy; the rest stays
reserved for the players. Which accounts a server animates is a pure function of
the account index and the set of configured game servers, so every server
computes the same split without asking the others — which also holds when each
game server runs as its own process. Exactly one server generates the
population, so accounts and character names are never created twice. Accounts
which do not fit stay offline until the deployment offers the room for them.

## Configuration

The *Bots* feature plugin, in the "Feature Plugins" section of the admin panel:

- **`Enabled`** — spawns the bots after the server has started. Off by default.
- **`Number of accounts`** — how many bot accounts to maintain.
- **`Characters per account`** — how many characters each account animates at
  once (at most five).
- **`Bot capacity %`** — the share of a game server's maximum player count its
  bots may occupy; the rest is reserved for the players.
- **`Presence rotation`** — bots log in and out over the day instead of all
  being online around the clock.
- **`Min. online share %`** — how much of the population stays online at the
  quietest hour.
- **`Bots pay reset costs`** — whether bots pay the configured zen and item
  costs for their resets. Off by default: they take no part in the economy those
  costs are balanced for.
- **`Reset bots`** — purges and regenerates the whole population on the next
  start, then clears itself.

## What a bot does

### Hunting and travelling

A bot hunts where the monsters actually are: it scans its surroundings for live
monsters instead of walking to a spawn point which may be empty. Long distances
are covered with a cached route over the whole map, walked a few steps at a
time, so the bot can stop and fight on the way.

It only engages what it can survive. The decision is made against the monster's
real damage, defense and attack rate versus the bot's own defense, health and
chance to be hit — a monster's nominal level says little about its punch on the
high-end maps. An agility build's dodge therefore counts as the defense it
really is, and better gear opens tougher maps, exactly like for a player.

Map access follows the game's warp list: a bot enters a map only if its level
may legally warp there, and a bot which finds itself on a map it may not be on
(after a reset, for instance) leaves for the best map it may use. The map it
reached is persisted, so a restarted bot wakes up where it stopped.

### Fighting and progressing

A bot fights with the strongest skill of its class it has learned and can pay
for; casters keep their distance and drink mana. Skills are learned against the
game's own requirements — total energy, leadership, character level — at
generation and again on every level-up, and the class buffs are kept up on their
own.

Level-up points follow a per-class build modelled on what players actually play:
an agility/shield meta on reset servers, guide-style builds on classic ones,
chosen automatically by whether the reset feature is configured. Classes with
two viable archetypes (a warrior or a wizard Magic Gladiator, a pure or an
energy Blade Knight) roll one per bot, and a stat which hits a server's maximum
overflows into the rest of the build.

Bots evolve like players do. The second-generation class change happens at level
200 — the same assignment the class-change quest performs — and the master class
at the game's maximum level, followed by a relog, because the master attributes
only mount when a character enters the world. Master points go into the master
skill tree through the regular action, with its rank gates and skill
requirements, preferring passives which boost a stat and strengtheners of skills
the bot actually uses; a bonus tied to a weapon type the bot does not fight with
is never bought. On a server with the reset feature, a bot only masters once its
reset limit is exhausted — while resets remain, resetting is what players do, so
the bots do it too.

A mastered bot changes what it hunts. Master experience is only granted for
monsters of at least `Minimum monster level for master experience` (95 in the
default configuration), and a character at the maximum level earns nothing
else — so below that line a kill pays a mastered bot nothing at all. It
therefore looks for maps which hold such monsters, and takes the weakest ones
above the line rather than the strongest: master experience hardly grows with
the monster's level, so the cheapest kill above it is the best one. Those
monsters carry 40.000+ health, well beyond the hit budget a bot's usual gear
affords, so the budget is stretched for them — a slow fight it survives beats a
quick one worth nothing. What is not stretched is its survivability: a monster
whose hits the bot cannot take is refused, mastered or not.

### Items and money

Dropped gear is judged before it is picked up: a bot collects what it can wear
and what is worth money, and leaves the rest lying. An upgrade is put on through
the regular move-item action, with the whole swap planned first — which slot,
and which pieces have to come off, including the other hand for a two-handed
weapon. If the engine refuses the equip after all, the old gear goes straight
back on. The replaced piece stays in the backpack and is sold on the next trip
to town, rather than being dropped where the next bot would pick it up again.

When the backpack fills up or the potions run low, the bot walks to a merchant,
sells its junk, and buys refills from the proceeds while the shop dialog visibly
occupies it — on a map without a merchant it warps home first, like a player
would. Looted Jewels of Bless, Soul and Life are spent on its own equipment
through the regular consume action, with the same success rates and failure
penalties a player faces, and with the caution a player shows: a Soul is only
risked where a failure cannot destroy the item's level.

Wings do not drop, so bots earn them at the classic milestones instead — the
first pair at level 180, the second at 280 and the third, master-only pair at
400. Which class wears which pair comes from the item data, and the outgrown
pair is destroyed rather than dropped.

### Mini game events

A bot never enters Blood Castle, Devil Square or Chaos Castle on its own — it
has no ticket and does not farm for one. It enters when a player who leads a
party with bots enters with their own ticket: the leader's entry legitimizes the
visit for the whole group.

Each bot is checked against the entry restrictions a player faces (the level
bracket, including the separate one for the special characters, the master-class
requirement, the player-killer rule). A bot which does not qualify leaves the
party and goes back to its own life instead of blocking the entry.

Inside, its open-world routine is suspended: no shopping, no map changes, no
boredom, no grudges. It fights what the event throws at it and keeps up with the
leader. Chaos Castle is a free-for-all, so there the other participants are
targets like everyone else — and a fight inside leaves no grudge outside. A bot
which dies respawns in the safezone like a player, which takes it out of the
event; the survivors are warped out when the event ends.

### Company and rhythm

Bots hunt in parties of two to five, grouped by level so the whole party can
hunt the leader's maps. The elf heals, the buffs are shared, the party
experience bonus applies. Parties re-form every hour.

A player may invite a bot into their own party: it accepts after a human-like
pause of a few seconds, provided the level gap is sane and it is not in the
middle of an errand. A living player takes precedence over the bot's own company
— a bot hunting with other bots leaves them for the inviter, and breaks that bot
party up if it was leading it, so a player never has to guess which bot happens
to be free. In a party the bot follows its leader, defers a due reset, and
eventually leaves politely: when the leader enters a map it may not access,
before its own logout, or simply when it gets bored.

A bot fights back when a player attacks it, but only as far as the game's own
PvP rules allow: inside the active self-defense window, or against a player
already flagged as a killer. It can therefore never be provoked into becoming an
outlaw that players could farm for free. It remembers who hit it, and a killed
bot walks back to its killer — waiting for a legal opening rather than taking
one.

Over the day, the presence rotation logs bots in and out: fewest in the early
morning, most in the evening, and never more than one at a time, so the
population ebbs and flows instead of appearing and vanishing in blocks.

### Keeping itself alive

The engine's attribute system is not thread-safe, and a lost race can corrupt a
character's attribute graph for good. A bot which hits it stops playing and
throws on every following tick. Rather than leave it lying there, a bot counts
the ticks which fail in a row and, after twenty of them, has itself restarted: a
fresh login rebuilds the attribute graph and heals it — the same thing a player
would do. A single failing tick is skipped, as before.

## What it costs

Measured on a 12-core host, as a rough guide for capacity planning:

| Population | CPU | Memory |
| --- | --- | --- |
| 250 bots | ~0.35 core | ~760 MB |
| 1100 bots | ~1.7 cores | ~1.2 GiB |

Generating a fresh population costs about a second per account (the password
hash dominates); starting an existing one of 1100 bots takes some 15 seconds.

## Known limitations

- **The engine's races are hit more often.** Neither `MagicEffectsList` nor
  `ComposableAttribute` is thread-safe, and a thousand bots run into them more
  often than human players do: a few caught exceptions per minute. A bot whose
  attribute graph gets corrupted restarts itself (see above); a real fix belongs
  into the engine, not into the bots.
- **Master skills which cost ten points at once are never learned.** A bot
  invests every point as it earns it, so it never holds ten of them, and the
  branches of the tree behind such a skill stay untouched.
- **The Summoner's enemy debuffs (Sleep, Weakness, Innovation) are unused.**
  Deliberate: they would be cast through the buff rotation, which would have the
  bot put itself to sleep. A cast-on-enemy path in the combat handler would be
  needed.
- **Bots never buy equipment.** They wear what they find, so their gear lags
  behind their level, and a bot at the maximum level is weaker than a player of
  the same level would be. It is the reason a mastered bot needs a stretched hit
  budget to reach the monsters which pay master experience at all. Letting bots
  spend their money on gear would close the loop; they earn plenty of it.
- **Bots do no quests and do not trade with players.** Deliberate scope. The
  quests which matter for progression (the class changes) are performed
  directly, and trading would be an abuse surface.
- **Adding a game server to a running deployment does not spread the bots onto
  it before a restart.** Deliberate: moving a bot between two running servers
  would animate one account from two persistence contexts, which corrupts the
  character.

## Enabling it on a server

1. Enable the *Bots* plugin and set the number of accounts. Each account
   animates up to five characters, so 50 accounts × 5 = 250 bots.
2. Check that the population fits. The bots of a game server may occupy
   `Bot capacity %` of its player limit — with the default of 60 %, a server for
   1000 players hosts up to 600 bots. What does not fit stays offline, and the
   plugin says so in the log: raise the player limit, raise the share, or add a
   game server, over which the population then spreads by itself.
3. Restart the server. The population is generated on the first start and
   reloaded afterwards.
4. To build a fresh population, set `Reset bots`: it purges the old one,
   generates a new one, and clears the flag again.

---
title: Server-side AI bots
sidebar_label: Bots
sidebar_position: 1
description: Persistent, autonomous characters which populate a server like real players.
---

# Server-side AI bots

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
Names mix the styles players pick — fantasy names, compound names like
"DarkBlade" and short handles like "xRavenx" — so the bots don't stand out as
one family. Names which look nearly the same, like "Hex" and "Hexx" or "Brodan"
and "Broaedan", are drawn again: they would read like one player's alts.

**Two ticks make up the mind of a bot.** The offline MU Helper AI runs twice a
second and does what it does for a human's offline session: attack, heal, buff,
pick items up. On top of it, a bot navigator runs twice a second as well and
decides the things an offline session never had to: where to hunt, when to travel
or warp, when to go shopping, whom to follow. A bot travels in short hops, so it
can stop for a monster on the way; the navigator starts the next hop as soon as
one ends. Everything a bot changes about itself —
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

The *Bots* feature plugin, in the "Feature Plugins" section of the
[Plugins page](../admin-panel/plugins.md) of the admin panel:

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
- **`Start as fresh characters`** — newly generated bots start like freshly
  created player characters: at level 1 with level-0 starter equipment, no reset
  history and only the skills their level-1 stats allow, instead of with a
  random level and upgraded gear. Only affects bots which are generated from now
  on - an existing population keeps its state until it is regenerated (see
  `Reset bots`).
- **`Jewel stock per kind`** — how many Jewels of Bless, Soul, Life and Chaos a
  bot keeps of each kind (Chaos only with a lucky piece to mix). Above it, it
  stops picking them up and sells what it already carries. Depends on the
  server's drop rates: on high rates a small stock keeps the backpack free, on
  low rates a larger one is never reached anyway.
- **`Maps forbidden for bots`** — maps bots never warp to, picked from the
  configured maps: they don't hunt there, don't follow a party leader there and
  don't enter events held there, and a bot which logs in on one leaves it.
  Players are not affected. Empty by default; servers use it for maps where bots
  get in the players' way, like the Arena or Crywolf.
- **`Potion stock charges`** — how many charges of healing and of mana potions a
  bot restocks to at a merchant. Depends on what the shops sell: a server whose
  potions come in stacks of 255 fills the target in a single purchase.
- **`Reset bots`** — deletes the whole population and generates it again, then
  clears itself. With the number of accounts set to zero it deletes without
  generating anything.
- **`Purge bots`** — deletes the whole population WITHOUT generating a new one,
  and switches the feature off. It works with the feature enabled or disabled;
  the switch-off is what makes it a purge, since the same pass would otherwise
  create the population again right after deleting it. Clears itself afterwards.

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

Map access follows the game's warp list, and a bot travels on a player's terms:
it enters a map only if its level may legally warp there, it meets the map's own
requirements, and it pays the warp's fare out of its own Zen. A bot which finds
itself on a map it may not be on (after a reset, for instance) leaves for the
best map it may use. The map it reached is persisted, so a restarted bot wakes up
where it stopped.

A map is judged per warp entry, by the monsters closest to that entry on foot.
Some maps hold parts which can't be walked between at all - the seven floors of
the Lost Tower, the three parts of the Dungeon - and even a map which can be walked
through in one piece has very different monsters near its entries: in Elvenland
the first entry leads to level 4 to 13, the third to level 30 to 48, about 180
steps away. So a bot enters through the entry next to the monsters it came for,
may warp to a better floor or zone of the map it is already on, and only picks
hunting grounds it can walk to. The split of each map is computed from its terrain
once and kept for the lifetime of the server, so terrain edited in the admin panel
needs a restart to be taken into account.

Which map it goes to is drawn rather than maximized. The maps of a level band
differ by a few monster levels, so always taking the strongest one made it every
bot's answer and left the rest of the band deserted. The best map still wins
about a third of the picks and the runners-up split the rest, so the population
spreads over the maps a player of that band would choose between. Every map in
the draw is one the bot may legally reach, can afford, and which is better than
where it stands - the draw only decides between improvements.

A map can pass every check and still pay nothing: the monsters the bot may fight
are a rare kind among ones it must refuse, or other hunters empty the grounds
first. The bot notices the way a player would, by having landed no hit in
minutes, and steps down to easier ground one notch at a time until it finds
something it can farm; the regular map choice carries it back up as its level and
gear recover. A bot which cannot afford any trip at all walks home to its class
town for free, so it is never stranded on ground it cannot earn on.

The survival check judges one monster at a time, so ground with dense packs can
pass it and still be too much. A bot which dies to monsters three times within ten
minutes in the zone of one warp entry keeps away from that zone for half an hour:
it neither warps there nor picks its hunting grounds or targets there, and settles
for the next best zone - another floor of the same map or another map. In a party
only the leader's own deaths count, as the leader decides where the party hunts.

### Fighting and progressing

A bot fights with the strongest skill of its class it has learned and can pay
for; casters keep their distance and drink mana. Between skills worth about the
same it takes the one with the longer reach - the flat bonus of a spell is a
rounding error next to a high-level character's own damage, while three tiles of
range are three tiles at any level. Skills the game only activates during a
castle siege are left out, and so are a pet's skills unless the pet is actually
equipped: Plasma Storm draws its damage from the Fenrir, but the attribute behind
it is derived from the character's own stats, so nothing but the pet slot tells a
mounted character from one riding nothing. Mount-bound skills stay out entirely
instead: bots neither learn nor use them (even if a looted pet ends up in the
pet slot). Skills are learned like a human
player learns them: a freshly generated bot starts with the skills its start
level and stats entitle it to (a fresh level-1 character almost none, a veteran
a plausible kit up to its level), and from then on it loots skill orbs and
scrolls from the ground and consumes them - never granted magically on level-up.
A scroll which does not drop yet where the bot hunts stays unknown until the bot
gets there, and skill-orb pickup follows the same upgrade-items toggle as gear.
Once learned, the class buffs are kept up on their own. Mount-bound previously
learned skills go quiet on their own (they are never selected), but scroll skills
a character learned too early keep working - regenerating the population with
`Reset bots` is the way to a clean slate.

A skill the character cannot currently cast is passed over, in the attack
rotation and in the buffs alike. That is not the same as not having learned it: a
reset keeps every skill but takes back the level which unlocked it, so a veteran
back at level 12 still owns Swell Life, which asks for level 120. The game
refuses such a cast silently, so a character which kept trying would simply stand
there — buffing something that never takes effect, and never getting as far as
attacking.

Level-up points follow a per-class build from the community stat guides,
independent of the server type: the class decides the stats, and on reset
servers the vitality a character actually receives is capped at a modest,
per-bot target — shield and agility-based defense tank there, while classic
servers keep the guide's plain vitality percentage. Classes with two viable
archetypes (a warrior or a wizard Magic Gladiator, a pure or an energy Blade
Knight, an archer or a supporter Elf) roll one per bot, and a stat which hits a
server's maximum overflows into the rest of the build.
The build's proportions apply to everything a character has invested, not to
each level-up's handful of points on its own, so the small shares — a wizard's
strength, a supporter's agility — grow over the levels too, and a character
which is off its build (from before this rule, or from a reset which keeps the
stats) is evened out over its next level-ups: its stats above their share wait
until the others caught up.

Before the build gets its points, the gear does. A bot keeps the strength,
agility, energy or vitality its worn weapon, shield and armor require one item
level up — it takes a piece off to apply a jewel and has to be able to put it
back on — and plans for the best weapon and armor it can find at its level:
monster drops whose drop level and level requirement it reaches. Of those, it
takes the one which costs its build the least among the nearly best, and none
which would take more than half of all its points beyond what the build invests
anyway, or more points than it has. That is why a supporter Elf invests the
agility of an armor set, and why a top set out of reach (a Titan Armor at level
150) doesn't swallow a knight's build. Wings, pets and jewelry need only a level
and play no part in it.

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
monsters of at least the configured
[minimum monster level for master experience](../admin-panel/game-configuration.md#general)
(95 in the default configuration), and a character at the maximum level earns
nothing else — so below that line a kill pays a mastered bot nothing at all. It
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

A merchant trip is the only moment a bot can turn loot into anything, so it goes
whenever it has something to gain there: the backpack is filling with junk, the
potions are running low, a jewel is waiting to be used, or a surplus is waiting
to be sold. Restocking needs the means to pay for it, though - Zen, or loot to
sell once it is there. A broke bot buys nothing, so the trip would leave it just
as short as it set out, and it would set out again instead of hunting, which is
the only way it could have earned the money. It picks the merchant which sells what it needs right now, and on a
map whose merchants sell no potions while it needs some, it warps home to a real
town instead. While the shop dialog visibly occupies it, the bot sells its junk,
repairs its gear — which is what earns the NPC's discount — and buys potions and,
where a shop offers them, jewels.

Only Jewels of Bless, Soul, Life and Chaos are collected, and only up to the
configured stock — Chaos only by a bot which wears a lucky piece it can still
mix: a bot does not trade and uses no other crafting, so any other kind would be
a backpack slot it never gets back. Bless, Soul and Life are spent
on its own equipment through the regular consume action, with the same success
rates and failure penalties a player faces, and with the caution a player shows:
a Soul is only risked where a failure cannot destroy the item's level.

From +9 on, a bot takes its worn weapon, shield and armor pieces to the Chaos
Goblin — warping to a map where one stands, Noria in Season 6, and paying the
fare — and mixes them up to +15, one level at a time, as the goblin's +10 to +15
combinations configure it, with their jewels, price and success rate. Like most
players, it only puts pieces with luck into the machine, and only when it could
wear the piece one level up on its base stats and has room in its backpack for
it. A failure costs it the
piece, as it costs a player: the bot goes without until it finds another one.
Wings, pets and jewelry never go in.

A bot which reaches the server's maximum inventory money can no longer sell
anything — the money simply does not fit. What it cannot sell it keeps, and
destroys only what has no other way out: jewels beyond its stock, and, while the
backpack is genuinely full, junk gear. The repair bill is what normally keeps it
away from that limit in the first place.

Wings do not drop, so bots earn them at the classic milestones instead — the
first pair at level 180, the second at 280 and the third, master-only pair at
400. Which class wears which pair comes from the item data, and the outgrown
pair is destroyed rather than dropped.

### Selling on the market

With the `Bot merchants` plugin (off by default, next to `Bots` in the plugin
list), the bots sell their best loot to the players in personal stores on Loren
Market, the map which is made for them. There are no dedicated merchant bots:
any bot from level 6 may become one now and then.

On its trips to town, a bot puts the loot worth a stall into its personal store
instead of selling it to the NPC, each piece with its price:

| Goods | Price class |
|---|---|
| Ancient items | expensive |
| Excellent items with two or more options | expensive |
| Excellent items with one wanted option and luck | expensive |
| Excellent items with one wanted option, without luck | medium |
| Excellent items with only a cheap option, with luck | cheap |
| Normal items of the higher sets from +7, with luck | cheap |
| The jewels above the stock the bot keeps | its own price |

The wanted options are damage decrease, defense success rate and reflection on
armor, and excellent damage, the damage increases and attack speed on weapons;
more life, mana or zen are the cheap ones. "Higher sets" start at the drop
level 60. Each price class has its currency - zen or a jewel - with surcharges
per level above +7 and per additional option, and ancient items and the best
excellent items of the higher sets cost more. For each kind of jewel, the
currency (zen or another jewel) and the price are set separately; by default,
the jewels are sold for their value in zen. A store holds at most two jewels of
a kind (`Jewels: maximum per kind`): Jewels of Chaos are the most common loot,
and with more, many stalls offered nothing else. Prices in jewels need the
`Player shop currencies` plugin with these jewels; goods whose currency isn't
allowed are sold to the NPC as before. Players of the original client can only
buy goods for zen.

A zen price isn't exact to the last coin: when a good goes into the store, its
price is drawn within 10 % up or down and rounded the way a player would ask
for it - to 50,000 below a million, to 100,000 below ten million, to whole
millions above. A Jewel of Chaos costs 750,000 at one merchant and 850,000 at
another. The price stays until the good is sold or leaves the store; prices in
jewels stay exact. `Zen price spread` 0 keeps all prices exact.

With enough goods (4 by default, `Minimum goods to start`), a bot goes to the
market for a shift of 30 to 90 minutes: through the map's spawn gate, regardless
of the warp list, to a stall apart from the other merchants - anywhere in the
safezone of the market, at least two tiles away from the walls, so no store
blocks a corridor like the entrance. Some players sell in town instead, and so
do 20 % of the shifts: in Lorencia, Devias, Noria or Elvenland, in the safezone
near the spawn gate, in the middle of the town (`City merchants (%)`, `City map
numbers`); a city without a free stall sends the bot to the market. Every store
gets a name no other open store has. At most 50 bots per game server sell at the
same time (`Maximum merchants`) - a ceiling, not a number of merchants: only
bots with enough goods go, so there are few right after a server start. A bot
leaves early when it sold everything, after 30 minutes without a sale, or when
it logs out; then it warps home and goes back hunting, and its next shift is two
hours later at the earliest. A bot in a party of bots leaves it for the shift,
like a player who goes selling; a party with a player is never left. At most 3
bots start a shift every 30 seconds (`New merchants per 30 seconds`), so after a
server start the market fills up within minutes and the parties don't fall apart
at once. On a shift, a bot declines party invitations.

A bot buys nothing, so the zen and the jewels paid for its goods are removed
right after each sale. There are no discounts - players would learn to wait for
them. A good which found no buyer for three shifts leaves the store and is sold
to the NPC, making room for fresh goods. The server counts these shifts in
memory only: a restart, or a day in which the good wasn't on the market, starts
the count over.

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
grudges. It fights what the event throws at it and keeps up with the
leader. Chaos Castle is a free-for-all, so there the other participants are
targets like everyone else — and a fight inside leaves no grudge outside. A bot
which dies respawns in the safezone like a player, which takes it out of the
event; the survivors are warped out when the event ends.

### Company and rhythm

Bots hunt in parties of two to five, grouped by level so the whole party can
hunt the leader's maps. Each party holds exactly one energy elf buffer and
one of each build at most, and buffers group up whenever a party can be
formed around them. Generation sizes the buffer count to the expected party
count. The elf heals, the buffs are shared,
the party experience bonus applies. Parties re-form every hour.

A player may invite a bot into their own party: it accepts after a human-like
pause of a few seconds, as long as it is not in the middle of an errand. There is
no level gate — just like OpenMU's own party action, a bot accepts an inviter of
any level, since it is the player who invites, and once it joins it stays for the
whole session. A living player takes precedence over the bot's own company
— a bot hunting with other bots leaves them for the inviter, and breaks that bot
party up if it was leading it, so a player never has to guess which bot happens
to be free. In a party the bot follows its leader — in town too, back from a
fight once it falls more than twelve tiles behind, and to another map through the
warp entry closest to the leader, so it lands on the leader's floor. It pays that
warp like a player; one which can't afford it hunts where it stands, without trips
of its own, until it can — and defers a due reset, leaving
only on the engine's own terms: when the party disbands, when it is kicked,
when the leader enters a map it may not access, ten minutes after every human
companion has disconnected without reconnecting, or before the bot's own logout
(the presence rotation or a fault restart stops it).

A bot fights back when a player attacks it, but only as far as the game's own
PvP rules allow: inside the active self-defense window, or against a player
already flagged as a killer. It can therefore never be provoked into becoming an
outlaw that players could farm for free. It remembers who hit it, and a killed
bot walks back to its killer — waiting for a legal opening rather than taking
one.

Over the day, the presence rotation logs bots in and out: fewest in the early
morning, most in the evening, and never more than one at a time, so the
population ebbs and flows instead of appearing and vanishing in blocks.

### Talking

With the separate *Bot chat* plugin (off by default), bots say a line now and
then. The lines come from the plugin's configuration, one list per situation
(event lines, greetings, whisper replies, …); there is no language model. A bot
only talks about what it could actually do:

| Situation | Where | Who says it |
|---|---|---|
| A Blood Castle, Devil Square or Chaos Castle entrance opened | `/post`, then it waits for players | a bot which meets that event's entry requirements — the same check that lets it follow a leader in |
| A Blood Castle, Devil Square or Chaos Castle entrance opened | normal chat in town, no `/post` ("bc open, anyone going?") | the nearest bot which meets the entry requirements, once per town where a player stands - 50 % chance |
| An invasion (White Wizard, Golden, Red Dragon) started | `/post` | a bot which may warp to the invaded map |
| A level milestone (every 50 levels by default) | normal chat, players nearby | the bot which reached it |
| A player entered the game (not after a warp) | normal chat, `{player}` | the nearest bot which sees the player — once per player in 30 minutes |
| A player leveled up or made a reset | normal chat, "gz", `{player}`, `{level}`, `{resets}` | the nearest bot which sees the player — 30 % chance, once per player in 10 minutes |
| A player's kill dropped an excellent or ancient item | normal chat ("nice drop!"), `{item}` | the nearest bot - 50 % chance |
| The bot put on the new wings it earned at a level milestone | normal chat ("finally, my wings!"), `{wings}` | the bot, while a player sees it - 50 % chance; at a level milestone, instead of the level comment |
| A player put on wings | normal chat ("nice wings!"), `{player}`, `{wings}` | the nearest bot which sees the player - 50 % chance, once per player and pair until the server restarts |
| A golden monster, the Red Dragon or the White Wizard appeared near a bot | normal chat ("Golden Goblin here!"), `{monster}` | the nearest bot, while a player sees it - once per map and monster in two minutes |
| A player the bot was in a party with before enters the game or invites it again | normal / party chat ("good to see you again"), `{player}` | the bot - it remembers its party companions until the server restarts, and a companion nearby greets first |
| The bot accepted a player's party invitation | party chat, `{player}` | the bot |
| An event the bot went through with a player's party ended | party chat, `{event}` | one bot per party |
| A player killed the bot (outside of an event) | normal chat, `{player}`, before it respawns — a different line when the same player kills it again | the bot |
| A player mentioned the bot's name in the normal chat next to it | normal chat, the same lines as whisper replies | the bot |
| A player wrote in the party chat (`~`) | party chat, the same lines as whisper replies | the bot whose name is in the message; without a name, one bot of the party - only when a keyword matches |
| Now and then, advertising its goods (see Selling on the market) | normal chat, `{location}`, `{store}` | a bot behind its open store on the market or in a city which a player sees |
| A player whispered to a merchant what it sells ("what are you selling?", "za ile?"), or asked it by name in the normal chat | the same way back: a line of the *merchant whisper replies* which points to its store, `{store}` | a bot behind its open store on the market or in a city |
| A player stands near two bots (every 5 minutes per player) | normal chat: one bot says something, the other answers | two bots close together, both in a town or both out hunting |
| A player asked the bot for a trade or challenged it to a duel | whisper, 70 % chance, `{player}` — the bot turns it down in any case (no trade window, no duels) | the bot |
| A player whispered to the bot | whisper back, after a few seconds — answering the whisper's keywords (see below) | the bot |

A bot which posts about an event does not stay out hunting where nobody
could find it. It goes to the meeting point and waits there until the entrance
closes: next to the Messenger of Archangel for Blood Castle, next to Charon for
Devil Square, and — since Chaos Castle is entered with the ticket alone — in the
safezone of its home town, which the post names (`{map}`). Waiting does not
make it decline party invitations; being invited is the point. A player who
invites it and enters the event takes it along like any party bot. If nobody
invites it, it goes back to hunting when the entrance closes. A bot does not
announce an event whose entrance closes in less than two minutes, and a bot
already in a party with a player does not look for company.

A player can also search party members among the bots with a post:
`/post party 300-350 bk dw`. A post with one of the party words (`party`, `pt`,
`grupa`, …) and a level — `300-350`, `300+`, `min 300` or just `300` — calls up
to five free bots of the game server whose level fits; class words (`dk`, `bk`,
`bm`, `dw`, `elf`, `mg`, `dl`, `sum`, `rf`, … — each meaning its whole class
line) narrow it down. Instead of a level the post may name an event with its
level — `/post party bc7`, `pt ds 5`, `party chaos castle 3` — and then only
bots which may enter that event answer: the event's own level bracket (the
lower one of the Magic Gladiator, Dark Lord, Rage Fighter and Summoner
included) and the master class requirement of the Blood Castle 8, as
configured for the event. Each of them whispers to the player, a few seconds
apart.
The same search posted again calls five others. The player whispers back to a
bot where to meet — `devias 215 40`, a warp of the warp list like
`devias3 227 227` or `losttower7`, a map name alone, or `here` for where the
player stands — and the bot warps there, walks up and waits five minutes for
the invitation. It enters through the warp from which the place is the
shortest walk, like a player would: the floors of the Lost Tower or the
Dungeon can't be walked between at all, and the walk from the town of Devias
to Devias3 takes long. A bot already standing on the map walks to the named
coordinates, even on a map without an entry in the warp list. A place it may
not go to (its level is too low for any warp leading there) it declines, and
the player may name another one while the offer lasts; a whisper without a
place gets asked again.
Map and warp names are recognized in every translation. The words, the number
of bots and the times are part of the configuration.

The conversations make a town feel lived in, like the townsfolk of a
role-playing game: "my potions are running low" — "the shop here sells them
cheap". Only when a human player stands near two bots which stand close
together do they talk — nobody else would hear it. Each conversation has a
place: `Town` (both bots in a safezone), `Hunting` (both outside of it, usually
fighting) or `Anywhere`. They are drawn without repetition until all of a place
were used, and each player hears about one every five minutes. The default
configuration comes with some 190 of them. A conversation may have a third
line - the first bot's riposte. A player who writes in the normal chat within
30 seconds after a conversation next to them gets an answer from one of the two
bots: a whisper reply when the message matches a keyword, otherwise "right?",
"haha, exactly" and the like.

A whisper reply can carry `Keywords`, a comma-separated list of words or
phrases it answers (`how are you, how r u`). The bot picks among the replies
whose keyword matches the whisper — whole words, ignoring case and punctuation,
the longest match winning, so "hi, how are you?" gets an answer to the question
rather than a greeting. When nothing matches, it uses a reply without keywords.
The default lines cover greetings, "how are you", the level and class, where the
bot is, where to hunt, its wings, party, help, trade, thanks and goodbye.

Placeholders in a line are filled in when it is said: `{event}`, `{map}`,
`{level}`, `{class}`, `{player}` (whisper replies), `{partner}` (the other bot of
a conversation), `{wings}` (see below), and `{location}` — the map with the
bot's coordinates the way players write them, e.g. "Devias 215, 40", so a player
can find it with the mini map. The default "where are you?" replies use it. In
an event post, `{location}` is the meeting point; in an invasion post, just the
invaded map. Each line is a localized text, so
every player sees the translation of their own language, or the English text
when it has none. The default lines come with a Polish translation, and the default
keywords understand Polish whispers too.

`{wings}` names the bot's wings with their level, e.g. "Wings of Soul +9"; in a
comment on a player's wings, the player's. A bot without wings doesn't answer
"what wings do you have?" with an empty name: a whisper reply with `{wings}` is
swapped for one of the *answers without wings* ("no wings yet, saving up").

A bot answers a message in the language it is written in: a Polish whisper
gets a Polish answer, an English one an English answer, whatever the player's
account language. The *Message languages* list holds the typical words and
letters by which it tells them apart; a message which doesn't tell ("lol") is
answered in the player's own language. Everything else a bot says - greetings,
posts, conversations - follows the account language:

For the Polish lines to reach a Polish player, three things are needed:

1. **The player's language is set to `pl`** — the *Language Iso Code* of the
   player's account in the admin panel. The game client doesn't tell the server
   its language, and the `/language` command only offers the languages of the
   server messages.
2. **The server has the full culture data.** The Docker images install
   `icu-data-full`; with the English culture data alone (`icu-libs` on Alpine),
   the server doesn't know the `pl` culture and every player gets the English
   lines. A self-built image needs the package too.
3. **The admin panel shows a field for Polish**, to read and edit the
   translations. It shows the languages of its own user interface, every
   language a text already has a translation in, and the additional content
   languages of the `AdminPanel:ContentLanguages` setting — e.g. the environment
   variable `AdminPanel__ContentLanguages__0=pl`. Without the setting, the
   Polish default lines are still shown, but a new line gets no Polish field.

Every line and conversation has a `When`: `Anytime` (the default), `Morning`
(5–11 o'clock), `Day` (11–18), `Evening` (18–23) or `Night` (23–5), by the local
time of the server. A greeting may be "good morning" in the morning and "late
night grind?" at night.

A line can come with a `Gesture` - the emotes of the game client, like a player
typing `/hello`: the bot turns to the player and waves with a greeting or a
goodbye, claps with a "gz", cheers after an event, raises its fist when a
player killed it. The default lines have fitting gestures.

A bot answers like a typing player: it takes a moment to read, then a little
time per character, so a quick "np" comes at once and a whole sentence takes a
few seconds (at most twelve).

The bots don't drown out the players: while the players around a bot are busy
talking (three normal chat messages within 30 seconds, by default), it says
nothing unasked — no conversation, greeting, congratulation or comment. It
still answers a player who talks to it.

A game master can make a bot talk with `/botsay <bot> <text>` - for staged
scenes of an event, or to spark a conversation. The text is said as typed in
the normal chat, to the players who see the bot; only bots can be made to talk.

The limits keep a large population from flooding the chat. All bots of a game
server share one gap between two posts (`Minimum time between posts`, 3 minutes),
each opened event type or started invasion is announced at most once, and each
bot has its own cooldown for the normal chat and for whisper replies. The two
post chances are 30 % by default, so not every opened event or invasion gets a
post. Bots post only while the `/post` command is enabled on the server, and
their messages look exactly like a player's post.

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
- **Populations generated before buffer-planned parties hold too few
  buffers.** Parties need one buffer each and refuse to form without one,
  so an old population mostly hunts solo. Set `Reset bots` to regenerate
  with the planned buffer count; the server log warns at startup while
  buffers are short.

## Enabling it on a server

1. Enable the *Bots* plugin on the [Plugins page](../admin-panel/plugins.md)
   and set the number of accounts. Each account animates up to five characters,
   so 50 accounts × 5 = 250 bots.
2. Check that the population fits. The bots of a game server may occupy
   `Bot capacity %` of its player limit — with the default of 60 %, a server for
   1000 players hosts up to 600 bots. What does not fit stays offline, and the
   plugin says so in the [log](../admin-panel/logs-and-monitoring.md): raise the
   player limit, raise the share, or add a game server, over which the
   population then spreads by itself.
3. Restart the server — or use *Reload configuration and restart all game
   servers* on the [Servers page](../admin-panel/servers.md). The population is
   generated on the first start and reloaded afterwards.
4. To build a fresh population, set `Reset bots`: it deletes the old one,
   generates a new one, and clears the flag again.
5. To stop the bots without losing them, uncheck `Enabled`: they log out within
   a few seconds and nothing is deleted, so checking it again brings the same
   characters back. To get rid of them for good, set `Purge bots` — it deletes
   every bot account with its characters, items and storages, and leaves the
   feature switched off.

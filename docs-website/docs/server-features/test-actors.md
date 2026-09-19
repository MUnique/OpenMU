---
title: Scripted test actors
sidebar_label: Test actors
sidebar_position: 2
description: Connection-less players which execute commands and report what happens to them, for testing game mechanics without a client.
---

# Scripted test actors

A test actor is a character played by a script instead of by a person: it logs
in without a connection, does what a command tells it — walk there, attack
that, cast this, say something, pick that up, warp — and records everything the
game would have shown to its client as a stream of JSON events.

It exists so that game mechanics can be exercised and asserted automatically.
PvP needs two players, a party needs several, and a victim's health is only
known to the victim itself; with actors, all of that is a shell script. The
feature is off unless the server is told to open its control port, and it is
meant for development and test servers, not for a live one.

## How it works

**An actor is a player, not a bot.** It derives from `Player` and repeats the
login sequence of the connection-less offline player (the same class which
keeps a character playing after its owner logs out, see [bots](bots.md)), but
it is deliberately *not* an `OfflinePlayer`: the server-side bots only defend
themselves against players which are not offline players, mini games skip
offline party leaders, and the admin panel lists them as offline accounts. An
actor has to be a human stand-in, so everything which asks "is this a real
player" answers yes.

**Commands run on the actor's own tick.** Each command executes inside the
player's persistence lock, so it never overlaps the periodic save or another
command, and the engine's attribute system is only ever touched from that one
flow. Long commands — a walk across the map, a repeated attack — release the
lock between their steps and can be interrupted: `halt`, or simply the next
command, ends the one in flight, which answers its caller with the progress it
made.

**Every command answers.** A command either succeeds with a result or fails
with a code (`out_of_range`, `safezone`, `not_in_view`, `unknown_skill`,
`no_path`, `interrupted`, …). Nothing is silently ignored — a test which
"passes" because nothing happened is worse than no test. A `walk` only
succeeds when the actor stands on the requested tile; when the engine refuses
or cuts the path short (the path finder and the movement check do not agree on
every tile), it answers `no_path` with the position actually reached. A
numeric field outside its range (`"x":300`) is refused as `bad_request`
instead of being wrapped into a plausible-looking value.

**Events carry the attribution the client protocol does not.** The view a
client receives tells it that it was hit, but not by whom, so hits are recorded
from the `IAttackableGotHitPlugIn` plugin point instead, which knows both
sides. The recorder is not a regular, discoverable plugin: it is registered at
that plugin point by the control endpoint when it starts, so a server which
never enables the actors neither runs it nor lists it. Each hit produces
exactly one event on the attacker's stream and one on the victim's, naming the
other side and its kind (player, bot, monster). Kills,
stat changes (health, shield, mana, ability), chat, drops, map changes and
objects entering or leaving view come from the actor's own recording view
container. Every event has a strictly increasing sequence number and a UTC
timestamp, so a reader can ask for everything after what it already saw, or
follow the stream live.

## The control endpoint

The server opens a TCP port when the environment variable `OPENMU_ACTOR_PORT`
is set to a port number, and does not open it otherwise. It binds the loopback
address (`127.0.0.1`), so the endpoint is reachable from the same machine
only, unless `OPENMU_ACTOR_ADDRESS` explicitly names another address
(`0.0.0.0`, `::1`) — inside a container, for instance, whose port publishing
then decides who can reach it. An invalid port or address is logged and
ignored, and the endpoint stays off. The protocol is newline-delimited JSON:
one request object per line, one response object per line, with a streaming
mode for following events.

```json
{"id":"1","cmd":"spawn","actor":"test1"}
{"ok":true,"id":"1","actor":{"actor":"test1","character":"test1Dk","map":"Lorencia","x":125,"y":132,"alive":true}}
```

The endpoint is registered by the all-in-one `MUnique.OpenMU.Startup` host
only; the distributed (Dapr) game server host does not open it.

The endpoint has **no authentication**. Keep it on a loopback address, never
publish it, and leave the variable unset on any server which is not yours to
test on. An account which is already animated — by another actor, by a
population bot on any game server of the process, or by a connected client — is
refused, so an actor can never drive a character someone else is driving.

## Commands

| Command | What it does |
|---|---|
| `spawn` / `stop` | animate an account's character; log it out again (saving its progress) |
| `list` / `state` / `nearby` | which actors exist; one actor's full state; what it can see |
| `walk x y` | walk to a position of the current map, one path-finder request over the whole map; `no_path` with the reached position when the way turns out to be blocked |
| `attack <id\|name> [--times N] [--interval ms]` | plain attacks against an object in view |
| `skill <number> <id\|name>` | cast a learned skill |
| `say`, `pickup`, `warp` | chat (including chat commands), pick up a drop, use a warp list entry |
| `halt` | cancel the walk or attack in flight, keeping the actor in the world |
| `events [--since N] [--follow]` | the actor's event stream |

## Population bots as opponents

The [bots](bots.md) feature pairs naturally with actors: bots are legal PvP
targets which fight back under the game's own self-defence rules, so an actor
has something to fight without a second script. The endpoint can switch them
on and off (`bots on [--count N]`, `bots off`, `bots status`) through the same
persisted plugin configuration the admin panel edits, and reports where the
animated bots currently are. The switch changes nothing it was not asked for:
`bots on` sets `Enabled`, `--count N` additionally sets the number of bot
accounts, and both survive a restart like any other edit of that
configuration. The characters per account and the presence rotation stay as
the operator configured them; `bots status` reports all three settings, so a
scenario knows what population to expect.

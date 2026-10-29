---
title: Speed hack detection
sidebar_label: Anti-cheat
sidebar_position: 2
description: Movement and attack speed checks, client clock observation, and how to roll them out safely.
---

The *Speed Hack Detection* feature plugin watches the walk and attack pace of
players and compares it with what their character may do. It is an observation
tool first: the stronger responses are opt-in, so a server can trial the checks
on live traffic before it rejects or punishes anything.

The plugin is configured on the
[Plugins page](../admin-panel/plugins.md) of the admin panel, in the "Feature
Plugins" section.

## What is checked

### Walking

The server executes walk steps itself and measures the steps it actually
performed, not the positions a client claims. The cost of the executed steps is
summed over a sliding window and compared with the time which passed plus a
tolerance. A client therefore cannot make the check believe it walked faster by
sending more packets, and network jitter has a whole window in which it evens
out.

A position which the server takes over from the client (the start of a walk or
a stop position) counts as walked distance as well, if it differs from the
server position by more than a tile and a half. Otherwise, a client could
cover distance by reporting a position further ahead with every walk request,
which the executed steps alone would never show. Smaller differences are not
counted, because the server is regularly a tile ahead of or behind the client.

Independent of the plugin, the server bounds how far a reported position may be
from its own:

- At the start of a walk the client may be at most
  `MaxAllowedWalkStartOffset` tiles away (5 by default).
- While the server is still walking the same path, the time since its last step
  bounds the offset, so a client which is a little ahead because of lag is
  accepted without opening a teleport.
- A stop position must lie on the running path, or next to the server position
  where the client rounds a tile. Anything else ends the walk at the server
  position and pulls the client back.

A position outside the bound is rejected and resynchronized; the walk is not
performed. This protection is part of `PlayerMovement` and remains active when
the plugin is disabled.

### Attacking

Attacks are checked with a token bucket. Every attack consumes a token, and
tokens regenerate at the minimum interval for the character's attack or magic
speed. A short burst may use the stored tokens, while a sustained rate above the
limit eventually empties the bucket.

Optional per-skill profiles override the common settings for skills whose
animation or server-side delay differs from the default calculation. A profile
selects the speed attribute, base delay, scaling factor and minimum interval for
one skill. Profiles are looked up by the used skill and its base skill.

Explicit area hits are also checked against the area of the preceding cast. The
hit and area records are keyed by skill and animation counter, so casts of
different skills cannot overwrite each other. The recorded area gets a small
range slack for targets which move before their hit packet arrives.

### Client clock and reported attack speed

Ping packets carry the client tick count and the attack speed the client has
calculated. The plugin compares the tick advance with the server's elapsed time
and the reported attack speed with the server-side value. Both checks only log at
debug level: a mismatch can be caused by lag or a different client, so it never
rejects or punishes on its own.

## Configuration

The defaults are deliberately non-punitive:

| Option | Default | Meaning |
|---|---:|---|
| `AutoBan` | `false` | Ban the account when the warning limit is reached. |
| `DisconnectOnViolation` | `false` | Disconnect on a violation which the policy answers with a disconnect. |
| `ObserveOnly` | `false` | Detect and log, but never reject, warn, disconnect or ban. |
| `MaxWarnings` | `3` | Warnings within the history period before the account is banned. |
| `AlertDebounceSeconds` | `5` | Minimum time between recorded warnings. |
| `WarningHistoryHours` | `1` | How long warnings stay in the player's history. |
| `EnforceWalkSpeed` | `false` | Allow walk-speed violations to reject walks and contribute to sanctions. |
| `WalkSpeedWindowSeconds` | `15` | Length of the sliding walk-speed window. |
| `WalkSpeedToleranceMs` | `2000` | Extra walking time allowed within the window, scaled with the character's step delay. |
| `MaxAllowedWalkStartOffset` | `5` | Maximum client/server offset at the start of a walk. |
| `SkillHitAreaRangeSlack` | `2` | Extra tiles allowed for an explicit-hit target area, for targets which moved after the cast. |
| `SkillHitCounterReplayWindowMs` | `1000` | Window in which the same hit counter counts as a replay. Zero disables the check. |
| `SkillHitMaxAreaToHitDelaySeconds` | `10` | Maximum age of an area attack for its explicit hit packets. Zero disables the check. |
| `ViolationScoreRejectThreshold` | see [Violation policy](#violation-policy) | Score at which a walk violation is rejected instead of softly observed. |
| `ViolationScoreHalfLifeMinutes` | see [Violation policy](#violation-policy) | Half-life of the walk violation score. |
| `MaxAttackTokens` | `5` | Bucket size for the attack check. |
| `AttackSpeedBaseDelayMs` | `450` | Base delay for physical attacks. |
| `AttackSpeedScalingFactor` | `1.2` | Attack speed scaling for physical attacks. |
| `AttackSpeedMinIntervalMs` | `60` | Minimum interval for physical attacks. |
| `MagicSpeedBaseDelayMs` | `450` | Base delay for magic attacks. |
| `MagicSpeedScalingFactor` | `1.2` | Magic speed scaling. |
| `MagicSpeedMinIntervalMs` | `60` | Minimum interval for magic attacks. |
| `PingClockToleranceMs` | `3000` | Allowed difference between client tick advance and server time. |
| `PingAttackSpeedTolerance` | `20` | Allowed difference between reported and calculated attack speed. |

Existing databases which predate a numeric option deserialize it as zero. The
walk window, tolerance thresholds and score settings fall back to their defaults
when a value is not positive, so an upgrade does not silently disable or
over-tighten the checks.

## Violation policy

A walk violation has a severity:

- **Soft** — recorded in the warning history; the walk is still performed.
- **Reject** — the walk is not performed and the client is resynchronized.

Reject is not decided by a single packet. Each violation adds a score
proportional to how far the measured pace exceeded its budget, and the score
halves every `ViolationScoreHalfLifeMinutes`. Only sustained cheating crosses
`ViolationScoreRejectThreshold`; an isolated lag spike does not. Offline players,
bots, spectators and game masters are exempt from punishment, and offline
players are not checked at all because the server paces their actions itself.

## Rolling it out

1. Start with `ObserveOnly` enabled and `AutoBan` and `DisconnectOnViolation`
   disabled. Watch the anti-cheat log entries against known network conditions.
2. Enable `EnforceWalkSpeed` once the window and tolerance fit the server's
   latency and movement speeds. Rejections are still limited to the escalation
   score.
3. Only then consider `DisconnectOnViolation` and `AutoBan`. Warnings are stored
   per player; the account ban happens at `MaxWarnings` inside
   `WarningHistoryHours`.

Walk rejection is a heuristic response to a client which is otherwise
authoritative for its position. If players report rubberbanding, raise the walk
tolerance or the window before increasing punishments.

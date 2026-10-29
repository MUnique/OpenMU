# Castle Siege mini-map and guild commands

During the battle (`Start` state), the guild master of each participating
alliance's master guild receives mini-map updates every three seconds while on
the Castle Siege map. Ordinary guild members and masters of allied member guilds
do not receive these tactical snapshots.

Updates show up to 1,000 players on the commander's own siege side, including
allied guilds, together with living castle gates and Guardian Statues. Players
on other maps and opposing sides are excluded. Updates stop when the battle
ends or the commander leaves the siege map.

The server pushes these updates automatically. The existing client protocol
has no mini-map subscription request or acknowledgment, so opening the UI does
not require a new request packet.

An alliance master can select one of seven command groups and place an attack,
defend, or wait order at a target coordinate. The server relays the selected
group and command to players on the same siege side and map, including the
commander. The group selects a numbered marker slot; it does not select a siege
side. Attempts are limited to one per second per player, including unauthorized
attempts, to keep repeated requests from flooding the siege's shared lock and
other players' connections.

The client must support the Castle Siege commander interface and the player
position (`B6`), NPC position (`BB`), and guild command (`B2:1D`) packets. Within
each update, the player packet replaces the position layer and the NPC packet
adds structures afterward. The commander position is already known locally;
the existing position packet has no separate commander-identity field.

## Player kills during the battle

During the battle, players of different siege sides may attack and kill each
other without becoming outlaws, and the attacks don't start the self-defense.
The attacking sides are enemies of each other as well as of the defenders.
This applies only on the Castle Siege map and only to players with a siege
side. Killing a player of the own side, or any player outside the battle, is
still a normal player kill.

## Friendly fire during the battle

During the battle, players of the same siege side can't damage each other,
including with area skills and pets. This applies only on the Castle Siege map
and only to players with a siege side.

# C1 11 - Hit request (by client)

## Is sent when ##
The client requests to hit another attackable game entity (monster, other player, etc.) with a normal physical hit - no skill is involved.


## Causes the following actions on the server side ##
The server first checks the state of the player. It must have a selected character and therefore on a game map instance.
Then the server checks if the target is on the same game map, and if the attacking player is one of its observers. Otherwise a hacker could just send attack packets for targets placed all over the map.

If all of these checks are done, the target is getting attacked by the player.

The attacking player gets a [response](C111%20-%20Hit%20response.md) which includes the caused damage. If the target is a player as well, it gets the same response, too.

All observing players are getting notified about this attack by an animation message, with the animation which is supplied in this message. The animation can be different for different types of equipped weapons.

## Structure ##

|  Length  | Data type | Value | Description |
|----------|---------|-------------|---------|
| 1 | byte | 0xC1   | [Packet type](PacketTypes.md) |
| 1 | byte | [Length] | Packet header - length of the packet |
| 1 | byte | 0x11   | Packet header - hit packet type identifier |
| 1 | ushort | 0x2001 | Target-Id |
| 1 | byte | 0x78 | Attack animation |
| 1 | byte | 0x00 | Looking direction |

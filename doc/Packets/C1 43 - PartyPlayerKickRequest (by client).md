# C1 43 - PartyPlayerKickRequest (by client)

## Is sent when

A party master wants to kick another player from his party, or when a player wants to kick himself from his party.

## Causes the following actions on the server side

If the sending player is the party master, or the player wants to kick himself, the target player is removed from the party.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x43  | Packet header - packet type identifier |
| 3 | 1 | Byte |  | PlayerIndex |
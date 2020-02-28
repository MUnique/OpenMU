# C1 42 - PartyListRequest (by client)

## Is sent when

When the player opens the party menu in the game client.

## Causes the following actions on the server side

If the player is in a party, the server sends back a list with information about all players of the party.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x42  | Packet header - packet type identifier |
# C1 43 - RemovePartyMember (by server)

## Is sent when

A party member got removed from a party in which the player is in.

## Causes the following actions on the client side

The party member with the specified index is removed from the party list on the user interface.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   4   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x43  | Packet header - packet type identifier |
| 3 | 1 | Byte |  | Index |
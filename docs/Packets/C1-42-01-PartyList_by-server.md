# C1 42 01 - PartyList (by server)

## Is sent when

A player joined a party or requested the current party list by opening the party dialog.

## Causes the following actions on the client side

The party list is updated.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x42  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x01  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | Count |
| 5 | PartyMember.Length * Count | Array of PartyMember |  | Members |

### PartyMember Structure

Data about a party member.

Length: 24 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 10 | String |  | Name |
| 10 | 1 | Byte |  | Index |
| 11 | 1 | Byte |  | MapId |
| 12 | 1 | Byte |  | PositionX |
| 13 | 1 | Byte |  | PositionY |
| 16 | 4 | IntegerLittleEndian |  | CurrentHealth |
| 20 | 4 | IntegerLittleEndian |  | MaximumHealth |
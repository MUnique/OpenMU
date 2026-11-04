# C1 44 - PartyHealthUpdate (by server)

## Is sent when

Periodically, when the health state of the party changed.

## Causes the following actions on the client side

The party health list is updated.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x44  | Packet header - packet type identifier |
| 3 | 1 | Byte |  | Count |
| 4 | PartyMemberHealth.Length * Count | Array of PartyMemberHealth |  | Members |

### PartyMemberHealth Structure

Health of a party member

Length: 1 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 4 bit | Byte |  | Index |
| 0 | 4 bit | Byte |  | Value; A value from 0 to 10 about the health of a player. 10 means the current health is 100% of the maximum health. |
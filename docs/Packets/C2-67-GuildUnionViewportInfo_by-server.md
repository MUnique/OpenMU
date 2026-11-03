# C2 67 - GuildUnionViewportInfo (by server)

## Is sent when

After players of a guild union came into view or the union relationship changed.

## Causes the following actions on the client side

The client updates the union name and relationship of the players.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC2  | [Packet type](PacketTypes.md) |
| 1 | 2 |    Short   |      | Packet header - length of the packet |
| 3 | 1 |    Byte   | 0x67  | Packet header - packet type identifier |
| 4 | 1 | Byte |  | PlayerCount |
| 5 | UnionPlayerInfo.Length * PlayerCount | Array of UnionPlayerInfo |  | Players |

### UnionPlayerInfo Structure

The union information of a player.

Length: 20 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 2 | ShortBigEndian |  | PlayerId |
| 4 | 4 | IntegerLittleEndian |  | GuildId |
| 8 | 1 | Byte |  | Relationship |
| 9 | 8 | String |  | UnionName |
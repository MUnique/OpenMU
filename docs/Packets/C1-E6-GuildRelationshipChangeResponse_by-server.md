# C1 E6 - GuildRelationshipChangeResponse (by server)

## Is sent when

After a guild relationship change was processed by the server.

## Causes the following actions on the client side

The client shows the result of the guild relationship change request.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   9   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xE6  | Packet header - packet type identifier |
| 3 | 1 | Byte |  | Result |
| 4 | 1 | Byte |  | RelationshipType |
| 5 | 4 | IntegerBigEndian |  | TargetGuildId |
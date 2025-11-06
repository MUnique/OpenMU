# C1 E5 - AllianceJoinRequest (by server)

## Is sent when

A guild master requested an alliance with another guild. This message is sent to the target guild master.

## Causes the following actions on the client side

The target guild master gets a message box with the request.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   11   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xE5  | Packet header - packet type identifier |
| 3 | 8 | String |  | RequesterGuildName; The name of the guild requesting the alliance (8 characters, space-padded). |
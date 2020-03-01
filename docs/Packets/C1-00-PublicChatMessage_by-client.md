# C1 00 - PublicChatMessage (by client)

## Is sent when

A player sends a public chat message.

## Causes the following actions on the server side

The message is forwarded to all surrounding players, including the sender.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x00  | Packet header - packet type identifier |
| 3 | 10 | String |  | Character |
| 13 |  | String |  | Message |
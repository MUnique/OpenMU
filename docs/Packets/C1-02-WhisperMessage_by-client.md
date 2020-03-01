# C1 02 - WhisperMessage (by client)

## Is sent when

A player sends a private chat message to a specific target player.

## Causes the following actions on the server side

The message is forwarded to the target player.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x02  | Packet header - packet type identifier |
| 3 | 10 | String |  | ReceiverName |
| 13 |  | String |  | Message |
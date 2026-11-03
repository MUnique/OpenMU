# C1 0C - WhisperResult (by server)

## Is sent when

After a whisper message could not be delivered.

## Causes the following actions on the client side

If Success is false, the client shows a message that the receiver is not online.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   4   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x0C  | Packet header - packet type identifier |
| 3 | 1 | Boolean |  | Success |
# C1 04 - ChatMessage

## Is sent when

This packet is sent by the server after another chat client sent a message to the current chat room.

## Causes the following actions on the client side

The client will show the message.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x04  | Packet header - packet type identifier |
| 3 | 1 | Byte |  | SenderIndex |
| 4 | 1 | Byte |  | MessageLength |
| 5 |  | Binary |  | Message; The message. It's "encrypted" with the 3-byte XOR key (FC CF AB). |
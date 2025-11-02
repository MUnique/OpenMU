# C1 00 - ChatMessage (by server)

## Is sent when

A player sends a chat message.

## Causes the following actions on the client side

The message is shown in the chat box and above the character of the sender.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x00  | Packet header - packet type identifier |
| 2 | 1 | ChatMessageType |  | Type |
| 3 | 10 | String |  | Sender |
| 13 |  | String |  | Message |

### ChatMessageType Enum

Defines the type of a chat message.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Normal | The message is a normal chat message, e.g. public, within a party or guild. |
| 2 | Whisper | The message is sent privately to the receiving player. |
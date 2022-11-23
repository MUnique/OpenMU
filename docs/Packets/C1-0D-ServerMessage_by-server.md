# C1 0D - ServerMessage (by server)

## Is sent when



## Causes the following actions on the client side



## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x0D  | Packet header - packet type identifier |
| 3 | 1 | MessageType |  | Type |
| 4 |  | String |  | Message |

### MessageType Enum

Defines a type of a server message.

| Value | Name | Description |
|-------|------|-------------|
| 0 | GoldenCenter | The message is shown as centered golden message in the client. |
| 1 | BlueNormal | The message is shown as a blue system message. |
| 2 | GuildNotice | The message is a guild notice, centered in green. |
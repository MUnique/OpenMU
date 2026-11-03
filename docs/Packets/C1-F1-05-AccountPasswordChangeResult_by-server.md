# C1 F1 05 - AccountPasswordChangeResult (by server)

## Is sent when

After the player requested to change the account password.

## Causes the following actions on the client side

The client shows a message depending on the result (0 to 3).

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   5   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF1  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x05  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | Result |
# C1 F1 03 - AccountPasswordQuestion (by server)

## Is sent when

After the player requested the security question of the account.

## Causes the following actions on the client side

If Result is 1, the client shows the question.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   35   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF1  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x03  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | Result |
| 5 | 30 | String |  | Question |
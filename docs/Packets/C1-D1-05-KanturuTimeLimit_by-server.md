# C1 D1 05 - KanturuTimeLimit (by server)

## Is sent when

A timed phase begins in the Kanturu event.

## Causes the following actions on the client side

The client starts a countdown timer shown in the Kanturu HUD. The value is divided by 1000 to obtain seconds.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   8   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xD1  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x05  | Packet header - sub packet type identifier |
| 4 | 4 | IntegerLittleEndian |  | TimeLimitMilliseconds; Countdown duration in milliseconds. |
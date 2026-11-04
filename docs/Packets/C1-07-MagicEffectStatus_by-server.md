# C1 07 - MagicEffectStatus (by server)

## Is sent when

A magic effect was added or removed to the own or another player.

## Causes the following actions on the client side

The user interface updates itself. If it's the effect of the own player, it's shown as icon at the top of the interface.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   7   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x07  | Packet header - packet type identifier |
| 3 | 1 | Boolean |  | IsActive |
| 4 | 2 | ShortBigEndian |  | PlayerId |
| 6 | 1 | Byte |  | EffectId |
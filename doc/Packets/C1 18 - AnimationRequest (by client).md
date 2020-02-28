# C1 18 - AnimationRequest (by client)

## Is sent when

A player does any kind of animation.

## Causes the following actions on the server side

The animation number and rotation is forwarded to all surrounding players.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x18  | Packet header - packet type identifier |
| 3 | 1 | Byte |  | Rotation |
| 4 | 1 | Byte |  | AnimationNumber |
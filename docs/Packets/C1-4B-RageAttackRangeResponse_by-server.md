# C1 4B - RageAttackRangeResponse (by server)

## Is sent when

A player (rage fighter) performs the dark side skill on a target and sent a RageAttackRangeRequest.

## Causes the following actions on the client side

The targets are attacked with visual effects.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   16   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x4B  | Packet header - packet type identifier |
| 4 | 2 | ShortLittleEndian |  | SkillId |
| 6 | RageTarget.Length *  | Array of RageTarget |  | Targets |

### RageTarget Structure

Contains the target identifier.

Length: 2 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 2 | ShortLittleEndian | 10000 | TargetId |
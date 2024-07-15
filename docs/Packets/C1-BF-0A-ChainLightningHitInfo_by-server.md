# C1 BF 0A - ChainLightningHitInfo (by server)

## Is sent when

The player applied chain lightning to a target and the server calculated the hits.

## Causes the following actions on the client side

The client shows the chain lightning effect.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xBF  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x0A  | Packet header - sub packet type identifier |
| 4 | 2 | ShortBigEndian |  | SkillNumber |
| 6 | 2 | ShortLittleEndian |  | PlayerId |
| 8 | 1 | Byte |  | TargetCount |
| 10 | ChainTarget.Length * TargetCount | Array of ChainTarget |  | Targets |

### ChainTarget Structure

Contains the target identifier.

Length: 2 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 2 | ShortLittleEndian |  | TargetId |
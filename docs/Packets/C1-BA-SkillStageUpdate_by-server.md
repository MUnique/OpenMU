# C1 BA - SkillStageUpdate (by server)

## Is sent when

After a player started a skill which needs to load up, like Nova.

## Causes the following actions on the client side

The client may show the loading intensity.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   7   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xBA  | Packet header - packet type identifier |
| 3 | 2 | ShortLittleEndian |  | ObjectId |
| 5 | 1 | Byte | 0x28 | SkillNumber |
| 6 | 1 | Byte |  | Stage |
# C3 1E - AreaSkill095 (by client)

## Is sent when

A player is performing an skill which affects an area of the map.

## Causes the following actions on the server side

It's forwarded to all surrounding players, so that the animation is visible. In the original server implementation, no damage is done yet for attack skills - there are separate hit packets.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   7   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x1E  | Packet header - packet type identifier |
| 3 | 1 | Byte |  | SkillIndex; The index of the skill in the skill list. |
| 4 | 1 | Byte |  | TargetX |
| 5 | 1 | Byte |  | TargetY |
| 6 | 1 | Byte |  | Rotation |
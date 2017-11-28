# C1 F3 52 - Master skill level increase request #

## Is sent when ##
When the player wants to increase the level of one of his available master skills (of the master skill tree).


## Causes the following actions on the server side ##
The server checks if the player has a selected character, available master level up points,
the requested skill exists and if this skill is actually a master skill.
If this requirements are met, it further depends if the character has already learned this skill.
If that's not the case already, the server checks if the skill can be learned (e.g. required predecessor
skill learned and increased to level 10) and adds this skill to the character if the requirements are met.
If the requirements are not met, nothing happens, otherwise or when the skill was already available, the
level of the requested skill is increased by one and the available level up points decreased by one as well.

Based on the success of the request, the server sends back a [response](C1F352 - Master skill level increase response.md)

## Structure ##

|  Length  | Data type | Value | Description |
|----------|---------|-------------|---------|
| 1 | byte | 0xC1   | [Packet type](PacketTypes.md) |
| 1 | byte | 0x08   | Packet header - length of the packet |
| 1 | byte | 0xF3   | Packet header - packet type identifier |
| 1 | byte | 0x52   | Packet header - master skill level increase |
| 1 | ushort (BE) | 0xA601 | Master Skill Id |
| 1 | byte | 0x00   | unknown |
| 1 | byte | 0x00   | unknown |

# C1 A3 - Quest Completion Reward

## Is sent when ##
  * The own player completed a quest.
  * Another player completed a quest (to show the animation/effect).


## Causes the following actions on the client side ##
  * Shows an animation/effect.
  * Shows/Applies the reward at the own player.


## Structure ##

|  Length  | Data type | Value | Description |
|----------|---------|-------------|---------|
| 1 | byte | 0xC1    | [Packet type](PacketTypes.md) |
| 1 | byte | 0x07    | Packet header - length of the packet |
| 1 | byte | 0xA3    | Packet header - packet type identifier |
| 2 | short |        | Player id, small endian |
| 1 | byte |         | Reward Type |
| 1 | byte |         | In case of a stat point reward, it contains the added stat points |


### Reward Type

| Value | Reward |
|-------|------------|
|  200  | Adds the defined number of stat points |
|  201  | Changes the character class from 1st to 2nd |
|  202  | Adds the defined number of stat points which are caused to be added because the next class has more points per level |
|  203  | Adds the combo skill capability |
|  204  | Changes the character class from 2nd to 3rd |

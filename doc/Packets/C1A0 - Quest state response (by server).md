# C1 A0 - Request Quest Response

## Is sent when ##
  * The client enters the game with a character.
  * The character completes or cancels a quest.
  * The client [requests](<C1A0 - Request quest state (by client).md>) the state explicitly.


## Causes the following actions on the client side ##
The client knows the current state and can offer certain actions which require the completion of a specific quest.
For example, the learning of skills or availability of the combo skill.


## Structure ##

|  Length  | Data type | Value | Description |
|----------|---------|-------------|---------|
| 1 | byte | 0xC1    | [Packet type](PacketTypes.md) |
| 1 | byte |         | Packet header - length of the packet |
| 1 | byte | 0xA0    | Packet header - packet type identifier |
| 1 | byte |         | Number of all quests, minus 1 (independent of character) |
| 0 ~ 50 | byte[] |  | The quest states |

Webzen did a good job saving some bits, I'll try to explain how it works.

### Quest Index
Every available quest has an unique index in this quest state data. In usual server files this is configured in the Quest.txt.

| Index | Quest Type |
|-------|------------|
|   0   | Scroll of Emperor (Level 150 Quest) |
|   1   | Three Treasures Of Mu (Level 220 Quest, Part 1) |
|   2   | Gain 'Hero Status' (Level 220 Quest, Part 2) |
|   3   | Secret of 'Dark Stone' (Level 220 Quest, Part 3 for Blade Knights, to gain Combo Skill) |
|   4   | Certificate of Strength (Level 400) |
|   5   | Infiltration Of 'Barrack' (Level 400) |
|   6   | Infiltration Of 'Refuge' (Level 400) |

### Quest State
Each Quest does have a state, with the following enumeration:

| State | Description |
|-------|-------------|
|   0   | Undefined |
|   1   | Quest active (in progress) |
|   2   | Quest complete |
|   3   | Quest not done |

### The combined quest states
All of the defined quest types are put into this quest state data field, basically they just use two bytes.
They squeeze the state of 4 quests into one byte, so two bits for each state.
The index of the quest defines the position of this bits in the array.
Examples: 

  * Scroll of Emperor is in progress:
    * Binary: 1111 1101 1111 1111
    * Hex: 0xFD 0xFF

  * Every quest is done, last one is in progress:
    * Binary: 1010 1010 11**01** 1010
    * Hex: 0xAA 0xDA

  * Full example packet: C1 06 A0 06 EA FF
    * 06: 6 Quests defined on the server
    * EA: ‭11_10_10_10‬ -> First 3 quests completed
    * FF: 11_11_11_11 -> none of these completed
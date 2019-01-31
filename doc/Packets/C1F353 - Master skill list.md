# C1 F3 53 - Master skill list #

## Is sent when ##
The client entered the game with a master class character.


## Causes the following actions on the client side ##
The master skill tree data is initialized.
When the character is a master character, it's important to send this message - otherwise old data of another character may be shown.


## Structure ##

|  Length  | Data type | Value | Description |
|----------|---------|-------------|---------|
| 1 | byte | 0xC1   | [Packet type](PacketTypes.md) |
| 1 | byte |    | Packet header - length of the packet |
| 1 | byte | 0xF3   | Packet header - packet type identifier |
| 1 | byte | 0x53   | Packet header - master skill list |
| 3 | byte |  | padding |
| 4 | uint (BE) |  | Master Skill count |

Then for each skill, a block of 12 bytes each:

|  Length  | Data type | Value | Description |
|----------|---------|-------------|---------|
| 1 | byte |  | Master skill index on the client, depends on character class |
| 1 | byte |  | Master skill level |
| 2 | byte |  | padding |
| 4 | float |  | current value of the skill effect |
| 4 | float |  | value of next level of the skill effect |

### Master skill index

Each possible visible slot of a master skill in the master skill tree is identified
by an index. Each root has 36 slots (4 * 9), and roots are indexed one after another,
that means left root starts at index 1, middle root at 37, right root at 73.
As this display can differ between character classes, we have to pass it in here.
To me, it's a mystery why Webzen can't work with the skill number alone and determine
this index on client side.

I decided to put this indexes into [code](../../src/GameServer/RemoteView/MasterSkillExtensions.cs) since it would pollute the data model too much.
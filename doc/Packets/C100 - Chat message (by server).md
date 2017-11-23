# C1 00 - Chat message (by server) #

## Is sent when ##
A player sends a chat message, which is forwarded to another player which receives this message.


## Causes the following actions on the client side ##
The message is displayed in the chat message area.


## Structure ##

|  Length  | Data type | Value | Description |
|----------|---------|-------------|---------|
| 1 | byte | 0xC1   | [Packet type](PacketTypes.md) |
| 1 | byte | [Length] | Packet header - length of the packet |
| 1 | byte | 0x00   | Packet header - packet type identifier |
| 10| string |     | Sender player name string, terminated by 0x00 if shorter than 10 |
| n | string |     | The message, terminated by 0x00 if shorter than n |


### Additional Information ###

Depending on the message prefix, the message was sent to the following group of players:

| Prefix | Recipients |
|--------|------------|
|[No Prefix]| All players who observe (see) the sending player |
|@| All online guild members on all sub-servers|
|@@| All online alliance guild members on all sub-servers|
|~| All party members of the party the player is currently in |
|$| All online gen members on all sub-servers|
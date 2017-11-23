# C1 00 - Chat message (by client) #

## Is sent when ##
A player sends a chat message.


## Causes the following actions on the server side ##
The message is forwarded to other players (and the player itself), except the message is a command (see additional information) for the server.


## Structure ##

|  Length  | Data type | Value | Description |
|----------|---------|-------------|
| 1 | byte | 0xC1   | [Packet type](PacketTypes.md) |
| 1 | byte | [Length] | Packet header - length of the packet |
| 1 | byte | 0x00   | Packet header - packet type identifier |
| 10| string |     | Sender player name string, terminated by 0x00 if shorter than 10 |
| n | string |     | The message, terminated by 0x00 if shorter than n |


### Additional Information ###

Depending on the message prefix, the message is sent to the following other players:

| Prefix | Recipients |
|--------|------------|
|[No Prefix]| All players who observe (see) this player|
|@| All online guild members on all sub-servers|
|@@| All online alliance guild members on all sub-servers|
|~| All party members of the party the player is currently in |
|$| All online gen members on all sub-servers|
|/| Nobody. It is the prefix for commands. For available commands, check [this: TODO] documentation |
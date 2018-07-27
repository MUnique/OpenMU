# C1 A2 - Set quest state response

## Is sent when ##
As response to the [set quest state request](<C1A2 - Set quest state request (by client)>).

## Causes the following actions on the client side ##
The client shows the new quest state.


## Structure ##

|  Length  | Data type | Value | Description |
|----------|---------|-------------|---------|
| 1 | byte | 0xC1    | [Packet type](PacketTypes.md) |
| 1 | byte |         | Packet header - length of the packet |
| 1 | byte | 0xA2    | Packet header - packet type identifier |
| 1 | byte |         | Index of the quest |
| 1 | byte |         | The result of the request. |
| 1 | byte |         | New actual state of the quest |

The values of the index and state of the quest are documented [here](<C1A0 - Quest state response (by server).md>).

### Result

| Value | Description |
|-------|-------------|
|   0   | Success     |
|   1   | Failed, Required Item to finish the quest not found yet |
| 0xFF  | Failed, e.g. quest not found      |
|   n   | The "StartContext" of a condition which wasn't met, when trying to start a quest |

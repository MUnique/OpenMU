# C1 11 - Hit response (by server)

## Is sent when ##
The player of the connected client got hit or when a previously attacked target got hit.


## Causes the following actions on the client side ##
It shows the caused damage at the specified target object.

## Structure ##

|  Length  | Data type | Value | Description |
|----------|---------|-------------|---------|
| 1 | byte | 0xC1   | [Packet type](PacketTypes.md) |
| 1 | byte | 0x0A | Packet header - length of the packet |
| 1 | byte | 0x11   | Packet header - hit packet type identifier |
| 1 | ushort | 0x2001 | Target-Id |
| 1 | byte | 0x00 | Damage attributes |
| 1 | ushort |  | Damage to health attribute |
| 1 | ushort |  | Damage to shield attribute |


### Damage Attributes
Depending on the chances of some applied damage attributes, there are flags which are set in the damage attributes field.
A value of 0x00 means, it's a normal hit without any special attributes.

The first 4 bits are used for the kind of the damage.
| Value | Description |
|-------|-------------|
| 0x01  | Damage was calculated by ignoring the defense of the target |
| 0x02  | Damage was calculated with the excellent damage bonus of 20 % base damage |
| 0x03  | Damage was calculated with the maximum base damage (critical damage) |

The next attributes are used when the hit is applied multiple times. They can be OR-combined with the previous values.
| Value | Description |
|-------|-------------|
| 0x40  | Damage was applied twice (Double damage) |
| 0x80  | Damage was applied three times (Triple damage), e.g. by doing a combo skill combination |

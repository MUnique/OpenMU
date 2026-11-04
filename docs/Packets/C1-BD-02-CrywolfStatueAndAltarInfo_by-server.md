# C1 BD 02 - CrywolfStatueAndAltarInfo (by server)

## Is sent when

Every two seconds while the altars can be contracted and during the battle.

## Causes the following actions on the client side

The client shows the shield of the statue and the states of the altars.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   16   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xBD  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x02  | Packet header - sub packet type identifier |
| 4 | 4 | IntegerLittleEndian |  | StatueHealthPercent; The shield of the statue in percent, from 0 to 100. |
| 8 | 1 | Byte |  | Altar1State; The state of the first altar (205). The high nibble is the altar state (1 = contracted), the low nibble the number of remaining contracts. |
| 9 | 1 | Byte |  | Altar2State; The state of the second altar (206), like Altar1State. |
| 10 | 1 | Byte |  | Altar3State; The state of the third altar (207), like Altar1State. |
| 11 | 1 | Byte |  | Altar4State; The state of the fourth altar (208), like Altar1State. |
| 12 | 1 | Byte |  | Altar5State; The state of the fifth altar (209), like Altar1State. |
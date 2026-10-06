# C1 BD 03 - CrywolfContractResult (by server)

## Is sent when

The player requested to contract an altar.

## Causes the following actions on the client side

On success, the client shows that the player is a guardian of the altar, and lets the character pray. Otherwise, it shows why the contract failed.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   8   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xBD  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x03  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | Result; The result of the contract: 1 when it was accepted, 0 otherwise. The client compares the whole byte, so it's no boolean field. |
| 5 | 1 | Byte |  | AltarState; The new state of the altar, like in the CrywolfStatueAndAltarInfo. |
| 6 | 2 | ShortBigEndian |  | AltarKey; The key of the altar. The client uses the key minus 317 as index into its five altar states without checking the range, so it must be 317 plus the index of the altar (0 to 4). |
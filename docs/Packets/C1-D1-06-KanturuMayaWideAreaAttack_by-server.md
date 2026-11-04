# C1 D1 06 - KanturuMayaWideAreaAttack (by server)

## Is sent when

The Maya body executes a wide-area attack during the Maya battle phase.

## Causes the following actions on the client side

The client calls MayaSceneMayaAction(type) which plays one of two visual sequences on the Maya body model: storm (0) or stone-rain (1). This is a purely cosmetic packet — damage is handled server-side.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   7   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xD1  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x06  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | ObjClassH; High byte of the Maya object class; ignored by the client. |
| 5 | 1 | Byte |  | ObjClassL; Low byte of the Maya object class; ignored by the client. |
| 6 | 1 | AttackType |  | Type |

### AttackType Enum

Visual type of the Maya wide-area attack.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Storm | Stone-storm effect (MODEL_STORM3 plus falling debris around the hero). |
| 1 | Rain | Stone-rain effect (MODEL_MAYASTONE projectiles falling on the hero). |
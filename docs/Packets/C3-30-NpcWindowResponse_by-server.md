# C3 30 - NpcWindowResponse (by server)

## Is sent when

After the client talked to an NPC which should cause a dialog to open on the client side.

## Causes the following actions on the client side

The client opens the specified dialog.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   11   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x30  | Packet header - packet type identifier |
| 3 | 1 | NpcWindow |  | Window |

### NpcWindow Enum

Defines the kind of npc window which should be shown on the client.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Merchant | A merchant window. |
| 1 | Merchant1 | Another merchant window. |
| 2 | VaultStorage | A vault storage. |
| 3 | ChaosMachine | A chaos machine window. |
| 4 | DevilSquare | A devil square window. |
| 6 | BloodCastle | A blood castle window. |
| 7 | PetTrainer | The pet trainer window. |
| 9 | Lahap | The lahap window. |
| 12 | CastleSeniorNPC | The castle senior window. |
| 17 | ElphisRefinery | The elphis refinery window. |
| 18 | RefineStoneMaking | The refine stone making window. |
| 19 | RemoveJohOption | The jewel of harmony option removal window. |
| 20 | IllusionTemple | The illusion temple window. |
| 21 | ChaosCardCombination | The chaos card combination window. |
| 22 | CherryBlossomBranchesAssembly | The cherry blossom branches assembly window. |
| 23 | SeedMaster | The seed master window. |
| 24 | SeedResearcher | The seed researcher window. |
| 25 | StatReInitializer | The stat reinitializer window. |
| 32 | DelgadoLuckyCoinRegistration | The delgado lucky coin registration window. |
| 33 | DoorkeeperTitusDuelWatch | The doorkeeper titus duel watch window. |
| 35 | LugardDoppelgangerEntry | The lugard doppelganger entry window. |
| 36 | JerintGaionEvententry | The jerint gaion event entry window. |
| 37 | JuliaWarpMarketServer | The julia warp market server window. |
| 38 | CombineLuckyItem | The dialog window which allows to exchange or refine Lucky Item. Used by NPC "David". |
# C1 A9 - PetInfoRequest (by client)

## Is sent when

The player hovers over a pet. The client sends this request to retrieve information (level, experience) of the pet (dark raven, horse).

## Causes the following actions on the server side

The server sends a PetInfoResponse.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   6   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xA9  | Packet header - packet type identifier |
| 3 | 1 | PetType |  | Pet |
| 4 | 1 | StorageType |  | Storage |
| 5 | 1 | Byte |  | ItemSlot |

### PetType Enum

Describes the type of pet.

| Value | Name | Description |
|-------|------|-------------|
| 0 | DarkRaven | The dark raven pet. |
| 1 | DarkHorse | The dark horse pet. |

### StorageType Enum

Describes the type of storage.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Inventory | The inventory of the player. |
| 1 | Vault | The vault of the player. |
| 2 | TradeOwn | The own trading storage. |
| 3 | TradeOther | The trading storage of the other player. |
| 4 | Crafting | The crafting storage of the player. |
| 5 | PersonalShop | The shop storage of another player. |
| 254 | InventoryPetSlot | The inventory slot of the pet. That's used when a pet leveled up. |
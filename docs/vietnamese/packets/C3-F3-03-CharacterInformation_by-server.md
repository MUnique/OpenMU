# C3 F3 03 - CharacterInformation (server gửi)

## Được gửi khi nào

Sau khi người chơi chọn nhân vật và nhân vật vào game thành công.

## Hành động phía client

Nhân vật vào game world.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 72 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0xF3 | Packet header - packet type identifier |
| 3 | 1 | Byte | 0x03 | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | X |
| 5 | 1 | Byte |  | Y |
| 6 | 2 | ShortLittleEndian |  | MapId |
| 8 | 8 | LongBigEndian |  | CurrentExperience |
| 16 | 8 | LongBigEndian |  | ExperienceForNextLevel |
| 24 | 2 | ShortLittleEndian |  | LevelUpPoints |
| 26 | 2 | ShortLittleEndian |  | Strength |
| 28 | 2 | ShortLittleEndian |  | Agility |
| 30 | 2 | ShortLittleEndian |  | Vitality |
| 32 | 2 | ShortLittleEndian |  | Energy |
| 34 | 2 | ShortLittleEndian |  | CurrentHealth |
| 36 | 2 | ShortLittleEndian |  | MaximumHealth |
| 38 | 2 | ShortLittleEndian |  | CurrentMana |
| 40 | 2 | ShortLittleEndian |  | MaximumMana |
| 42 | 2 | ShortLittleEndian |  | CurrentShield |
| 44 | 2 | ShortLittleEndian |  | MaximumShield |
| 46 | 2 | ShortLittleEndian |  | CurrentAbility |
| 48 | 2 | ShortLittleEndian |  | MaximumAbility |
| 52 | 4 | IntegerLittleEndian |  | Money |
| 56 | 1 | CharacterHeroState |  | HeroState |
| 57 | 1 | CharacterStatus |  | Status |
| 58 | 2 | ShortLittleEndian |  | UsedFruitPoints |
| 60 | 2 | ShortLittleEndian |  | MaxFruitPoints |
| 62 | 2 | ShortLittleEndian |  | Leadership |
| 64 | 2 | ShortLittleEndian |  | UsedNegativeFruitPoints |
| 66 | 2 | ShortLittleEndian |  | MaxNegativeFruitPoints |
| 68 | 1 | Byte |  | InventoryExtensions |

### Enum CharacterHeroState

Định nghĩa trạng thái hero của nhân vật.

| Value | Name | Description |
|-------|------|-------------|
| 0 | New | Nhân vật mới, trạng thái cao nhất. |
| 1 | Hero | Nhân vật ở trạng thái Hero. |
| 2 | LightHero | Nhân vật vẫn là Hero nhưng trạng thái gần hết. |
| 3 | Normal | Trạng thái trung tính. |
| 4 | PlayerKillWarning | Nhân vật đã PK và có cảnh báo. |
| 5 | PlayerKiller1stStage | Đã PK 2 nhân vật, có một số hạn chế. |
| 6 | PlayerKiller2ndStage | PK nhiều hơn 2 nhân vật, bị hạn chế nặng. |

### Enum CharacterStatus

Trạng thái của nhân vật.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Normal | Trạng thái bình thường. |
| 1 | Banned | Nhân vật bị cấm chơi. |
| 32 | GameMaster | Nhân vật là game master. |

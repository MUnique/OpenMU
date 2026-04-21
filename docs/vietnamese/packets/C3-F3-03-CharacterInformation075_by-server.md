# C3 F3 03 - CharacterInformation075 (server gửi)

## Được gửi khi nào

Sau khi người chơi đã chọn nhân vật và vào game.

## Hành động phía client

Nhân vật bước vào thế giới game.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 42 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0xF3 | Packet header - packet type identifier |
| 3 | 1 | Byte | 0x03 | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | X |
| 5 | 1 | Byte |  | Y |
| 6 | 1 | Byte |  | MapId |
| 8 | 4 | IntegerLittleEndian |  | CurrentExperience |
| 12 | 4 | IntegerLittleEndian |  | ExperienceForNextLevel |
| 16 | 2 | ShortLittleEndian |  | LevelUpPoints |
| 18 | 2 | ShortLittleEndian |  | Strength |
| 20 | 2 | ShortLittleEndian |  | Agility |
| 22 | 2 | ShortLittleEndian |  | Vitality |
| 24 | 2 | ShortLittleEndian |  | Energy |
| 26 | 2 | ShortLittleEndian |  | CurrentHealth |
| 28 | 2 | ShortLittleEndian |  | MaximumHealth |
| 30 | 2 | ShortLittleEndian |  | CurrentMana |
| 32 | 2 | ShortLittleEndian |  | MaximumMana |
| 36 | 4 | IntegerLittleEndian |  | Money |
| 40 | 1 | CharacterHeroState |  | HeroState |
| 41 | 1 | CharacterStatus |  | Status |

### Enum CharacterHeroState

Định nghĩa trạng thái hero của nhân vật.

| Value | Name | Description |
|-------|------|-------------|
| 0 | New | Nhân vật mới và ở trạng thái cao nhất. |
| 1 | Hero | Nhân vật là hero. |
| 2 | LightHero | Nhân vật là hero, nhưng trạng thái gần như hết. |
| 3 | Normal | Nhân vật ở trạng thái trung lập. |
| 4 | PlayerKillWarning | Nhân vật đã giết người chơi khác và có cảnh báo PK. |
| 5 | PlayerKiller1stStage | Nhân vật đã giết hai người chơi và có một số hạn chế. |
| 6 | PlayerKiller2ndStage | Nhân vật đã giết hơn hai người chơi và có hạn chế nặng. |

### Enum CharacterStatus

Trạng thái của nhân vật.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Normal | Trạng thái nhân vật bình thường. |
| 1 | Banned | Nhân vật bị cấm vào game. |
| 32 | GameMaster | Nhân vật là game master. |

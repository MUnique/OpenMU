# C2 12 - AddCharactersToScope095 (server gửi)

## Được gửi khi nào

Một hoặc nhiều nhân vật (character) đi vào phạm vi quan sát của người chơi.

## Hành động phía client

Client thêm các nhân vật đó vào bản đồ đang hiển thị.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC2 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 2 | Short |  | Packet header - độ dài packet |
| 3 | 1 | Byte | 0x12 | Packet header - packet type identifier |
| 4 | 1 | Byte |  | CharacterCount |
| 5 | CharacterData.Length * CharacterCount | Array of CharacterData |  | Characters |

### Cấu trúc CharacterData

Chứa dữ liệu của một nhân vật trong phạm vi.

Độ dài: 31 byte

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 2 | ShortBigEndian |  | Id |
| 2 | 1 | Byte |  | CurrentPositionX |
| 3 | 1 | Byte |  | CurrentPositionY |
| 4 | 13 | Binary |  | Appearance |
| 17 << 0 | 1 bit | Boolean |  | IsPoisoned |
| 17 << 1 | 1 bit | Boolean |  | IsIced |
| 17 << 2 | 1 bit | Boolean |  | IsDamageBuffed |
| 17 << 3 | 1 bit | Boolean |  | IsDefenseBuffed |
| 18 | 10 | String |  | Name |
| 28 | 1 | Byte |  | TargetPositionX |
| 29 | 1 | Byte |  | TargetPositionY |
| 30 | 4 bit | Byte |  | Rotation |
| 30 << 0 | 4 bit | CharacterHeroState |  | HeroState |

### Enum CharacterHeroState

Định nghĩa trạng thái hero của nhân vật.

| Value | Name | Description |
|-------|------|-------------|
| 0 | New | Nhân vật mới và ở trạng thái cao nhất. |
| 1 | Hero | Nhân vật là hero. |
| 2 | LightHero | Nhân vật là hero, nhưng trạng thái gần như hết. |
| 3 | Normal | Nhân vật ở trạng thái trung lập. |
| 4 | PlayerKillWarning | Nhân vật đã giết nhân vật khác và có cảnh báo PK. |
| 5 | PlayerKiller1stStage | Nhân vật đã giết hai nhân vật và có một số hạn chế. |
| 6 | PlayerKiller2ndStage | Nhân vật đã giết hơn hai nhân vật và có hạn chế nặng. |

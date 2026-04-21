# C2 12 - AddCharactersToScope075 (server gửi)

## Được gửi khi nào

Một hoặc nhiều nhân vật đi vào phạm vi quan sát của người chơi.

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

Độ dài: 27 byte

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 2 | ShortBigEndian |  | Id |
| 2 | 1 | Byte |  | CurrentPositionX |
| 3 | 1 | Byte |  | CurrentPositionY |
| 4 | 9 | Binary |  | Appearance |
| 13 << 0 | 1 bit | Boolean |  | IsPoisoned |
| 13 << 1 | 1 bit | Boolean |  | IsIced |
| 13 << 2 | 1 bit | Boolean |  | IsDamageBuffed |
| 13 << 3 | 1 bit | Boolean |  | IsDefenseBuffed |
| 14 | 10 | String |  | Name |
| 24 | 1 | Byte |  | TargetPositionX |
| 25 | 1 | Byte |  | TargetPositionY |
| 26 | 4 bit | Byte |  | Rotation |
| 26 << 0 | 4 bit | CharacterHeroState |  | HeroState |

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

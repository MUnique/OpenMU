# C2 45 - AddTransformedCharactersToScope075 (server gửi)

## Được gửi khi nào

Người chơi đang đeo nhẫn biến thân quái (transformation ring).

## Hành động phía client

Nhân vật hiển thị dạng quái, xác định bởi thuộc tính Skin.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC2 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 2 | Short |  | Packet header - độ dài packet |
| 3 | 1 | Byte | 0x45 | Packet header - packet type identifier |
| 4 | 1 | Byte |  | CharacterCount |
| 5 | CharacterData.Length * CharacterCount | Array of CharacterData |  | Characters |

### Cấu trúc CharacterData

Chứa dữ liệu của một nhân vật đã biến thân.

Độ dài: 19 byte

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 2 | ShortBigEndian |  | Id |
| 2 | 1 | Byte |  | CurrentPositionX |
| 3 | 1 | Byte |  | CurrentPositionY |
| 4 | 1 | Byte |  | Skin |
| 5 << 0 | 1 bit | Boolean |  | IsPoisoned |
| 5 << 1 | 1 bit | Boolean |  | IsIced |
| 5 << 2 | 1 bit | Boolean |  | IsDamageBuffed |
| 5 << 3 | 1 bit | Boolean |  | IsDefenseBuffed |
| 6 | 10 | String |  | Name |
| 16 | 1 | Byte |  | TargetPositionX |
| 17 | 1 | Byte |  | TargetPositionY |
| 18 | 4 bit | Byte |  | Rotation |
| 18 << 0 | 4 bit | CharacterHeroState |  | HeroState |

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

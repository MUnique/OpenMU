# C1 F3 01 - CreateCharacter (client gửi)

## Được gửi khi nào

Khi client đang ở màn hình chọn nhân vật và người chơi yêu cầu tạo nhân vật mới.

## Hành động phía server

Server kiểm tra người chơi có đủ điều kiện tạo nhân vật hay không và gửi phản
hồi về client.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 15 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0xF3 | Packet header - packet type identifier |
| 3 | 1 | Byte | 0x01 | Packet header - sub packet type identifier |
| 4 | 10 | String |  | Name; tên nhân vật cần tạo. |
| 14 << 2 | 6 bit | CharacterClassNumber |  | Class; class nhân vật cần tạo. |

### Enum CharacterClassNumber

Giá trị class nhân vật ở phía client.

| Value | Name | Description |
|-------|------|-------------|
| 0 | DarkWizard | Dark Wizard |
| 2 | SoulMaster | Soul Master |
| 3 | GrandMaster | Grand Master |
| 4 | DarkKnight | Dark Knight |
| 6 | BladeKnight | Blade Knight |
| 7 | BladeMaster | Blade Master |
| 8 | FairyElf | Fairy Elf |
| 10 | MuseElf | Muse Elf |
| 11 | HighElf | High Elf |
| 12 | MagicGladiator | Magic Gladiator |
| 13 | DuelMaster | Duel Master |
| 16 | DarkLord | Dark Lord |
| 17 | LordEmperor | Lord Emperor |
| 20 | Summoner | Summoner |
| 22 | BloodySummoner | Bloody Summoner |
| 23 | DimensionMaster | Dimension Master |
| 24 | RageFighter | Rage Fighter |
| 25 | FistMaster | Fist Master |

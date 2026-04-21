# C1 F3 00 - CharacterList075 (server gửi)

## Được gửi khi nào

Sau khi client yêu cầu, thường là sau khi đăng nhập thành công.

## Hành động phía client

Client hiển thị danh sách nhân vật khả dụng của tài khoản.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte |  | Packet header - độ dài packet |
| 2 | 1 | Byte | 0xF3 | Packet header - packet type identifier |
| 3 | 1 | Byte | 0x00 | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | CharacterCount |
| 5 | CharacterData.Length * CharacterCount | Array of CharacterData |  | Characters |

### Cấu trúc CharacterData

Dữ liệu của một nhân vật trong danh sách.

Độ dài: 24 byte

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte |  | SlotIndex |
| 1 | 10 | String |  | Name |
| 11 | 2 | ShortBigEndian |  | Level |
| 13 | 4 bit | CharacterStatus |  | Status |
| 13 << 4 | 1 bit | Boolean |  | IsItemBlockActive |
| 14 | 9 | Binary |  | Appearance |

### Enum CharacterStatus

Trạng thái của nhân vật.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Normal | Trạng thái nhân vật bình thường. |
| 1 | Banned | Nhân vật bị cấm vào game. |
| 32 | GameMaster | Nhân vật là game master. |

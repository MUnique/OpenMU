# C1 F3 01 - CharacterCreationSuccessful (server gửi)

## Được gửi khi nào

Sau khi server xử lý thành công yêu cầu tạo nhân vật.

## Hành động phía client

Nhân vật mới xuất hiện trong danh sách nhân vật.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 42 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0xF3 | Packet header - packet type identifier |
| 3 | 1 | Byte | 0x01 | Packet header - sub packet type identifier |
| 4 | 1 | Boolean | true | Success |
| 5 | 10 | String |  | CharacterName |
| 15 | 1 | Byte |  | CharacterSlot |
| 16 | 2 | ShortLittleEndian |  | Level |
| 18 << 3 | 1 | CharacterClassNumber |  | Class |
| 19 | 1 | Byte |  | CharacterStatus |
| 20 |  | Binary |  | PreviewData |

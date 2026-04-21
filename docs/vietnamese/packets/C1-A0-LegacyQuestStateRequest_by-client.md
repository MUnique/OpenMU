# C1 A0 - LegacyQuestStateRequest (client gửi)

## Được gửi khi nào

Sau khi người chơi bước vào thế giới game với một nhân vật.

## Hành động phía server

Trạng thái nhiệm vụ được gửi lại dưới dạng phản hồi.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 3 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xA0 | Tiêu đề gói - mã định danh loại gói |
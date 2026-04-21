# C1 B2 02 - CastleSiegeUnregisterRequest (client gửi)

## Được gửi khi nào

Người chơi đã mở một NPC bao vây lâu đài để hủy đăng ký liên minh bang hội của mình.

## Hành động phía server

Máy chủ trả về kết quả hủy đăng ký cuộc vây hãm lâu đài.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 5 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xB2 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x02 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 1 | Boolean | true | đang cho đi |
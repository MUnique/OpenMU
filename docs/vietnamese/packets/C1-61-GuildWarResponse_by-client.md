# C1 61 - GuildWarResponse (client gửi)

## Được gửi khi nào

Một chủ bang hội đã yêu cầu một cuộc chiến bang hội chống lại một bang hội khác.

## Hành động phía server

Nếu chủ bang hội xác nhận, cuộc chiến sẽ được tuyên bố.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 4 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x61 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Boolean |  | Đã chấp nhận |
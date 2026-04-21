# C1 BF 05 - IllusionTempleRewardRequest (client gửi)

## Được gửi khi nào

Người chơi yêu cầu phần thưởng của sự kiện.

## Hành động phía server

Máy chủ kiểm tra xem người chơi có đang chơi trò chơi chiến thắng hay không và trả lại phần thưởng, thường là dưới dạng vật phẩm rơi ra.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 4 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xBF | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x05 | Tiêu đề gói - mã định danh loại gói phụ |
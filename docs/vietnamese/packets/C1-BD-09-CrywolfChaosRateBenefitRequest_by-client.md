# C1 BD 09 - CrywolfChaosRateBenefitRequest (client gửi)

## Được gửi khi nào

Người chơi mở hộp thoại chế tạo vật phẩm, ví dụ: cỗ máy hỗn loạn.

## Hành động phía server

Máy chủ trả về dữ liệu về trạng thái lợi ích của sự kiện crywolf. Nếu giành chiến thắng trước đó, tỷ lệ hỗn loạn sẽ tăng lên vài phần trăm.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 4 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xBD | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x09 | Tiêu đề gói - mã định danh loại gói phụ |
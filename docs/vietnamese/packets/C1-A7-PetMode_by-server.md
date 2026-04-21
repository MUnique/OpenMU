# C1 A7 - PetMode (server gửi)

## Được gửi khi nào

Sau khi khách hàng gửi PetAttackCommand (dưới dạng xác nhận) hoặc khi lệnh trước đó kết thúc và thú cưng được đặt lại về chế độ Bình thường.

## Hành động phía client

Máy khách cập nhật chế độ thú cưng trong giao diện người dùng.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 7 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xA7 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | ClientToServer.PetType | ClientToServer.PetType.DarkRaven | Thú cưng |
| 4 | 1 | ClientToServer.PetCommandMode |  | Thú cưngLệnhChế độ |
| 5 | 2 | ShortBigEndian |  | Id mục tiêu |
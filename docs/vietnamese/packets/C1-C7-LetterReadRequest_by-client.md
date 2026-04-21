# C1 C7 - LetterReadRequest (client gửi)

## Được gửi khi nào

Người chơi yêu cầu đọc một chữ cái cụ thể trong danh sách chữ cái của mình.

## Hành động phía server

Máy chủ sẽ gửi lại nội dung thư được yêu cầu cho ứng dụng khách trò chơi.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 6 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xC7 | Tiêu đề gói - mã định danh loại gói |
| 4 | 2 | ShortLittleEndian |  | Thư mục lục |
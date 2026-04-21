# C4 C7 - OpenLetterExtended (server gửi)

## Được gửi khi nào

Sau khi người chơi yêu cầu đọc một lá thư.

## Hành động phía client

Bức thư được mở trong một hộp thoại mới.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC4 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 2 | Short |  | Tiêu đề gói - chiều dài của gói |
| 3 | 1 | Byte | 0xC7 | Tiêu đề gói - mã định danh loại gói |
| 4 | 2 | ShortLittleEndian |  | Thư mục lục |
| 6 | 42 | Binary |  | Người gửiGiao diện |
| 48 | 1 | Byte |  | Xoay |
| 49 | 1 | Byte |  | Hoạt hình |
| 50 |  | String |  | Tin nhắn |
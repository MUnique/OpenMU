# C1 66 - GuildInformation (server gửi)

## Được gửi khi nào

Một ứng dụng khách trò chơi đã yêu cầu thông tin (công khai) của bang hội, ví dụ: khi nó gặp một người chơi của bang hội chưa từng được biết đến trước đó.

## Hành động phía client

Những người chơi thuộc bang hội được hiển thị là người chơi của bang hội.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 60 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x66 | Tiêu đề gói - mã định danh loại gói |
| 4 | 4 | IntegerLittleEndian |  | Id bang hội |
| 8 | 1 | Byte |  | Loại bang hội |
| 9 | 8 | String |  | Tên bang hội liên minh |
| 17 | 8 | String |  | Tên bang hội |
| 25 | 32 | Binary |  | biểu tượng |
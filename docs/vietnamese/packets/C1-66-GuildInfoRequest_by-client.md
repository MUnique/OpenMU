# C1 66 - GuildInfoRequest (client gửi)

## Được gửi khi nào

Một người chơi đưa một người chơi khác vào phạm vi xem thuộc bang hội và mã định danh bang hội không xác định (= chưa được lưu vào bộ nhớ đệm theo yêu cầu trước đó) đối với anh ta.

## Hành động phía server

Máy chủ gửi phản hồi bao gồm tên bang hội và biểu tượng.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 8 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x66 | Tiêu đề gói - mã định danh loại gói |
| 4 | 4 | IntegerLittleEndian |  | Id bang hội |
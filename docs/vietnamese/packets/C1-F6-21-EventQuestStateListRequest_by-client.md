# C1 F6 21 - EventQuestStateListRequest (client gửi)

## Được gửi khi nào

Ứng dụng khách trò chơi yêu cầu danh sách nhiệm vụ sự kiện, thường là sau khi vào trò chơi.

## Hành động phía server

Máy chủ có thể trả lời bằng một phản hồi dường như phụ thuộc vào việc nhân vật có phải là thành viên của Gen hay không. Nếu nó không thuộc gen, nó sẽ gửi phản hồi (F603).

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 4 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xF6 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x21 | Tiêu đề gói - mã định danh loại gói phụ |
# C3 C6 - AddLetter (server gửi)

## Được gửi khi nào

Sau khi nhận được thư hoặc sau khi người chơi vào trò chơi với một nhân vật.

## Hành động phía client

Bức thư xuất hiện trong danh sách chữ cái.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 79 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xC6 | Tiêu đề gói - mã định danh loại gói |
| 4 | 2 | ShortLittleEndian |  | Thư mục lục |
| 6 | 10 | String |  | Tên người gửi |
| 16 | 30 | String |  | Dấu thời gian |
| 46 | 32 | String |  | Chủ thể |
| 78 | 1 | LetterState |  | Tình trạng |

### Enum LetterState
Mô tả trạng thái của một lá thư.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Read | Bức thư đã được đọc trước đó. |
| 1 | Unread | Bức thư vẫn chưa được đọc. |
| 2 | New | Bức thư mới (= vừa được người gửi gửi) và chưa được đọc. Nó sẽ thông báo cho người dùng về bức thư đã nhận được. |
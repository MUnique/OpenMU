# C3 1C 0F - MapChanged (server gửi)

## Được gửi khi nào

Bản đồ đã được thay đổi ở phía máy chủ.

## Hành động phía client

Ứng dụng khách trò chơi thay đổi bản đồ và tọa độ đã chỉ định.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 15 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x1C | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x0F | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 1 | Boolean | true | IsMapChange; Nếu sai, nó sẽ hiển thị hoạt ảnh dịch chuyển tức thời (bong bóng màu trắng) và máy khách không xóa tất cả các đối tượng trong phạm vi của nó. |
| 5 | 2 | ShortBigEndian |  | Bản đồSố |
| 7 | 1 | Byte |  | Vị tríX |
| 8 | 1 | Byte |  | Vị tríY |
| 9 | 1 | Byte |  | Xoay |
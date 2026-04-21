# C3 30 - TalkToNpcRequest (client gửi)

## Được gửi khi nào

Một người chơi muốn nói chuyện với NPC.

## Hành động phía server

Dựa trên loại NPC, máy chủ sẽ gửi phản hồi lại cho ứng dụng khách trò chơi. Ví dụ: nếu đó là NPC thương gia, nó sẽ gửi lại rằng hộp thoại thương gia sẽ được mở và những mặt hàng nào được NPC này cung cấp.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 5 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x30 | Tiêu đề gói - mã định danh loại gói |
| 3 | 2 | ShortBigEndian |  | Id Npc |
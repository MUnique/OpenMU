# C1 F6 31 - NpcBuffRequest (client gửi)

## Được gửi khi nào

Ứng dụng khách trò chơi yêu cầu nhận được buff từ npc nhiệm vụ hiện đang tương tác. Theo những gì chúng tôi biết, chỉ có NPC Elf Soldier mới cung cấp khả năng hỗ trợ như vậy cho đến khi nhân vật đạt đến cấp độ nhất định (150 hoặc 220).

## Hành động phía server

Máy chủ sẽ kiểm tra xem hộp thoại Quest NPC (ví dụ: Elf Soldier) chính xác có được mở hay không và người chơi chưa đạt đến giới hạn cấp độ hay không. Nếu đúng cả hai trường hợp, nó sẽ thêm một buff xác định (MagicEffect) cho người chơi; Nếu không, một tin nhắn sẽ được gửi đến người chơi.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 4 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xF6 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x31 | Tiêu đề gói - mã định danh loại gói phụ |
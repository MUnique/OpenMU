# C1 54 - GuildMasterAnswer (client gửi)

## Được gửi khi nào

Người chơi mở hộp thoại của NPC chủ bang hội và quyết định bước tiếp theo.

## Hành động phía server

Nó sẽ hủy việc tạo bang hội hoặc tiếp tục với hộp thoại tạo bang hội nơi người chơi có thể nhập tên và biểu tượng bang hội.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 4 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x54 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Boolean |  | ShowCreationDialog; Giá trị liệu hộp thoại tạo bang hội có được hiển thị hay không. Nếu không, việc tạo bang hội sẽ bị hủy và hộp thoại sẽ bị đóng. |
# C1 F7 01 - EnterEmpireGuardianEvent (client gửi)

## Được gửi khi nào

Người chơi muốn tham gia sự kiện người giám hộ đế chế nhờ hộp thoại của npc.

## Hành động phía server

Việc kiểm tra xem người chơi có thể tham gia sự kiện hay không và chuyển nó đến sự kiện nếu có thể.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 5 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xF7 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x01 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 1 | Byte | 01 | Khe vật phẩm; Khe vật phẩm của vé sự kiện. Không được sử dụng bởi máy chủ. |
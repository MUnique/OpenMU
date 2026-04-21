# C1 A2 - LegacyQuestStateSetRequest (client gửi)

## Được gửi khi nào

Người chơi muốn thay đổi trạng thái của một nhiệm vụ, ví dụ: để bắt đầu hoặc kết thúc một nhiệm vụ.

## Hành động phía server

Tùy thuộc vào trạng thái mới được yêu cầu, phản hồi sẽ được gửi lại.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 5 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xA2 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte |  | Số nhiệm vụ |
| 4 | 1 | LegacyQuestState |  | Bang mới |

### Enum LegacyQuestState
Trạng thái nhiệm vụ của hệ thống nhiệm vụ kế thừa.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Undefined | Trạng thái không được xác định. Giá trị này được sử dụng khi không có nhiệm vụ nào được xác định cho một chỉ mục cụ thể |
| 1 | Active | Nhiệm vụ đang hoạt động và đang được tiến hành. |
| 2 | Complete | Nhiệm vụ đã hoàn thành. |
| 3 | Inactive | Nhiệm vụ không hoạt động, điều đó có nghĩa là nó chưa hoạt động và chưa hoàn thành. |
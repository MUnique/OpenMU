# C1 F6 0F - QuestCancelled (server gửi)

## Được gửi khi nào

Máy chủ xác nhận yêu cầu hủy nhiệm vụ.

## Hành động phía client

Khách hàng đặt lại trạng thái của nhiệm vụ và có thể yêu cầu lại danh sách nhiệm vụ mới có sẵn. Danh sách này sau đó có thể sẽ chứa lại nhiệm vụ đã bị hủy.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 8 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xF6 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x0F | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 2 | ShortLittleEndian |  | Số nhiệm vụ; Số nhiệm vụ hiện tại. Trong thông báo này, nó luôn là 0 vì nhóm có liên quan đến khách hàng. |
| 6 | 2 | ShortLittleEndian |  | Nhóm nhiệm vụ |
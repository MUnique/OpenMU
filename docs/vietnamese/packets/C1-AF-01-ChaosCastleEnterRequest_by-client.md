# C1 AF 01 - ChaosCastleEnterRequest (client gửi)

## Được gửi khi nào

Người chơi yêu cầu vào lâu đài hỗn loạn bằng cách sử dụng vật phẩm 'Áo giáp của lính canh'.

## Hành động phía server

Máy chủ kiểm tra xem người chơi có thể tham gia sự kiện hay không và gửi phản hồi (Mã 0xAF) lại cho máy khách. Nếu thành công, nhân vật sẽ được chuyển đến bản đồ sự kiện.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 6 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xAF | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x01 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 1 | Byte |  | Cấp độ lâu đài; Cấp độ của lâu đài hỗn loạn. Xuất hiện luôn là 0. |
| 5 | 1 | Byte |  | TicketItemInventoryIndex; Chỉ mục của mục vé trong kho. |
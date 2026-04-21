# C1 9A - BloodCastleEnterRequest (client gửi)

## Được gửi khi nào

Người chơi yêu cầu vào lâu đài máu thông qua NPC Tổng lãnh thiên thần.

## Hành động phía server

Máy chủ kiểm tra xem người chơi có thể tham gia sự kiện hay không và gửi phản hồi (Mã 0x9A) lại cho máy khách. Nếu thành công, nhân vật sẽ được chuyển đến bản đồ sự kiện.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 5 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x9A | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte |  | Cấp độ lâu đài; Cấp độ của hình vuông chiến đấu. |
| 4 | 1 | Byte |  | TicketItemInventoryIndex; Chỉ mục của mục vé trong kho. Xin lưu ý rằng giá trị này cao hơn 12 so với mức cần thiết - điều này vô nghĩa, nhưng thực chất nó là như vậy ... |
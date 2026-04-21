# C1 F6 0A - QuestSelectRequest (client gửi)

## Được gửi khi nào

Khách hàng mở hộp thoại NPC nhiệm vụ và chọn một nhiệm vụ có sẵn.

## Hành động phía server

Nếu nhiệm vụ đã được kích hoạt, nó sẽ phản hồi bằng QuestProgress. Nếu nhiệm vụ không hoạt động, máy chủ sẽ quyết định xem nhân vật có thể bắt đầu nhiệm vụ hay không và phản hồi bằng QuestStepInfo với Số khởi đầu. Một nhân vật có thể thực hiện tối đa 3 nhiệm vụ cùng một lúc.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 9 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xF6 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x0A | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 2 | ShortLittleEndian |  | Số nhiệm vụ |
| 6 | 2 | ShortLittleEndian |  | Nhóm nhiệm vụ |
| 8 | 1 | Byte |  | SelectedTextIndex; Chỉ mục dựa trên 1 của chỉ mục đã chọn trong hộp thoại. Nó là 0 khi không có văn bản nào được chọn. Vẫn chưa rõ khi nào chúng ta cần điều đó. |
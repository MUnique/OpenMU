# C1 F3 00 - RequestCharacterList (client gửi)

## Được gửi khi nào

Sau khi đăng nhập thành công hoặc sau khi người chơi quyết định rời khỏi thế giới game để quay lại màn hình chọn nhân vật.

## Hành động phía server

Máy chủ gửi danh sách ký tự với tất cả các ký tự có sẵn.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 5 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xF3 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x00 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 1 | Byte |  | Ngôn ngữ |
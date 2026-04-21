# C1 F3 15 - FocusCharacter (client gửi)

## Được gửi khi nào

Người chơi tập trung (nhấp vào nó) một nhân vật mà anh ta dự định bước vào thế giới trò chơi trên màn hình chọn nhân vật.

## Hành động phía server

Máy chủ kiểm tra xem ký tự này có tồn tại hay không và gửi phản hồi lại. Nếu thành công, ứng dụng khách trò chơi sẽ làm nổi bật nhân vật được tập trung.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 14 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xF3 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x15 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 10 | String |  | Name |
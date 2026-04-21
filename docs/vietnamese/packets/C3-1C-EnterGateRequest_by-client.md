# C3 1C - EnterGateRequest (client gửi)

## Được gửi khi nào

Thông thường: Khi người chơi đi vào một khu vực trên bản đồ trò chơi được cấu hình làm cổng tại tệp dữ liệu khách hàng. Trong trường hợp đặc biệt của các pháp sư, gói này cũng được sử dụng cho kỹ năng dịch chuyển tức thời. Trong trường hợp này, GateNumber là 0 và tọa độ đích được chỉ định.

## Hành động phía server

Nếu người chơi được phép vào "cổng", nó sẽ được chuyển đến khu vực cổng thoát tương ứng.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 8 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x1C | Tiêu đề gói - mã định danh loại gói |
| 4 | 2 | ShortLittleEndian |  | Số cổng |
| 6 | 1 | Byte |  | Dịch chuyển mục tiêuX |
| 7 | 1 | Byte |  | Dịch chuyển mục tiêuY |
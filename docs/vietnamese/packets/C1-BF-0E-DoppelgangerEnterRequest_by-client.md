# C1 BF 0E - DoppelgangerEnterRequest (client gửi)

## Được gửi khi nào

Người chơi muốn tham gia sự kiện doppelganger.

## Hành động phía server

Máy chủ kiểm tra vé sự kiện và di chuyển người chơi đến bản đồ sự kiện.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 5 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xBF | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x0E | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 1 | Byte |  | VéMụcKhe cắm |
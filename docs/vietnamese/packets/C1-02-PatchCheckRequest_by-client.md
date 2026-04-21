# C1 02 - PatchCheckRequest (client gửi)

## Được gửi khi nào

Gói này được máy khách (launcher) gửi để kiểm tra xem phiên bản vá có đủ cao để có thể kết nối với máy chủ hay không.

## Hành động phía server

Máy chủ kết nối sẽ kiểm tra phiên bản và gửi thông báo 'PatchVersionK' hoặc 'ClientNeedsPatch'.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 6 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x02 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte |  | Phiên bản chính |
| 4 | 1 | Byte |  | Phiên bản nhỏ |
| 5 | 1 | Byte |  | Phiên bản vá |
# C1 83 00 - UnlockVault (client gửi)

## Được gửi khi nào

Người chơi muốn mở khóa kho tiền được bảo vệ bằng một chiếc ghim.

## Hành động phía server

Trạng thái khóa vault trên máy chủ được cập nhật. VaultProtectionInformation được gửi dưới dạng phản hồi.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 7 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x83 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x00 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 2 | ShortLittleEndian |  | Ghim |
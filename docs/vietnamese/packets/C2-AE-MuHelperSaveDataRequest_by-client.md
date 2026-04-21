# C2 AE - MuHelperSaveDataRequest (client gửi)

## Được gửi khi nào

Khách hàng muốn lưu dữ liệu MU Helper hiện tại.

## Hành động phía server

Máy chủ sẽ lưu dữ liệu MU Helper được cung cấp.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC2 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 2 | Short | 261 | Tiêu đề gói - chiều dài của gói |
| 3 | 1 | Byte | 0xAE | Tiêu đề gói - mã định danh loại gói |
| 4 | 257 | Binary |  | Dữ liệu trợ giúp |
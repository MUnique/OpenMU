# C2 AE - MuHelperConfigurationData (server gửi)

## Được gửi khi nào

Máy chủ đã lưu dữ liệu MU Helper của người dùng.

## Hành động phía client

Người dùng muốn lưu dữ liệu MU Helper.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC2 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 2 | Short | 261 | Tiêu đề gói - chiều dài của gói |
| 3 | 1 | Byte | 0xAE | Tiêu đề gói - mã định danh loại gói |
| 4 | 257 | Binary |  | Dữ liệu trợ giúp |
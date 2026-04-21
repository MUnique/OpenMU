# C3 99 - ServerImmigrationRequest (client gửi)

## Được gửi khi nào

Không biết?

## Hành động phía server

Không biết?

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte |  | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x99 | Tiêu đề gói - mã định danh loại gói |
| 3 |  | String |  | Mã bảo mật |
# C1 83 01 - SetVaultPin (client gửi)

## Được gửi khi nào

Người chơi muốn đặt mã pin mới cho kho tiền khi nó ở trạng thái mở khóa.

## Hành động phía server

Chốt vault đã được thiết lập. VaultProtectionInformation được gửi dưới dạng phản hồi.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 27 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x83 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x01 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 2 | ShortLittleEndian |  | Ghim |
| 6 | 20 | String |  | Mật khẩu; Mật khẩu của tài khoản được yêu cầu để đặt mã pin vault mới. |
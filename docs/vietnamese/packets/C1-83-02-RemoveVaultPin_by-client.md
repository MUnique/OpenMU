# C1 83 02 - RemoveVaultPin (client gửi)

## Được gửi khi nào

Người chơi muốn tháo chốt cho kho tiền khi nó ở trạng thái mở khóa.

## Hành động phía server

Ghim vault được gỡ bỏ. VaultProtectionInformation được gửi dưới dạng phản hồi.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 27 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x83 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x02 | Tiêu đề gói - mã định danh loại gói phụ |
| 6 | 20 | String |  | Mật khẩu; Mật khẩu của tài khoản được yêu cầu để xóa mã vault. |
# C1 83 - VaultProtectionInformation (server gửi)

## Được gửi khi nào

Sau khi người chơi yêu cầu mở kho tiền.

## Hành động phía client

Ứng dụng khách trò chơi cập nhật giao diện người dùng để hiển thị trạng thái bảo vệ vault hiện tại.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 4 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x83 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | VaultProtectionState |  | Bảo vệNhà nước |

### Enum VaultProtectionState
Xác định trạng thái bảo vệ vault.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Unprotected | Kho tiền không được bảo vệ. |
| 1 | Locked | Kho tiền được bảo vệ và khóa. Để di chuyển vật phẩm hoặc tiền, người chơi cần mở khóa. |
| 10 | UnlockFailedByWrongPin | Kho tiền được bảo vệ và khóa. Mở khóa do người dùng yêu cầu không thành công do mã pin sai. |
| 11 | SetPinFailedBecauseLock | Kho tiền được bảo vệ và khóa và cài đặt mã pin do người chơi yêu cầu không thành công do khóa. |
| 12 | Unlocked | Kho tiền được bảo vệ nhưng đã bị người chơi mở khóa. |
| 13 | RemovePinFailedByWrongPassword | Kho tiền được bảo vệ và việc xóa mã pin do người chơi yêu cầu không thành công do sử dụng sai mật khẩu. |
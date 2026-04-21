# C1 F1 01 - LoginResponse (server gửi)

## Được gửi khi nào

Sau khi server xử lý xong login request.

## Hành động phía client

Hiển thị kết quả đăng nhập. Nếu thành công, client tiếp tục gửi request lấy
danh sách nhân vật.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 5 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0xF1 | Packet header - packet type identifier |
| 3 | 1 | Byte | 0x01 | Packet header - sub packet type identifier |
| 4 | 1 | LoginResult |  | Success |

### Enum LoginResult

Kết quả của login request.

| Value | Name | Description |
|-------|------|-------------|
| 0 | InvalidPassword | Sai mật khẩu. |
| 1 | Okay | Đăng nhập thành công. |
| 2 | AccountInvalid | Tài khoản không hợp lệ. |
| 3 | AccountAlreadyConnected | Tài khoản đã đăng nhập ở phiên khác. |
| 4 | ServerIsFull | Server đầy. |
| 5 | AccountBlocked | Tài khoản bị khóa. |
| 6 | WrongVersion | Client sai phiên bản. |
| 7 | ConnectionError | Lỗi nội bộ trong quá trình kết nối. |
| 8 | ConnectionClosed3Fails | Đóng kết nối do sai thông tin đăng nhập 3 lần. |
| 9 | NoChargeInfo | Không có thông tin thanh toán. |
| 10 | SubscriptionTermOver | Hết thời hạn thuê bao. |
| 11 | SubscriptionTimeOver | Hết thời lượng thuê bao. |
| 14 | TemporaryBlocked | Tài khoản bị khóa tạm thời. |
| 17 | OnlyPlayersOver15Yrs | Chỉ người chơi trên 15 tuổi được phép kết nối. |
| 210 | BadCountry | Client kết nối từ quốc gia bị chặn. |

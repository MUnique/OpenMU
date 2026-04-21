# C1 92 - UpdateMiniGameState (server gửi)

## Được gửi khi nào

Trạng thái của một sự kiện trò chơi nhỏ sắp thay đổi sau 30 giây nữa.

## Hành động phía client

Phía khách hàng hiển thị một thông báo về trạng thái thay đổi.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 4 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x92 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | MiniGameTypeState |  | Tình trạng |

### Enum MiniGameTypeState
Tình trạng của các trò chơi mini.

| Value | Name | Description |
|-------|------|-------------|
| 0 | DevilSquareClosed | Được gửi khi quảng trường ma quỷ hiện đang đóng và sẽ được mở. "Bạn sẽ vào Quảng trường Quỷ (x giây kể từ bây giờ)". |
| 1 | DevilSquareOpened | Được gửi khi trò chơi hình vuông ma quỷ hiện đang mở để vào và đóng để vào. "Cổng Quảng trường Quỷ sẽ đóng lại sau x giây". |
| 2 | DevilSquareRunning | Được gửi khi trò chơi hình vuông ma quỷ hiện đang diễn ra và sắp kết thúc. "Cổng Quảng trường Quỷ sắp đóng lại (còn x giây)". |
| 3 | BloodCastleClosed | Trò chơi lâu đài máu đã đóng cửa. "Đóng lâu đài máu (trong x giây)". |
| 4 | BloodCastleOpened | Trò chơi lâu đài máu được mở ra để vào. "Xâm nhập lâu đài máu (tính bằng x giây)". |
| 5 | BloodCastleEnding | Trò chơi lâu đài máu sắp kết thúc. "Lâu Đài Máu kết thúc (trong x giây)". |
| 6 | BloodCastleFinished | Trò chơi lâu đài máu đã kết thúc. "Sự kiện Lâu đài Máu sẽ tắt (trong x giây)". |
| 7 | BloodCastleCongratulations | Trò chơi lâu đài máu đã kết thúc thành công. "Chúc mừng". |
| 10 | ChaosCastleClosed | Trò chơi lâu đài hỗn loạn đã đóng cửa. "Đóng cửa lâu đài hỗn loạn (trong x giây)". |
| 11 | ChaosCastleOpened | Trò chơi lâu đài hỗn loạn được mở ra để vào. "Xâm nhập lâu đài hỗn loạn (tính bằng x giây)". |
| 12 | ChaosCastleEnding | Trò chơi lâu đài hỗn loạn sắp kết thúc. "Sự kiện Lâu đài hỗn loạn kết thúc (trong x giây)". |
| 13 | ChaosCastleFinished | Trò chơi lâu đài hỗn loạn đã kết thúc. Sự kiện Lâu đài Hỗn loạn sẽ tắt (trong x giây)". |
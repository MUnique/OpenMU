# C3 F1 02 - LogOut (client gửi)

## Được gửi khi nào

Khi khách hàng muốn rời khỏi trò chơi theo nhiều cách khác nhau.

## Hành động phía server

Tùy thuộc vào LogOutType, máy chủ trò chơi thực hiện một số kiểm tra và gửi phản hồi lại cho máy khách. Nếu yêu cầu thành công, ứng dụng khách trò chơi sẽ đóng trò chơi, quay lại máy chủ hoặc chọn nhân vật.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 5 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xF1 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x02 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 1 | LogOutType |  | Type |

### Enum LogOutType
Mô tả cách người chơi muốn rời khỏi trò chơi hiện tại.

| Value | Name | Description |
|-------|------|-------------|
| 0 | CloseGame | Người chơi muốn đóng trò chơi. |
| 1 | BackToCharacterSelection | Người chơi muốn quay lại màn hình chọn nhân vật. |
| 2 | BackToServerSelection | Người chơi muốn quay lại màn hình chọn máy chủ. |
# C1 AA 01 - DuelStartResult (server gửi)

## Được gửi khi nào

Sau khi client gửi `DuelStartRequest`, và yêu cầu duel bị thất bại hoặc người
chơi được mời đã phản hồi.

## Hành động phía client

Client hiển thị trạng thái duel bắt đầu hoặc bị hủy.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 17 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0xAA | Packet header - packet type identifier |
| 3 | 1 | Byte | 0x01 | Packet header - sub packet type identifier |
| 4 | 1 | DuelStartResultType |  | Result |
| 5 | 2 | ShortBigEndian |  | OpponentId |
| 7 | 10 | String |  | OpponentName |

### Enum DuelStartResultType

Mô tả kết quả bắt đầu duel.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Success | Duel bắt đầu thành công. |
| 12 | FailedByTooLowLevel | Không thể bắt đầu duel vì một trong hai người chơi chưa đạt level tối thiểu (thường là 30). |
| 14 | FailedByError | Không thể bắt đầu duel do lỗi không mong muốn. |
| 15 | Refused | Không thể bắt đầu duel vì đối thủ từ chối. |
| 16 | FailedByNoFreeRoom | Không thể bắt đầu duel vì không còn phòng duel trống. |
| 28 | FailedBy_ | Không thể bắt đầu duel vì ... |
| 30 | FailedByNotEnoughMoney | Không thể bắt đầu duel vì một trong hai người chơi không đủ Zen (thường là 30000). |

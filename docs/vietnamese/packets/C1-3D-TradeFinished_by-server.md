# C1 3D - TradeFinished (server gửi)

## Được gửi khi nào

Khi một phiên giao dịch đã kết thúc.

## Hành động phía client

Client đóng cửa sổ trade. Tùy kết quả sẽ hiển thị message tương ứng.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 4 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0x3D | Packet header - packet type identifier |
| 3 | 1 | TradeResult |  | Result |

### Enum TradeResult

Định nghĩa kết quả của giao dịch.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Cancelled | Giao dịch bị hủy. |
| 1 | Success | Giao dịch thành công. |
| 2 | FailedByFullInventory | Thất bại do inventory đầy. |
| 3 | TimedOut | Thất bại do quá thời gian chờ. |
| 4 | FailedByItemsNotAllowedToTrade | Thất bại do có item không được phép giao dịch. |

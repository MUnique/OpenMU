# C2 F4 06 - ServerListResponse (server gửi)

## Được gửi khi nào

Packet này được server gửi sau khi client yêu cầu danh sách server hiện tại.

## Hành động phía client

Client hiển thị danh sách server khả dụng cùng thông tin tải.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC2 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 2 | Short |  | Packet header - độ dài packet |
| 3 | 1 | Byte | 0xF4 | Packet header - packet type identifier |
| 4 | 1 | Byte | 0x06 | Packet header - sub packet type identifier |
| 5 | 2 | ShortBigEndian |  | ServerCount |
| 7 | ServerLoadInfo.Length *  | Array of ServerLoadInfo |  | Servers |

### Cấu trúc ServerLoadInfo

Chứa id và mức tải của một server.

Độ dài: 4 bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 2 | ShortLittleEndian |  | ServerId |
| 2 | 1 | Byte |  | LoadPercentage |

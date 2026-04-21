# C1 51 - GuildJoinResponse (client gửi)

## Được gửi khi nào

Chủ bang hội đã trả lời yêu cầu đã gửi trước đó.

## Hành động phía server

Nếu yêu cầu được chủ bang hội chấp nhận, người chơi yêu cầu trước đó sẽ được thêm vào bang hội.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 6 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x51 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Boolean |  | Đã chấp nhận |
| 4 | 2 | ShortBigEndian |  | Id người yêu cầu |
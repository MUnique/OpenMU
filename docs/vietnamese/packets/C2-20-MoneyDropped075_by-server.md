# C2 20 - MoneyDropped075 (server gửi)

## Được gửi khi nào

Tiền được rơi xuống đất.

## Hành động phía client

Client hiển thị tiền trên mặt đất.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC2 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 2 | Short | 14 | Packet header - độ dài packet |
| 3 | 1 | Byte | 0x20 | Packet header - packet type identifier |
| 4 | 1 | Byte | 1 | ItemCount |
| 5 | 2 | ShortBigEndian |  | Id |
| 5 << 7 | 1 bit | Boolean |  | IsFreshDrop; Nếu cờ được set, tiền được thêm vào map kèm animation và âm thanh. Ngược lại chỉ được thêm như thể đã nằm sẵn trên đất. |
| 7 | 1 | Byte |  | PositionX |
| 8 | 1 | Byte |  | PositionY |
| 9 | 4 bit | Byte | 15 | MoneyNumber |
| 9 | 4 bit | Byte | 14 | MoneyGroup |
| 10 | 4 | IntegerLittleEndian |  | Amount |

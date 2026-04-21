# C1 22 - PickupItemRequest075 (client gửi)

## Được gửi khi nào

Khi người chơi yêu cầu nhặt một item nằm trên mặt đất gần nhân vật.

## Hành động phía server

Nếu người chơi được phép nhặt item và là người đầu tiên gửi yêu cầu thành công,
server sẽ thử thêm item vào inventory và trả response kết quả.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 5 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0x22 | Packet header - packet type identifier |
| 3 | 2 | ShortBigEndian |  | ItemId |

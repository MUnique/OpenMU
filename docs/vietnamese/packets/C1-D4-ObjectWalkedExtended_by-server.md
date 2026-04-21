# C1 D4 - ObjectWalkedExtended (server gửi)

## Được gửi khi nào

Một đối tượng trong phạm vi quan sát (bao gồm cả người chơi của chính mình) đã đi đến vị trí khác.

## Hành động phía client

Đối tượng được chuyển động để đi tới vị trí mới.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte |  | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xD4 | Tiêu đề gói - mã định danh loại gói |
| 2 | 1 | Byte |  | Mã tiêu đề |
| 3 | 2 | ShortBigEndian |  | ID đối tượng |
| 5 | 1 | Byte |  | NguồnX |
| 6 | 1 | Byte |  | NguồnY |
| 7 | 1 | Byte |  | Mục tiêuX |
| 8 | 1 | Byte |  | Mục tiêuY |
| 9 | 4 bit | Byte |  | Xoay mục tiêu |
| 9 | 4 bit | Byte |  | Đếm bước |
| 10 |  | Binary |  | Dữ liệu bước |
# C1 F6 0B - QuestStepInfo (server gửi)

## Được gửi khi nào

Sau khi khách hàng trò chơi nhấp vào một nhiệm vụ trong danh sách nhiệm vụ, tiếp tục nhiệm vụ hoặc từ chối bắt đầu nhiệm vụ.

## Hành động phía client

Khách hàng hiển thị mô tả tương ứng về bước tìm kiếm hiện tại.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 11 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xF6 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x0B | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 2 | ShortLittleEndian |  | Số bước nhiệm vụ; Một con số chỉ định mô tả: A) khi chọn một nhiệm vụ trong danh sách nhiệm vụ, đó là "Số bắt đầu"; B) khi một nhiệm vụ được bắt đầu thì đó là số nhiệm vụ; C) khi số bắt đầu đã được gửi trước đó và người chơi từ chối bắt đầu nhiệm vụ, nó sẽ gửi "Số từ chối". |
| 6 | 2 | ShortLittleEndian |  | Nhóm nhiệm vụ |
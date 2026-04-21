# C2 F3 53 - MasterSkillList (server gửi)

## Được gửi khi nào

Thông thường sau khi vào game với nhân vật chính.

## Hành động phía client

Dữ liệu có sẵn trong cây kỹ năng chính.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC2 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 2 | Short |  | Tiêu đề gói - chiều dài của gói |
| 3 | 1 | Byte | 0xF3 | Tiêu đề gói - mã định danh loại gói |
| 4 | 1 | Byte | 0x53 | Tiêu đề gói - mã định danh loại gói phụ |
| 8 | 4 | IntegerLittleEndian |  | Bậc ThầyKỹ NăngĐếm |
| 12 | MasterSkillEntry.Length * MasterSkillCount | Array of MasterSkillEntry |  | Kỹ năng |

### Cấu trúc MasterSkillEntry
Một mục trong danh sách kỹ năng bậc thầy.

Độ dài: 12 byte

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte |  | MasterSkillIndex; Chỉ số kỹ năng bậc thầy trên cây kỹ năng bậc thầy của khách hàng đối với lớp nhân vật nhất định. |
| 1 | 1 | Byte |  | Mức độ |
| 4 | 4 | Float |  | Giá trị hiển thị |
| 8 | 4 | Float |  | DisplayValueOfNextLevel |
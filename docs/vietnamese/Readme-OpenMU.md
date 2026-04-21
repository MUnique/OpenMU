# Tài liệu OpenMU

Thư mục này nên chứa toàn bộ tài liệu kỹ thuật của dự án OpenMU, bao gồm
[mô tả packet](../Packets/Readme.md), cơ chế game và kiến trúc phần mềm.

## Vì sao không dùng wiki?

Chúng tôi cho rằng phần lớn tài liệu (đặc biệt là mô tả packet) gắn chặt với
mã nguồn thực tế. Vì vậy, việc đặt tài liệu kỹ thuật ngay trong codebase là
hợp lý hơn, thay vì tách ra wiki.

Tuy nhiên, các tài liệu kiểu hướng dẫn sử dụng cho người dùng cuối vẫn có thể
đặt trên wiki.

## Kiến trúc Game Server

Lưu ý (theo [sven-n](https://github.com/sven-n)):
*Đây không phải kiến trúc hoàn hảo. Nhưng với mục tiêu dự án, nó linh hoạt và
không quá phức tạp.*

Để xem tổng quan, tham khảo
[architecture overview](../architecture%20overview.png).

Các interface để các subsystem giao tiếp với nhau nằm trong
[MUnique.OpenMU.Interfaces](https://github.com/MUnique/OpenMU/tree/master/src/Interfaces).

### Giao tiếp giữa game client và server

Giao tiếp mạng diễn ra qua [Connection class](https://github.com/MUnique/OpenMU/tree/master/src/Network/Connection.cs).
`MUnique.OpenMU.Network` chứa đầy đủ thành phần cần thiết để kết nối theo
protocol MU Online, bao gồm cả các struct message trong
`MUnique.OpenMU.Network.Packets`.

#### Client -> Server

Dữ liệu từ client được chuyển tới các packet handler trong namespace
`MUnique.OpenMU.GameServer.MessageHandler`. Mỗi handler là một
`IPacketHandlerPlugIn`.

Các handler parse packet rồi gọi player actions trong
`MUnique.OpenMU.GameLogic.PlayerActions`; tầng này không cần biết packet
format hay cơ chế truyền thông.

#### Server -> Client

Chiều ngược lại được thực hiện qua các view trong
`MUnique.OpenMU.GameServer.RemoteView`.
Các view dùng `Connection` để gửi packet đúng protocol, trong khi GameLogic
chỉ làm việc với [view interface plugins](https://github.com/MUnique/OpenMU/tree/master/src/GameLogic/Views/IViewPlugIn.cs).

#### Lợi ích của kiến trúc này

GameLogic không biết action được trigger thế nào và view được hiển thị ra sao.
Nhờ đó:

- Có thể thay networking bằng GUI/plugin khác.
- Có thể gọi player actions trực tiếp từ UI thay vì packet handler.
- Có thể mở rộng sang client/game khác nhưng vẫn tái sử dụng server components.

Mọi plugin có thể bật/tắt trong Admin Panel, thay thế bằng bản mở rộng.
Hệ thống cũng cho phép nhiều protocol/client version cùng chạy trên một game
world bằng cách triển khai nhiều view/handler và listener port tương ứng.

### Truy cập dữ liệu

Mẫu truy cập chính:

- Khi server start: nạp game configuration.
- Khi client login: nạp account.
- Trong game: lưu account theo các thời điểm và chu kỳ định sẵn.

#### Mục tiêu thiết kế

Hỗ trợ nhiều loại database (kể cả NoSQL) mà không sửa game logic.
Vì vậy game logic không nên chứa SQL cụ thể; thay vào đó dùng các abstraction
phù hợp để truy cập dữ liệu theo đơn vị lớn.

Ví dụ với document database:

- Account có thể là một document.
- Game configuration có thể là một document.

#### Abstractions

Game logic dùng abstraction kiểu [Repository](https://martinfowler.com/eaaCatalog/repository.html),
nằm trong namespace `MUnique.OpenMU.Persistence`.

Dự án dùng mô hình context-based:

- `GameConfiguration` được nạp qua `GameConfigurationRepository` trong một
  `IContext`.
- Mỗi player có `IPlayerContext` riêng để nạp `Account`.
- Khi lưu account, thực tế là lưu context; context tự xử lý các thay đổi cần
  ghi xuống database.

#### Triển khai hiện tại và DB hỗ trợ

Hiện tại persistence layer là
[MUnique.OpenMU.Persistence.EntityFramework](../src/Persistence/EntityFramework/Readme.md),
dùng Entity Framework Core và PostgreSQL.

#### Hướng tương lai

Do data model phức tạp, relational model thuần có thể không tối ưu dài hạn.
Hiện dự án dùng truy vấn lớn trả về JSON và cho hiệu năng khá tốt.

Về sau có thể:

- Kết hợp bảng quan hệ + JSON columns.
- Hoặc chuyển hẳn sang document database (ví dụ RavenDB).

### Thông tin thêm

- [Packets](../Packets/Readme.md): thông tin cấu trúc packet
- [Master Skill System](MasterSystem.md): mô tả hệ thống master skill
- [GameMap](GameMap.md): mô tả triển khai GameMap
- [Progress](Progress.md): tiến độ tính năng
- [Admin Panel](https://github.com/MUnique/OpenMU/tree/master/src/Web/AdminPanel)
- [Attribute System](https://github.com/MUnique/OpenMU/tree/master/src/AttributeSystem)
- [Network](https://github.com/MUnique/OpenMU/tree/master/src/Network)
- [Startup](https://github.com/MUnique/OpenMU/tree/master/src/Startup)

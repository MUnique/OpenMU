# Hướng dẫn setup và chạy OpenMU

Tài liệu này tổng hợp các bước cần thiết để chạy dự án OpenMU theo cách nhanh
nhất và cách dành cho phát triển/debug.

## 1) Chạy nhanh bằng Docker (khuyến nghị)

Đây là cách phù hợp nếu bạn muốn chạy thử server nhanh, ít cấu hình thủ công.

### Yêu cầu

- Đã cài Docker Desktop (hoặc Docker Engine + Docker Compose plugin)
- Máy local còn trống các cổng:
  - `80` (Admin Panel)
  - `55901-55906` (GameServer)
  - `44405-44406` (ConnectServer)
  - `55980` (ChatServer)

### Các bước

1. Clone source:

   ```bash
   git clone https://github.com/MUnique/OpenMU.git
   cd OpenMU/deploy/all-in-one
   ```

2. Chạy stack all-in-one:

   ```bash
   docker compose up -d --no-build
   ```

3. Mở Admin Panel:
   - URL: `http://localhost/`
   - User mặc định: `admin`
   - Password mặc định: `openmu`

4. Kiểm tra service đã chạy:

   ```bash
   docker compose ps
   ```

5. Dừng hệ thống khi không dùng:

   ```bash
   docker compose down
   ```

## 2) Chạy local để phát triển/debug

Dùng cách này khi bạn cần sửa code, debug, hoặc kiểm thử thay đổi.

### Yêu cầu

- .NET SDK 10
- PostgreSQL
- Node.js 16+
- IDE hỗ trợ C# (Visual Studio hoặc JetBrains Rider)
- Source code OpenMU

### Các bước

1. Clone source và vào thư mục:

   ```bash
   git clone https://github.com/MUnique/OpenMU.git
   cd OpenMU
   ```

2. Restore package:

   ```bash
   dotnet restore src/MUnique.OpenMU.sln
   ```

3. Cấu hình kết nối database tại:
   - `src/Persistence/EntityFramework/ConnectionSettings.xml`
   - Tối thiểu cần đúng user/password của chuỗi kết nối admin và app.

4. Build solution:

   ```bash
   dotnet build src/MUnique.OpenMU.sln
   ```

5. Chạy startup project:

   ```bash
   dotnet run --project src/Startup/Startup.csproj
   ```

6. Mở Admin Panel tại `http://localhost/`, vào phần setup nếu cần khởi tạo lại
   dữ liệu/game version.

### Tùy chọn hữu ích

- Reinitialize DB:

  ```bash
  dotnet run --project src/Startup/Startup.csproj -- -reinit
  ```

- Tự động start listener:

  ```bash
  dotnet run --project src/Startup/Startup.csproj -- -autostart
  ```

## 3) Kết nối game client

- Chuẩn bị game client MU phù hợp (hoặc launcher tương thích OpenMU).
- Nếu client và server chạy cùng máy local, dùng IP loopback dạng `127.x.x.x`
  (không dùng `127.0.0.1`).
- Dùng đúng cổng ConnectServer:
  - `44405`: client gốc
  - `44406`: open source client

## 4) Kiểm tra nhanh khi gặp lỗi

- Xem trạng thái container:

  ```bash
  docker compose ps
  ```

- Xem log container:

  ```bash
  docker compose logs -f
  ```

- Kiểm tra log chạy local:
  - Xem output console của `Startup`
  - Đảm bảo PostgreSQL đang chạy và user/password đúng
  - Đảm bảo các cổng không bị process khác chiếm

## Tài liệu liên quan

- [Quick Start](../../QuickStart.md)
- [Deployment overview](../../deploy/README.md)
- [All-in-one deployment](../../deploy/all-in-one/README.md)
- [Mac Server Dev & Debug Guide](../Mac-Server-Dev-Debug-Guide.md)

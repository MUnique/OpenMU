# Triển khai kiểm thử cấu trúc Packet

## Tổng quan

Triển khai này cung cấp bộ test được sinh tự động cho các cấu trúc packet định
nghĩa bằng XML. Mục tiêu là xác thực packet definition chính xác và phát hiện
sớm các lỗi như khai báo sai độ dài packet.

## Thành phần chính

### 1) `src/Network/Packets/GenerateTests.xslt`

XSLT dùng để sinh mã test C# từ XML packet definitions, bao gồm:

- kiểm tra packet/structure có độ dài cố định đúng với khai báo
- kiểm tra `GetRequiredSize` cho packet độ dài biến thiên
- kiểm tra biên trường dữ liệu để tránh tràn buffer
- sinh mã test C# hợp lệ với naming rõ ràng

### 2) `tests/MUnique.OpenMU.Network.Packets.Tests/`

Project test mới:

- tự động sinh file test trong quá trình build (khi `ci` không được set)
- tích hợp với hạ tầng test sẵn có (NUnit, StyleCop, ...)
- đã được thêm vào solution chính để chạy trong CI/CD

### 3) File test được sinh

- `ClientToServerPacketTests.cs`
- `ServerToClientPacketTests.cs`
- `ChatServerPacketTests.cs`
- `ConnectServerPacketTests.cs`

## Các loại kiểm tra

### Kiểm tra packet độ dài cố định

- So độ dài khai báo trong XML với độ dài thực tế từ struct sinh ra.
- Đảm bảo `fieldIndex + fieldSize` không vượt quá độ dài packet.

### Kiểm tra packet độ dài biến thiên

- Xác nhận `GetRequiredSize` tính đúng theo dữ liệu đầu vào.

### Kiểm tra biên trường dữ liệu

- Đảm bảo không có field vượt khỏi biên packet.

## Cách hoạt động

1. Build đọc XML packet definitions qua XSLT.
2. Sinh test methods đầy đủ cho từng packet/structure.
3. Chạy test trong quy trình test thông thường.
4. Tích hợp trực tiếp vào CI/CD.

## Lợi ích

- **Phát hiện tự động**: lỗi packet definition được bắt ngay từ build/test.
- **Độ phủ cao**: áp dụng cho nhiều nhóm packet/structure.
- **Ít bảo trì**: XML đổi thì test sinh lại tự động.
- **Phát hiện sớm**: giảm lỗi runtime do packet sai cấu trúc.

## Cách dùng

Test chạy tự động trong build/test pipeline.

Chạy tay:

```bash
dotnet test tests/MUnique.OpenMU.Network.Packets.Tests/
```

## Ví dụ lỗi có thể bắt

- Packet khai báo dài 10 byte nhưng field thực tế cần 12 byte.
- Field bắt đầu ở index 8, size 4 trong packet dài 10 byte.
- Tính toán `GetRequiredSize` sai.
- Field definition bị chồng lấn.

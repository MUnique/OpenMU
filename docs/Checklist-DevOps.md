# Checklist DevOps/Platform Engineer (Private MU)

## 1) Environment & Configuration

- [ ] Chuẩn hóa môi trường:
  - [ ] `dev`.
  - [ ] `staging`.
  - [ ] `prod`.
- [ ] Tách cấu hình bằng biến môi trường, không hardcode.
- [ ] Quản lý secret an toàn theo từng môi trường.

## 2) CI Pipeline

- [ ] Thiết lập pipeline build server tự động.
- [ ] Thiết lập pipeline build client theo target chính.
- [ ] Thêm smoke test cho luồng:
  - [ ] login.
  - [ ] vào map.
- [ ] Áp dụng quy tắc fail-fast khi build/test lỗi.

## 3) Runtime Reliability

- [ ] Thiết lập health-check cho các dịch vụ chính.
- [ ] Có cơ chế restart an toàn khi service lỗi.
- [ ] Theo dõi tài nguyên hệ thống:
  - [ ] CPU.
  - [ ] RAM.
  - [ ] I/O.
  - [ ] DB connections.

## 4) Observability

- [ ] Thu thập metrics:
  - [ ] online player count.
  - [ ] packet error rate.
  - [ ] average RTT.
  - [ ] disconnect reason.
- [ ] Thiết lập alert:
  - [ ] login fail tăng bất thường.
  - [ ] crash loop.
  - [ ] DB connection issue.
- [ ] Chuẩn hóa định dạng log để dễ truy vết.

## 5) Release & Rollback

- [ ] Tạo checklist phát hành nội bộ cho Closed Test.
- [ ] Tạo runbook rollback:
  - [ ] rollback server version.
  - [ ] rollback config.
  - [ ] rollback database migration (nếu có).
- [ ] Kiểm tra quy trình rollback định kỳ (game day nhẹ).

## 6) Security Baseline

- [ ] Bảo vệ secret trong CI/CD.
- [ ] Giới hạn quyền truy cập theo vai trò.
- [ ] Rà soát log để tránh lộ dữ liệu nhạy cảm.
- [ ] Có cơ chế audit thao tác quan trọng trong môi trường vận hành.

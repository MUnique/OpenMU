# Checklist Client Engineer (Private MU)

## 1) Technical Direction

- [ ] Chốt client track chính:
  - [ ] Unity (`afrokick/UniMU`) nếu ưu tiên Web về sau.
  - [ ] MonoGame (`bernatvadell/muonline`) nếu ưu tiên mobile sớm.
- [ ] Chốt client track dự phòng để giảm rủi ro.

## 2) Connection to OpenMU (Season 6)

- [ ] Kết nối client với OpenMU local theo baseline protocol.
- [ ] Hoàn thành luồng MVP:
  - [ ] Login.
  - [ ] Chọn/tạo nhân vật.
  - [ ] Vào map.
  - [ ] Di chuyển.
  - [ ] Đánh quái.
  - [ ] Nhặt đồ.
- [ ] Đối soát lỗi packet với phía server cho đến khi ổn định.

## 3) Cross-Platform Input

- [ ] Tách input layer rõ ràng:
  - [ ] Desktop: keyboard/mouse.
  - [ ] Mobile: touch + joystick.
  - [ ] Dùng chung gameplay layer.
- [ ] Nếu Unity:
  - [ ] Tích hợp Unity Input System.
  - [ ] Tích hợp joystick package miễn phí.

## 4) Mobile Optimization

- [ ] Giảm GC spike:
  - [ ] Ưu tiên object pooling cho entity/effect/projectile.
  - [ ] Hạn chế cấp phát mới trong vòng lặp update.
- [ ] Tối ưu rendering:
  - [ ] Giảm overdraw.
  - [ ] Cấu hình quality profile theo nhóm thiết bị.
- [ ] Tối ưu networking phía client:
  - [ ] Debounce/rate-limit input packet không cần thiết.
  - [ ] Kiểm soát tần suất update object.

## 5) Device Test Matrix

- [ ] Chốt thiết bị test tối thiểu:
  - [ ] Android yếu.
  - [ ] Android trung bình.
  - [ ] iOS (nếu có).
- [ ] KPI cần đạt:
  - [ ] FPS trung bình đạt mục tiêu.
  - [ ] Nhiệt độ/hao pin chấp nhận được.
  - [ ] Ping/latency ổn định trong gameplay cơ bản.

## 6) Milestone Done Criteria

- [ ] Milestone 2: vertical slice gameplay hoàn chỉnh.
- [ ] Milestone 3: chạy được PC + Android (iOS nếu có).
- [ ] Milestone 5: có kết quả POC WebGL (nếu đi Unity).

# Checklist Project Lead / Product Owner (Private MU)

## 1) Vision & Scope

- [ ] Chốt mục tiêu 8 tuần đầu:
  - [ ] Ưu tiên "playable MVP nhanh".
  - [ ] Hoặc ưu tiên "nền tảng Unity cho web về sau".
- [ ] Chốt phạm vi MVP:
  - [ ] login.
  - [ ] tạo/chọn nhân vật.
  - [ ] vào map.
  - [ ] di chuyển.
  - [ ] combat cơ bản.
  - [ ] nhặt đồ.

## 2) Decision Log (Bắt buộc)

- [ ] Chốt client track chính: Unity hay MonoGame.
- [ ] Chốt protocol phase đầu: Season 6.
- [ ] Chốt milestone và tiêu chí nghiệm thu từng milestone.
- [ ] Lưu decision log theo ngày, người quyết định, lý do.

## 3) Planning & Ownership

- [ ] Phân vai rõ:
  - [ ] Server owner.
  - [ ] Client owner.
  - [ ] DevOps owner.
- [ ] Mỗi task có:
  - [ ] owner.
  - [ ] deadline.
  - [ ] định nghĩa hoàn thành.
- [ ] Có danh sách blocker và SLA xử lý blocker.

## 4) Quality Gates

- [ ] Không chuyển milestone nếu chưa đạt tiêu chí chất lượng:
  - [ ] Không có blocker correctness.
  - [ ] Không có lỗi crash lặp lại chưa rõ nguyên nhân.
  - [ ] Có log/metrics đủ để debug production-like issues.
- [ ] Mọi thay đổi lớn phải có test plan.

## 5) Weekly Operating Cadence

- [ ] Weekly planning: chốt mục tiêu tuần + rủi ro.
- [ ] Mid-week sync: cập nhật tiến độ, xử lý blocker.
- [ ] End-week review:
  - [ ] demo kết quả.
  - [ ] so sánh kế hoạch/thực tế.
  - [ ] cập nhật scope tuần kế tiếp.

## 6) Release Readiness

- [ ] Trước Closed Test:
  - [ ] Có checklist release.
  - [ ] Có checklist rollback.
  - [ ] Có người on-call chịu trách nhiệm.
- [ ] Sau mỗi đợt test:
  - [ ] Tổng hợp bug theo mức độ.
  - [ ] Chốt thứ tự sửa theo tác động người chơi.
  - [ ] Cập nhật lại timeline phát hành.

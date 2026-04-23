# Checklist Client Engineer (Private MU)

## 0) Nguồn uy tín cần bám (PC + Mobile + Web)

### 0.1 Primary track (khuyến nghị): Unity 6 LTS

- [ ] Chốt Unity 6 LTS làm client track chính cho PC + Android + iOS.
- [ ] Bám tài liệu chính thức:
  - [ ] Unity iOS requirements: https://docs.unity3d.com/6000.3/Documentation/Manual/ios-requirements-and-compatibility.html
  - [ ] Unity Android requirements: https://docs.unity3d.com/6000.3/Documentation/Manual/android-requirements-and-compatibility.html
  - [ ] Unity Netcode for GameObjects: https://docs.unity3d.com/Packages/com.unity.netcode.gameobjects@2.7/manual/index.html
  - [ ] Unity transport layer docs: https://docs.unity3d.com/Packages/com.unity.netcode.gameobjects@2.7/manual/advanced-topics/transports.html

### 0.2 Open-source backup track: Godot 4.x

- [ ] Giữ Godot như phương án dự phòng (open-source, mobile export rõ ràng).
- [ ] Bám tài liệu chính thức:
  - [ ] Godot Android export: https://docs.godotengine.org/en/stable/tutorials/export/exporting_for_android.html
  - [ ] Godot iOS export: https://docs.godotengine.org/en/stable/tutorials/export/exporting_for_ios.html

### 0.3 Web nâng cấp giai đoạn sau

- [ ] Chọn Web client theo hướng WebGPU + Babylon.js (có fallback WebGL).
- [ ] Bám tài liệu chính thức:
  - [ ] web.dev - WebGPU major browser support: https://web.dev/blog/webgpu-supported-major-browsers
  - [ ] Babylon WebGPU support: https://doc.babylonjs.com/setup/support/webGPU

### 0.4 Baseline tương thích OpenMU protocol

- [ ] Dùng MuMain làm tham chiếu client/protocol cho Season 6:
  - [ ] https://github.com/sven-n/MuMain
- [ ] Bám OpenMU server/network packets:
  - [ ] https://github.com/MUnique/OpenMU

### 0.5 Repo mobile MU client để fork/mua

- [ ] Ưu tiên fork thử nghiệm:
  - [ ] `bernatvadell/muonline` (MonoGame, active, đa nền tảng): https://github.com/bernatvadell/muonline
  - [ ] `afrokick/UniMU` (Unity, cũ nhưng gần OpenMU): https://github.com/afrokick/UniMU
- [ ] Hướng web/mobile bổ sung:
  - [ ] `afrokick/muonlinejs` (Babylon.js + proxy): https://github.com/afrokick/muonlinejs
- [ ] Mua nếu cần tăng tốc:
  - [ ] Đánh giá Unity Asset Store MMORPG kits (license + customization cost + legal risk).
  - [ ] Chỉ dùng nguồn có license rõ ràng, tránh source leak/warez.
- [ ] Quyết định fork chính thức hôm nay:
  - [ ] **Fork chính:** `bernatvadell/muonline`
  - [ ] Chạy theo: `docs/Day0-Fork-Bootstrap-Client.md`

## 1) Technical Direction

- [ ] Chốt client track chính:
  - [ ] Unity 6 LTS + Netcode for GameObjects + Unity Transport (recommended).
  - [ ] Godot 4.x (backup nếu cần open-source stack toàn diện).
- [ ] Chốt client track dự phòng để giảm rủi ro.
- [ ] Không dùng nguồn thiếu maintainer/không có docs chính thức làm nền tảng chính.

### 1.1 Decision matrix (phải chốt trước khi code nhiều)

- [ ] So sánh Unity vs Godot theo tiêu chí bắt buộc:
  - [ ] Maturity và tốc độ tuyển dụng.
  - [ ] Tooling profiling trên mobile.
  - [ ] Tốc độ dựng prototype gameplay MU.
  - [ ] Khả năng tái sử dụng asset pipeline về sau.
  - [ ] Độ khó mở rộng web phase.
- [ ] Chốt scoring (1-5) và quyết định:
  - [ ] Unity >= 4 điểm tổng quan: chọn làm production track.
  - [ ] Godot >= 4 điểm tổng quan: dùng làm backup/POC track.
- [ ] Lưu quyết định vào ADR nội bộ:
  - [ ] `docs/adr/ADR-Client-Engine-Selection.md`

### 1.2 Kết luận tạm thời cho dự án này

- [ ] Chọn Unity làm đường chính (PC + Android + iOS).
- [ ] Giữ Godot để kiểm chứng rủi ro vendor lock-in.
- [ ] Web đi theo hướng tách client web riêng (Babylon/WebGPU), không ép từ mobile build.

## 1.3 Client architecture target (PC + mobile trước, web sau)

- [ ] Thiết kế theo module:
  - [ ] `Client.Core` (domain/gameplay rules phía client, không phụ thuộc engine).
  - [ ] `Client.Protocol` (serialization/packet map theo OpenMU).
  - [ ] `Client.Runtime.Unity` (scene/input/render/audio, platform adapters).
  - [ ] `Client.Tools` (packet capture, replay, diff tool).
- [ ] Thiết lập nguyên tắc:
  - [ ] Không để logic gameplay nằm trực tiếp trong MonoBehaviour.
  - [ ] Input adapter tách desktop/mobile, cùng API command.
  - [ ] Network adapter tách transport khỏi gameplay state.
- [ ] Chuẩn bị cho web phase:
  - [ ] Giữ `Client.Protocol` dùng chung cho web client.
  - [ ] Chuẩn hóa message contract bằng golden packet tests.
  - [ ] Tránh phụ thuộc asset/runtime chỉ có trên Unity trong phần Core/Protocol.

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
- [ ] Thiết lập packet replay regression:
  - [ ] Capture phiên login->move->attack mẫu.
  - [ ] Replay tự động sau mỗi thay đổi protocol adapter.
  - [ ] Fail build nếu packet decode/encode lệch baseline.

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
- [ ] Milestone 5: có kết quả POC Web (WebGPU first, WebGL fallback).

## 7) Phase plan đề xuất

- [ ] Phase A (2-4 tuần): Protocol + vertical slice PC.
- [ ] Phase B (2-4 tuần): Android tuning + mobile input hoàn chỉnh.
- [ ] Phase C (2-3 tuần): iOS stabilization + build pipeline.
- [ ] Phase D (3-5 tuần): Web POC dùng Babylon + shared protocol package.

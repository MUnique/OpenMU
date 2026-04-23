# ADR-Client-Engine-Selection

## Status

Accepted

## Date

2026-04-21

## Context

Project mục tiêu phát triển MU đa nền tảng:

- PC trước (để validate gameplay/protocol nhanh).
- Mobile (Android/iOS) ngay sau đó.
- Web ở phase sau (không làm blocker cho bản PC/mobile đầu tiên).

Server backend là OpenMU, packet/protocol có đặc thù Season 6 và cần độ ổn định cao
qua nhiều lần refactor phía client.

Nhóm cần chọn engine và hướng kiến trúc đủ ổn định để:

- Ship nhanh vertical slice.
- Mở rộng mobile với chi phí tối ưu.
- Không khóa đường lên web về sau.

## Decision

1. Chọn **Unity 6 LTS** làm production engine cho client PC + mobile.
2. Chọn **Unity Netcode for GameObjects + Unity Transport** làm stack networking
   mặc định phía client runtime.
3. Giữ **Godot 4.x** làm backup/POC track để giảm rủi ro phụ thuộc vendor.
4. Web phase đi theo hướng **client web riêng** (Babylon.js + WebGPU first, WebGL
   fallback), không ép reuse trực tiếp Unity runtime.
5. Chuẩn hóa kiến trúc shared:
   - `Client.Core`: gameplay/domain client logic (engine-agnostic).
   - `Client.Protocol`: packet encode/decode và mapping theo OpenMU.
   - `Client.Runtime.Unity`: scene/input/render/audio/platform integration.
   - `Client.Tools`: packet capture/replay/diff.

## Rationale

- Unity 6 LTS có tài liệu chính thức, hệ sinh thái lớn, tuyển dụng dễ, tooling
  profiling mobile tốt.
- Netcode + Transport chính chủ giúp giảm rủi ro maintenance so với stack community
  làm nền tảng chính.
- Tách `Core`/`Protocol` khỏi runtime giúp tránh khóa chặt vào Unity, hỗ trợ web
  phase sau này.
- Web có yêu cầu rendering/runtime khác đáng kể; đi client web riêng sẽ giảm độ
  phức tạp hơn so với cố “port nguyên runtime”.

## Consequences

### Positive

- Tốc độ ra MVP nhanh cho PC/mobile.
- Có đường kỹ thuật rõ ràng để lên web.
- Test protocol có thể chuẩn hóa bằng replay regression.

### Negative

- Cần duy trì năng lực ở 2 track (Unity chính + Godot backup mức tối thiểu).
- Web không “miễn phí”, cần effort riêng cho Babylon/WebGPU phase.

## Guardrails

- Không đưa gameplay business logic vào MonoBehaviour trực tiếp.
- Mọi thay đổi protocol phải đi kèm packet replay test.
- Không merge tính năng lớn khi chưa chạy qua matrix test thiết bị tối thiểu.

## References

- Unity iOS requirements:
  https://docs.unity3d.com/6000.3/Documentation/Manual/ios-requirements-and-compatibility.html
- Unity Android requirements:
  https://docs.unity3d.com/6000.3/Documentation/Manual/android-requirements-and-compatibility.html
- Unity Netcode for GameObjects:
  https://docs.unity3d.com/Packages/com.unity.netcode.gameobjects@2.7/manual/index.html
- Unity transport docs:
  https://docs.unity3d.com/Packages/com.unity.netcode.gameobjects@2.7/manual/advanced-topics/transports.html
- Godot Android export:
  https://docs.godotengine.org/en/stable/tutorials/export/exporting_for_android.html
- Godot iOS export:
  https://docs.godotengine.org/en/stable/tutorials/export/exporting_for_ios.html
- WebGPU browser support:
  https://web.dev/blog/webgpu-supported-major-browsers
- Babylon WebGPU support:
  https://doc.babylonjs.com/setup/support/webGPU
- OpenMU:
  https://github.com/MUnique/OpenMU
- MuMain:
  https://github.com/sven-n/MuMain

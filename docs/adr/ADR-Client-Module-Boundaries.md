# ADR-Client-Module-Boundaries

## Status

Accepted

## Date

2026-04-21

## Context

Client MU cần chạy đa nền tảng (PC + mobile, web phase sau). Nếu trộn gameplay,
protocol và engine code vào một chỗ thì sẽ:

- khó test tự động,
- khó tái sử dụng giữa Unity/mobile/web,
- tăng rủi ro regression khi đổi renderer/input/runtime.

## Decision

Chốt ranh giới module như sau:

1. `Client.Core`
   - Thuần C#, không phụ thuộc Unity/Godot/Babylon.
   - Chứa state client, use-cases gameplay, rules phía client.
2. `Client.Protocol`
   - Encode/decode packet, mapping model <-> bytes.
   - Không chứa render/input/UI logic.
3. `Client.Runtime.Unity`
   - Scene, prefab, rendering, animation, audio, platform bindings.
   - Chuyển input native thành command cho `Client.Core`.
4. `Client.Tools`
   - Capture/replay/diff packet, diagnostics, migration scripts.

## Rules

- `Runtime` chỉ gọi vào `Core` qua interface/use-case, không sửa state trực tiếp.
- `Protocol` không được tham chiếu runtime assemblies.
- Thêm packet mới bắt buộc có fixture và replay test tương ứng.
- Code web phase phải tái sử dụng `Client.Core` + `Client.Protocol`.

## Consequences

### Positive

- Giảm coupling, dễ mở rộng mobile/web.
- Dễ test unit và replay protocol.
- Refactor engine/runtime an toàn hơn.

### Negative

- Tốn effort thiết kế interface ban đầu.
- Cần review discipline để giữ boundary sạch.

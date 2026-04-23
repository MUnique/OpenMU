# Unity Project Structure Template (OpenMU Client)

## Mục tiêu

Template này chuẩn hóa cấu trúc thư mục và naming convention để bắt đầu dev
client MU cho PC + mobile, đồng thời giữ cửa cho web phase sau.

## Folder layout

```text
client-unity/
  Assets/
    _Project/
      Art/
        Characters/
        Environments/
        UI/
      Audio/
        Bgm/
        Sfx/
      Data/
        Config/
        Localization/
      Prefabs/
        Characters/
        Monsters/
        UI/
      Scenes/
        Bootstrap.unity
        Login.unity
        SelectCharacter.unity
        MainWorld.unity
      Scripts/
        Core/
          Domain/
          Application/
          Interfaces/
        Protocol/
          Packets/
          Serialization/
          Mapping/
        Runtime/
          Unity/
            Bootstrap/
            Input/
            View/
            Networking/
        Tools/
          Replay/
          Diagnostics/
      UI/
        UXML/
        USS/
      Addressables/
    Plugins/
    ThirdParty/
  Packages/
  ProjectSettings/
```

## Naming convention

- Namespace root: `OpenMU.Client`
- Core classes:
  - `PlayerState`, `MoveCommand`, `AttackUseCase`
- Protocol classes:
  - `LoginRequestPacket`, `MoveRequestPacketSerializer`
- Runtime classes:
  - `UnityInputAdapter`, `PlayerViewPresenter`, `NetworkLoopService`
- Suffix rules:
  - `*Service`, `*UseCase`, `*Adapter`, `*Presenter`, `*Serializer`, `*Packet`

## Assembly definition guideline

- `OpenMU.Client.Core.asmdef` -> không tham chiếu UnityEngine.
- `OpenMU.Client.Protocol.asmdef` -> không tham chiếu Runtime asmdef.
- `OpenMU.Client.Runtime.Unity.asmdef` -> được tham chiếu `Core` + `Protocol`.
- `OpenMU.Client.Tools.asmdef` -> phục vụ debug/replay/internal tooling.

## First scenes

1. `Bootstrap.unity`:
   - DI bootstrap, config load, network init.
2. `Login.unity`:
   - login flow + lỗi kết nối.
3. `SelectCharacter.unity`:
   - list/create/select.
4. `MainWorld.unity`:
   - map, movement, target, combat MVP.

## Done criteria for template adoption

- Tạo đủ thư mục + asmdef như trên.
- Chạy scene `Bootstrap -> Login` không lỗi.
- Có 1 packet end-to-end (`LoginRequest`) qua `Protocol` layer.

# C1 B2 01 - CastleSiegeRegistrationResult (by server)

## Is sent when

After a guild attempts to register or unregister for castle siege.

## Causes the following actions on the client side

The client shows a message indicating the result of the registration attempt.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   5   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x01  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | Result; The result code: 0=Success, 1=Unregistered, 2=NotInGuild, 3=NotTheGuildMaster, 4=NotInAlliance, 5=NotAllianceMaster, 6=RegistrationClosed, 7=AlreadyRegistered, 8=NotRegistered |
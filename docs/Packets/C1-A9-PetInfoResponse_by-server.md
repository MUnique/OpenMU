# C1 A9 - PetInfoResponse (by server)

## Is sent when

After the client sent a PetInfoRequest for a pet (dark raven, horse).

## Causes the following actions on the client side

The client shows the information about the pet.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   13   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xA9  | Packet header - packet type identifier |
| 3 | 1 | ClientToServer.PetType |  | Pet |
| 4 | 1 | ClientToServer.StorageType |  | Storage |
| 5 | 1 | Byte |  | ItemSlot |
| 6 | 1 | Byte |  | Level |
| 8 | 4 | IntegerLittleEndian |  | Experience |
| 12 | 1 | Byte |  | Health |
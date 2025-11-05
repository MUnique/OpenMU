# C1 B2 03 - CastleSiegeStatus (by server)

## Is sent when

When a player requests the current castle siege status.

## Causes the following actions on the client side

The client displays the current siege status including owner guild and state.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   14   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x03  | Packet header - sub packet type identifier |
| 4 | 8 | String |  | OwnerGuildName; The name of the guild that currently owns the castle (8 characters, space-padded). |
| 12 | 1 | Byte |  | State; The current state of the castle siege: 0=Inactive, 1=RegistrationOpen, 2=InProgress, 3=Ended |
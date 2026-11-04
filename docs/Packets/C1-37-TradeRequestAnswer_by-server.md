# C1 37 - TradeRequestAnswer (by server)

## Is sent when

The player which receives this message, sent a trade request to another player. This message is sent when the other player responded to this request.

## Causes the following actions on the client side

If the trade was accepted, a trade dialog is opened. Otherwise, a message is shown.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   20   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x37  | Packet header - packet type identifier |
| 3 | 1 | Boolean |  | Accepted |
| 4 | 10 | String |  | Name |
| 14 | 2 | ShortBigEndian |  | TradePartnerLevel |
| 16 | 4 | IntegerLittleEndian |  | GuildId |
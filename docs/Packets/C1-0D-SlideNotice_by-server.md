# C1 0D - SlideNotice (by server)

## Is sent when

A game master or the server sends a notice which should scroll across the top of the screen.

## Causes the following actions on the client side

The client scrolls the message through the band at the top of the screen, as often as the loop count says.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x0D  | Packet header - packet type identifier |
| 3 | 1 | Byte | 14 | Type; The band and queueing of the message. 10 to 12 use the help band, 13 to 15 the notice band. Within a band, the first value drops the message when it could not be shown within a minute, the second one queues it, and the third one puts it in front of the queue, ignoring the loop delay. |
| 4 | 1 | Byte | 1 | LoopCount; How often the message is shown. The client ignores messages with more than 30 loops. |
| 6 | 2 | ShortLittleEndian |  | LoopDelay; The delay between the loops, in seconds. |
| 8 | 4 | IntegerLittleEndian |  | Color; The color of the text, as 0xAABBGGRR. |
| 12 | 1 | Byte |  | Speed; The scroll speed, multiplied by 10. 0 means the default speed. |
| 13 |  | String |  | Message |
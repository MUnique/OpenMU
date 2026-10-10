# C1 F5 05 - DiscordLinkCode (by server)

## Is sent when

After the client requested a code to link the account to a Discord user.

## Causes the following actions on the client side

The client shows the code, so that the player can enter it in Discord.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   16   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF5  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x05  | Packet header - sub packet type identifier |
| 4 | 1 | DiscordLinkCodeResult |  | Result |
| 5 | 10 | String |  | LinkCode; The one-time code, e.g. 'ABCD-EFGH'. The player enters it in Discord with '/link'. |
| 15 | 1 | Byte |  | ValidMinutes; The number of minutes the code is valid. |

### DiscordLinkCodeResult Enum

The result of a request of a code to link the account to a Discord user.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Success | The code was created. |
| 1 | NotAvailable | Linking isn't available, e.g. because the server isn't connected to Discord. |
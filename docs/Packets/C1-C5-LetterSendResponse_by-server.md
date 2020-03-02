# C1 C5 - LetterSendResponse (by server)

## Is sent when

After the player requested to send a letter to another player.

## Causes the following actions on the client side

Depending on the result, the letter send dialog closes or an error message appears.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   8   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xC5  | Packet header - packet type identifier |
| 4 | 4 | IntegerLittleEndian |  | LetterId |
| 3 | 1 | LetterSendRequestResult |  | Result |

### LetterSendRequestResult Enum

Describes the result of a letter send request.

| Value | Name | Description |
|-------|------|-------------|
| 0 | TryAgain | The letter wasn't sent because there was an internal problem. The user should try again. |
| 1 | Success | The letter was sent. |
| 2 | MailboxFull | The mailbox of the recipient is full. |
| 3 | ReceiverNotExists | The receiver does not exist. |
| 4 | CantSendToYourself | A letter can't be sent to yourself. |
| 7 | NotEnoughMoney | The sender doesn't have enough money to send a letter. |
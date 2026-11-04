# C2 F5 01 - AvailableChatCommand (by server)

## Is sent when

After the client requested the list of available chat commands. One message is sent for each command which is available to the player.

## Causes the following actions on the client side

The client adds the command to its list of known commands, so that it can offer them to the player without requiring him to know or type them.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC2  | [Packet type](PacketTypes.md) |
| 1 | 2 |    Short   |      | Packet header - length of the packet |
| 3 | 1 |    Byte   | 0xF5  | Packet header - packet type identifier |
| 4 | 1 |    Byte   | 0x01  | Packet header - sub packet type identifier |
| 5 | 1 | Byte |  | Index; The index of this command within the list, starting at 0. |
| 6 | 1 | Byte |  | Count; The total number of commands which are available to the player, so that the client knows when it received all of them. |
| 7 | 1 | CharacterStatus |  | MinimumCharacterStatus; The character status which is required to execute the command. |
| 8 | 1 | Byte |  | ParameterCount |
| 9 | 32 | String |  | Command; The command including its slash, e.g. '/item'. |
| 41 | 48 | String |  | Name; The name of the command, in the language of the player. |
| 89 | 256 | String |  | Description; The description of the command, in the language of the player. |
| 345 | ChatCommandParameter.Length * ParameterCount | Array of ChatCommandParameter |  | Parameters; The parameters of the command, in the order in which they are expected when they are entered without their short names. |

### ChatCommandParameter Structure

Describes one parameter of a chat command, so that a user interface can offer an input for it.

Length: 102 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Boolean |  | IsRequired; Defines if the parameter has to be specified to execute the command. |
| 1 | 1 | ChatCommandParameterType |  | Type; The kind of value which is expected, so that a fitting input can be shown. |
| 2 | 32 | String |  | Name |
| 34 | 20 | String |  | ShortName; The short name which is used in the 'shortName=value' notation. It's empty when the parameter can only be passed by its position. |
| 54 | 48 | String |  | ValidValues; The accepted values, separated by a pipe. It's empty when the parameter isn't limited to a set of values. |

### ChatCommandParameterType Enum

The kind of value which a chat command parameter expects.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Text | The parameter expects a text. |
| 1 | Number | The parameter expects a number. |
| 2 | Boolean | The parameter expects a 0 or a 1. |

### CharacterStatus Enum

The status of a character.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Normal | The state of the character is normal. |
| 1 | Banned | The character is banned from the game. |
| 32 | GameMaster | The character is a game master. |
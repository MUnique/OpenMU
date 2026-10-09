# C2 F5 02 - AvailableChatCommandParameterHints (by server)

## Is sent when

Directly after an AvailableChatCommand message of a command which has parameters.

## Causes the following actions on the client side

The client remembers what the values of the parameters refer to and which values they accept, so that it can offer fitting inputs, e.g. a list of monsters instead of an empty number field. It's purely descriptive - a client which doesn't know this message can ignore it.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC2  | [Packet type](PacketTypes.md) |
| 1 | 2 |    Short   |      | Packet header - length of the packet |
| 3 | 1 |    Byte   | 0xF5  | Packet header - packet type identifier |
| 4 | 1 |    Byte   | 0x02  | Packet header - sub packet type identifier |
| 5 | 1 | Byte |  | Index; The index of the command within the list, which is the same as in the AvailableChatCommand message before. |
| 6 | 1 | Byte |  | ParameterCount |
| 7 | ChatCommandParameterHint.Length * ParameterCount | Array of ChatCommandParameterHint |  | Parameters; The hints for the parameters of the command, in the same order as in the AvailableChatCommand message. |

### ChatCommandParameterHint Structure

Describes what the value of a chat command parameter refers to and which values it accepts. It's a hint for the user interface, never a constraint.

Length: 20 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | ChatCommandValueReference |  | ValueReference; The kind of object which the value refers to. |
| 1 | 1 | Byte |  | GroupWithIndex; The index of the other parameter which identifies the referenced object together with this one, e.g. the group of an item. It's 255 when there is none. |
| 2 | 1 | Boolean |  | HasRange; Defines if the parameter is numeric and the minimum and maximum are known. |
| 4 | 8 | LongLittleEndian |  | Minimum; The smallest accepted value, as a signed number in two's complement. Only valid when HasRange is set. |
| 12 | 8 | LongLittleEndian |  | Maximum; The largest accepted value, as a signed number in two's complement. Only valid when HasRange is set. |

### ChatCommandValueReference Enum

The kind of object which the value of a chat command parameter refers to. New kinds are only appended, so that the values stay the same.

| Value | Name | Description |
|-------|------|-------------|
| 0 | None | The value doesn't refer to any known kind of object. |
| 1 | CharacterName | The value is the name of a character. |
| 2 | AccountName | The value is the login name of an account. |
| 3 | GuildName | The value is the name of a guild. |
| 4 | Map | The value is the number or the name of a map. |
| 5 | MapCoordinateX | The value is a x-coordinate on a map. |
| 6 | MapCoordinateY | The value is a y-coordinate on a map. |
| 7 | ItemGroup | The value is the group of an item definition. |
| 8 | ItemNumber | The value is the number of an item definition within its group. |
| 9 | MonsterNumber | The value is the number of a monster definition, which also identifies its model. |
| 10 | ObjectId | The value is the id of an object which is currently in the scope of the player. |
| 11 | SkillNumber | The value is the number of a skill. |
| 12 | LanguageIsoCode | The value is the ISO 639-1 code of a language, e.g. 'en'. |
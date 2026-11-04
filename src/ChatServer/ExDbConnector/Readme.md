# ChatServer ExDB Connector

This isn't directly a part of the OpenMU project. It's more like a side product
to make the ChatServer available to users of the 'classical' private MU Servers.
They have - or maybe had :) - the problem that they are bound to use the
original closed source ChatServer of Webzen, if they get it working at all.

So to offer an open source alternative to the original ChatServer of Webzen,
you can use this project to connect the OpenMU-ChatServer with your 'classic'
ExDB server.

## Configuration

To make this work correctly with an existing ExDB-Server, some might do some
minor adjustments in the configuration.

It's all configured in the ChatServer.cfg an should be self-explanatory.

### ChatServerListenerPort

It's the port to which the game clients should connect. Default is 55980,
but I'm not sure if it can be changed without modifying the client.

### ExDbHost and Port

The host and tcp port of the ExDB server. Usually it's on the same server, so
127.0.0.1 on port 55906.

### Xor32Key

This one is actually very important to get right. Otherwise, the game clients
will not be able to connect.
It's the same XOR32 key which is used for the 0xC1 packet encryption from game
client to game server.

You can't edit this key at the original ChatServer of Webzen, that's the reason
why it's pretty hard to get the ChatServer working on a private server.

## Communication between ExDB-Server and ChatServer

The ExDB server usually leaves the tcp port 55906 open, so that the ChatServer
(and maybe other kind of subservers?) can connect to it.

### Registration

When the ChatServer connects to the ExDB server, it sends a data packet to
register itself. It has the following struture:

|  Length  | Data type | Value | Description |
|----------|---------|-------------|---------|
| 1 | byte | 0xC1   | Packet header - type |
| 1 | byte | 0x3A   | Packet header - length of the packet |
| 1 | byte | 0x00   | Packet Type "server registration" |
| 1 | byte | 0x02   | Id for "ChatServer" |
| 2 | ushort | 0xDAAC   | ChatServer client port (‭default: 55980‬) |
| 11 | string | "ChatServer"   | ChatServer name |

Example: C1 3A 00 02 AC DA 43 68 61 74 53 65 72 76 65 72 00

From now, the ChatServer will receive chat room creation and invitation
requests from the ExDB Server, which were previously requested by the players.

### Chat Room Creation Request

When a client requests to create a new chat room, the following data packet is
sent from the ExDB Server to the ChatServer.

|  Length  | Data type | Value | Description |
|----------|---------|-------------|---------|
| 1 | byte | 0xC1   | Packet header - type |
| 1 | byte | 0x25   | Packet header - length of the packet |
| 1 | byte | 0xA0   | Packet Type 'chat room creation' |
| 10 | string |     | Name of the character who wants to create the room |
| 10 | string |     | Name of the character who should be invited to the room  |
| 1 | byte | 0x01   | "Type", not relevant? |
| 2 | ushort |      | Player id of the character who wants to create the room, big endian  |
| 2 | ushort |      | Server id of the character who wants to create the room, big endian  |
| 2 | ushort |      | Player id of the character who should be invited, big endian |
| 2 | ushort |      | Server id of the character who should be invited, big endian |

Example:
C1 25 A0 41 42 43 44 45
46 47 48 49 4A 50 51 52
53 54 55 56 57 58 59 01
20 01 00 01 20 02 00 01

### Chat Room Creation Responses

For each of both players, there is one data packet sent back to the ExDB Server:

|  Length  | Data type | Value | Description |
|----------|---------|-------------|---------|
| 1 | byte | 0xC1   | Packet header - type |
| 1 | byte | 0x2C   | Packet header - length of the packet |
| 1 | byte | 0xA0   | Packet Type 'chat room creation' |
| 1 | byte | 0x01   | Success flag |
| 2 | ushort |    | Chat room id, big endian |
| 10 | string |    | Name of the character to which a chat room invitation should be sent |
| 10 | string |    | Name of the chat partner character |
| 2 | ushort |    | Player id of the character to which a chat room invitation should be sent, big endian |
| 2 | ushort |     | Server id of the character to which a chat room invitation should be sent, big endian  |
| 2 | byte |    | Padding bytes for the alignment of the following authentication token |
| 4 | uint |  ‭‬  | Authentication token of the character to which a chat room invitation should be sent, big endian |
| 4 | uint |  ‭‬  | Authentication token of the chat partner, big endian |
| 1 | byte |    | 'Type' |
| 3 | byte |    | Don't know - padding?|

Example First Player:
C1 2C A0 01 00 00 41 42
43 44 45 46 47 48 49 4A
50 51 52 53 54 55 56 57
58 59 00 00 00 00 CC CC
00 00 11 04 01 00 BB 05
00 CC CC CC

Example Second Player:
C1 2C A0 01 00 00 50 51
52 53 54 55 56 57 58 59
41 42 43 44 45 46 47 48
49 4A 00 00 00 00 CC CC
01 00 BB 05 00 00 11 04
01 CC CC CC

### Chat Room Invitation Request

When a client requests to invite another friend to an existing chat room, the
following data packet is sent from the ExDB Server to the ChatServer.

|  Length  | Data type | Value | Description |
|----------|---------|-------------|---------|
| 1 | byte | 0xC1   | Packet header - type |
| 1 | byte | 0x16   | Packet header - length of the packet |
| 1 | byte | 0xA1   | Packet Type 'chat room invitation' |
| 1 | byte | 0x00   | Padding |
| 2 | ushort |  | Chat room id, big endian |
| 10 | string |    | Name of the character who should be invited to the room  |
| 2 | ushort |    | Player id of the character to which a chat room invitation should be sent, big endian |
| 2 | ushort |     | Server id of the character to which a chat room invitation should be sent, big endian  |
| 1 | byte |    | 'Type' |

Example:
C1 15 A1 00 00 00 61 62
63 64 65 66 67 68 69 6F
01 20 01 00 57

The ChatServer answers this with the same packet as above, but without filling
the second character name - no wonder, there is more than one player in the
room already.

Example:
C1 2C A0 01 00 00 61 62
63 64 65 66 67 68 69 6F
CC CC CC CC CC CC CC CC
CC CC 01 20 01 00 CC CC
02 00 C6 05 CC CC CC CC
57 CC CC CC

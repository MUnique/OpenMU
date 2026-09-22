---
title: Packet documentation
sidebar_label: Packets
sidebar_position: 2
description: Where to find the documentation of the network protocol.
---

# Packet documentation

The packet documentation describes the messages which are exchanged between
client and server. As stated in the project readme, the primary protocol is the
ENG (english) version of Season 6 Episode 3.

There is one file per packet. The documentation is **generated** from the XML
source files at
[`src/Network/Packets`](https://github.com/MUnique/OpenMU/tree/master/src/Network/Packets)
through XSLT, and the resulting ~500 markdown files live at
[`docs/Packets`](https://github.com/MUnique/OpenMU/tree/master/docs/Packets) in
the repository.

Because these files are generated and numerous, they are not part of this site
(yet). Browse them on GitHub:

| | |
|---|---|
| [Packets overview](https://github.com/MUnique/OpenMU/blob/master/docs/Packets/Readme.md) | Start here |
| [From game server to client](https://github.com/MUnique/OpenMU/blob/master/docs/Packets/ServerToClient.md) | |
| [From client to game server](https://github.com/MUnique/OpenMU/blob/master/docs/Packets/ClientToServer.md) | |
| [Between connect server and client](https://github.com/MUnique/OpenMU/blob/master/docs/Packets/ConnectServer.md) | |
| [Between chat server and client](https://github.com/MUnique/OpenMU/blob/master/docs/Packets/ChatServer.md) | |
| [Packet types](https://github.com/MUnique/OpenMU/blob/master/docs/Packets/PacketTypes.md) | What the leading byte `0xC1` – `0xC4` means |

## Contributing packet descriptions

Extend the **XML source files** at
[`src/Network/Packets`](https://github.com/MUnique/OpenMU/tree/master/src/Network/Packets),
not the generated markdown. To build the markdown files as well, install
[NodeJS 16+](https://nodejs.org) and rebuild the
`MUnique.OpenMU.Network.Packets` project.

## Related

* [Packet structure tests](https://github.com/MUnique/OpenMU/blob/master/docs/PacketStructureTests.md)
* The [network analyzer](https://github.com/MUnique/OpenMU/tree/master/src/Network/Analyzer),
  a tool to inspect the traffic between client and server

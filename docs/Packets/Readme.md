# Packet documentation

In this folder you find the documentation of the packet messages which are getting
exchanged between client and server. 

As stated in the project readme, the primary protocol is the ENG (english version)
of Season 6 Episode 3.

There is one file for each packet. The packet documentation is generated through
XSLT. The source XML files are at [MUnique.OpenMU.Network.Packets](https://github.com/MUnique/OpenMU/tree/master/src/Network/Packets).

In case you want to contribute packet documentations, please extend these source
files accordingly. If you want to build the markdown files as well,
you need to install `NodeJS 16+` and rebuild the `MUnique.OpenMU.Network.Packets` project.

## Packet types

MU Online packets can start with four different byte values (0xC1 to 0xC4) which
all have a different meaning. You can find a description [here](PacketTypes.md).

## Packets

  * [From Game Server to Client](ServerToClient.md)
  * [From Client to Game Server](ClientToServer.md)
  * [Between Connect Server and Client](ConnectServer.md)
  * [Between Chat Server and Client](ChatServer.md)
  
## Appearance

In some packets you might find something called "Appearance". You can find information
about the binary format [here](Appearance.md).

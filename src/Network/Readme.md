# Network

MUnique.OpenMU.Network contains all what's required to connect from and to a game server, using the MU Online network protocol.

[It](ListPacketQueue.cs) takes care that every data packet is completely received before it's handed over to further processing.
As soon as a data packet was received completely, it raises a PacketReceived-Event with the data packet.

As you might know, every MU Online data packet has the following structure:

* The first byte contains the type of the packet
  * 0xC1: A packet which is not (or very weakly) encrypted, with the maximum size of 255 bytes (1-byte length field)
  * 0xC2: A packet which is not (or very weakly) encrypted, with the maximum size of 65535 bytes (2-byte length field)
  * 0xC3: A packet which is encrypted, with the maximum size of 255 bytes (1-byte length field)
  * 0xC4: A packet which is encrypted, with the maximum size of 65535 bytes (2-byte length field)
* The next byte(s) contains the length of the packet.
  * 0xC1 and 0xC3: One byte containing the length of the packet
  * 0xC2 and 0xC4: Two bytes containing the length of the packet
* The following bytes are the payload, which might be encrypted or not, depending on the first byte. If it's a 0xC3 or 0xC4, the first byte of the payload also is a counter which goes from 0x00 to 0xFF, so that the consecutive encrypted packets always look different and replay attacks are difficult. However, this counter is not included in the packet given to the PacketReceived-EventHandler.

This project contains the encryption algorithms and [server-side keys](DefaultKeys.cs) which were used a few years ago at Season 6 Episode 3.
Please note, that these are not up-to-date and are known to the MU Online server (and cheater) community since over 10 years.
So if you want to use this project for your own server, please consider to change the encryption algorithms and keys.
The used encryption is encapsulated by interfaces, so it shouldn't be difficult.

## Known possible performance implications
This project works with byte arrays, and creates them for every received packet. When encryption is involved it might even create more
array instances. This could lead to some pressure on the garbage collector. At the moment we leave it as it is, but if this is a problem in the future,
we could switch to use some fix-sized buffer in combination with ArraySegment structures.
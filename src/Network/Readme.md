# Network

MUnique.OpenMU.Network contains all what's required to connect from and to a
MU Online game, connect or chat server, using the MU Online network protocol.

## Packet structure

As you might know, every MU Online data packet has the following structure:

* The first byte contains the type of the packet

  * 0xC1: A packet which is not (or very weakly) encrypted, with the maximum
          size of 255 bytes (1-byte length field)

  * 0xC2: A packet which is not (or very weakly) encrypted, with the maximum
          size of 65535 bytes (2-byte length field)

  * 0xC3: A packet which is encrypted, with the maximum size of 255 bytes
          (1-byte length field)

  * 0xC4: A packet which is encrypted, with the maximum size of 65535 bytes
          (2-byte length field)

* The next byte(s) contains the length of the packet.

  * 0xC1 and 0xC3: One byte containing the length of the packet

  * 0xC2 and 0xC4: Two bytes containing the length of the packet

* The following bytes are the payload, which might be encrypted or not,
  depending on the first byte. If it's a 0xC3 or 0xC4, the first byte of the
  encrypted payload also is a counter which goes from 0x00 to 0xFF, so that the
  consecutive encrypted packets always look different and replay attacks are
  difficult. However, this counter is not included in the packet given to the
  PacketReceived-EventHandler.

## Pipes

For performance reasons, we make use of Pipes (System.IO.Pipelines) to process
and send data packets.
To encrypt and decrypt data packets, these pipes are chained. You can check the
unit tests if you're interested how that works.

Just one example how they can be chained:

```csharp
private async Task EncryptDecryptFromClientToServer(byte[] packet)
{
    // this pipe connects the encryptor with the decryptor.
    // You can imagine this as the client-to-server network
    // connection, for example.
    var pipe = new Pipe();

    var encryptor = new PipelinedXor32Encryptor(
      new PipelinedSimpleModulusEncryptor(pipe.Writer, PipelinedSimpleModulusEncryptor.DefaultClientKey).Writer);
    var decryptor = new PipelinedXor32Decryptor(new PipelinedSimpleModulusDecryptor(pipe.Reader).Reader);
    encryptor.Writer.Write(packet);
    await encryptor.Writer.FlushAsync();
    var readResult = await decryptor.Reader.ReadAsync().ConfigureAwait(false);

    var result = readResult.Buffer.ToArray();
    Assert.That(result, Is.EquivalentTo(packet));
}
```

One thing to note is, that after a complete packet has been written to a
PipeWriter, FlushAsync is called - therefore the consumer, which waits to read
the next packet, is activated.

## Sending data

There is an extension method (```ConnectionExtensions.StartSafeWrite(this IConnection connection, byte packetType, int expectedPacketSize)```)
to make it as easy as possible to safely send data by writing to a pipe in
scenarios where multiple threads want to do that.

A simple example of the usage:

```csharp
// The next line requests 5 bytes from the pipe writer of the connection,
// locks the connection and sets the header bytes for packet type and
// length (e.g. C1 05).
// Disposing the writer is releasing the lock, so you don't want to keep
// this open too long.
using (var writer = connection.StartSafeWrite(0xC1, 5)) 
{
    var packet = writer.Span;
    packet[2] = 0x40; // The packet type (e.g. 0x40 for 'show party request')
    packet[3] = requester.Id.GetHighByte();
    packet[4] = requester.Id.GetLowByte();

    // Commit advances the pipe writer by 5 bytes.
    // An overload exists to advance it by a custom length for dynamically
    // sized packets.
    writer.Commit();
}
```

## Encryption

This project contains the encryption algorithms and client and server keys
which were used from 0.75 until Season 6 Episode 3.
Please note, that these are not up-to-date and are known to the MU Online
server (and cheater) community since over 10 years, so they're not secure at
all. The keys can be calculated based on known packet content or bruteforced in
a fraction of a second.

So, if you want to use this project for your own server, please consider to
change the encryption algorithms and keys.
The used encryption algorithms are encapsulated by interfaces (IPipelinedDecryptor,
IPipelinedEncryptor), so it shouldn't be difficult on server-side.

### History

#### Below version 0.74

As far as we know, there wasn't a packet encryption in the earliest versions
below 0.74.

#### Version 0.74.01 and higher

According to a korean change log, SimpleModulus got added with version **0.74.01**.
The first variant used 16 encryption keys which were used to encrypt blocks of
32 bytes size.

#### Somehwere between 0.75 and 0.97

* A counter got added to the first encrypted payload byte, so that every
  encrypted packet looked different and replay attacks were prevented (at least
  if you couldn't count ;-)).

* The XOR encryption with the 32 byte long key got added.

* The block size got reduced from 32 bytes to 8 bytes (unencrypted). That also
  means, each block is only encrypted by 4 instead of previously 16 keys.

#### Season 6

The algorithms of SimpleModulus and the Xor32 encryption were still used.
However, they desperately changed the keys of the Xor32 encryption during
maintenances.

#### ex700 (Season 7) and higher

The packet twister algorithm got added. This project also contains the
PacketTwister algorithm but it's unknown if it's complete and how long it was
in use in this form.

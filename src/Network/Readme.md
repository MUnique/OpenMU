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

There are two 'flavours' of packet structs in the ```MUnique.OpenMU.Network.Packets```
project:
  * ref-structs with names ending with 'Ref', which wrap ```Span<byte>```. They
    are more of a help when sending packets. The benefit is, that they never
    cause memory allocations on the heap.
  * normal structs, which wrap ```Memory<byte>``` without the 'Ref' suffix. They
    allow easier access in async code, where ref-structs are not allowed.

You can cast the structs to Memory or Span back and forth implicitly for easy usage.

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

There are extension methods for all known and "simple" network packets
in the ```MUnique.OpenMU.Network.Packets``` project and their sub-namespaces 
(e.g. ClientToServer) to make it as easy as possible to send data thread-safe.

A simple example of the usage:

```csharp
// assuming 'connection' is your IConnection instance:
await connection.SendPublicChatMessageAsync("Bob", "Hello Alice, how are you?").ConfigureAwait(false);
```

For more complicated packets, e.g. when arrays of structures are involved, the
following pattern in this example code is helpful:

```csharp
// Again, assume that connection is your IConnection instance. Additionally,
// assume that objectIds is a list of objects which leave the scope of a player.

public void SendObjectsOutOfScope(List<ushort> objectIds)
{
    // Write takes a span from the connection.Output and writes data into it
    // by wrapping the ref packet struct around it.
    int Write()
    {
        var count = objectIds.Length;
        var size = MapObjectOutOfScopeRef.GetRequiredSize(count);
        var span = connection.Output.GetSpan(size)[..size];
        var packet = new MapObjectOutOfScopeRef(span)
        {
            ObjectCount = (byte)count,
        };

        for (int i = 0; i < count; i++)
        {
            var objectIdStruct = packet[i];
            objectIdStruct.Id = objectIds[i];
        }

        return size;
    }

    // SendAsync is an extension method which takes the local non-async method 'Write'.
    await connection.SendAsync(Write).ConfigureAwait(false);
}
```

## Reading data

Reading data is even easier. If you have a ```Memory<byte>``` of your data,
just implicitly cast it to your wanted packet structure.

Example:
```csharp
/// <inheritdoc/>
public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
{
    ConsumeItemRequest message = packet;
    await this._consumeAction.HandleConsumeRequestAsync(player, message.ItemSlot, message.TargetSlot, Convert(message.FruitConsumption)).ConfigureAwait(false);
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

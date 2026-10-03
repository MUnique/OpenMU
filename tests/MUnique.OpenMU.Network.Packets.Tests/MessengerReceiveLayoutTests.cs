// <copyright file="MessengerReceiveLayoutTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Packets.Tests;

using System.Buffers.Binary;
using System.Text;
using MUnique.OpenMU.Network.Packets.ServerToClient;

/// <summary>
/// Writes messenger packets through the generated packet structs and checks that every value lands
/// at the byte offset where MuMain's receive structs read it (<c>FS_LETTER_ALERT</c>: a 60 byte subject,
/// <c>MAX_LETTER_TITLE_LENGTH</c>, and the read state after it).
/// </summary>
[TestFixture]
public class MessengerReceiveLayoutTests
{
    /// <summary>
    /// The letter list entry: index at 4, sender at 6 (10 bytes), timestamp at 16 (30 bytes),
    /// subject at 46 (60 bytes), state at 106; 107 bytes.
    /// </summary>
    [Test]
    public void LetterListEntry()
    {
        var subject = new string('s', 60);
        var data = new byte[AddLetter.Length];
        _ = new AddLetter(data)
        {
            LetterIndex = 0x0102,
            SenderName = "Sender",
            Timestamp = "2026-09-29 12:34:56",
            Subject = subject,
            State = AddLetter.LetterState.New,
        };

        Assert.Multiple(() =>
        {
            Assert.That(data.Length, Is.EqualTo(107));
            Assert.That(data[1], Is.EqualTo(107));
            Assert.That(BinaryPrimitives.ReadUInt16LittleEndian(data.AsSpan(4)), Is.EqualTo(0x0102));
            Assert.That(Encoding.UTF8.GetString(data, 6, 6), Is.EqualTo("Sender"));
            Assert.That(Encoding.UTF8.GetString(data, 16, 19), Is.EqualTo("2026-09-29 12:34:56"));
            Assert.That(Encoding.UTF8.GetString(data, 46, 60), Is.EqualTo(subject));
            Assert.That(data[106], Is.EqualTo((byte)AddLetter.LetterState.New));
        });
    }
}

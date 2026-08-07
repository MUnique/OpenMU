// <copyright file="GlobalPacketHeaderTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Packets.Tests;

using MUnique.OpenMU.Network.Packets.ServerToClient;
using NUnit.Framework;

/// <summary>
/// Verifies wire headers which cannot be validated by field-boundary tests alone.
/// </summary>
[TestFixture]
public class GlobalPacketHeaderTests
{
    /// <summary>
    /// Ensures that writing the object id does not overwrite the C2 subcode.
    /// </summary>
    [Test]
    public void AddCharacterToScopeGlobalPreservesSubCodeWhenIdIsWritten()
    {
        var data = new byte[AddCharacterToScopeGlobalRef.GetRequiredSize(0)];
        var packet = new AddCharacterToScopeGlobalRef(data);

        packet.Id = 0x1234;

        Assert.That(data[0], Is.EqualTo(0xC2));
        Assert.That(data[3], Is.EqualTo(0x12));
        Assert.That(data[4], Is.EqualTo(0xD6));
        Assert.That(data[5], Is.EqualTo(0x34));
        Assert.That(data[6], Is.EqualTo(0x12));
    }

}

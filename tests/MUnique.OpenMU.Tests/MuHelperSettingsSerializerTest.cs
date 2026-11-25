// <copyright file="MuHelperSettingsSerializerTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.GameServer.RemoteView.MuHelper;

/// <summary>
/// Tests for <see cref="MuHelperSettingsSerializer"/>.
/// </summary>
[TestFixture]
public class MuHelperSettingsSerializerTest
{
    private const int PickSocketMinimumOffset = 30;
    private const int ExtraItemsOffset = 65;

    /// <summary>
    /// Tests that the minimum socket count is read from its byte, 0 means off, and values above 5 count as 5.
    /// </summary>
    /// <param name="value">The value of the byte.</param>
    /// <returns>The deserialized minimum socket count.</returns>
    [TestCase((byte)0, ExpectedResult = 0)]
    [TestCase((byte)1, ExpectedResult = 1)]
    [TestCase((byte)3, ExpectedResult = 3)]
    [TestCase((byte)5, ExpectedResult = 5)]
    [TestCase((byte)9, ExpectedResult = 5)]
    [TestCase((byte)255, ExpectedResult = 5)]
    public int ReadsPickSocketMinimum(byte value)
    {
        var blob = new byte[257];
        blob[PickSocketMinimumOffset] = value;

        return MuHelperSettingsSerializer.TryDeserialize(blob)!.PickSocketMinimum;
    }

    /// <summary>
    /// Tests that the minimum socket count doesn't change the extra item names which follow it.
    /// </summary>
    [Test]
    public void PickSocketMinimumKeepsExtraItemNames()
    {
        var blob = new byte[257];
        blob[PickSocketMinimumOffset] = 3;
        "Sphere (Tri)"u8.ToArray().CopyTo(blob, ExtraItemsOffset);

        var settings = MuHelperSettingsSerializer.TryDeserialize(blob)!;

        Assert.That(settings.PickSocketMinimum, Is.EqualTo(3));
        Assert.That(settings.ExtraItemNames, Is.EqualTo(new[] { "Sphere (Tri)" }));
    }
}

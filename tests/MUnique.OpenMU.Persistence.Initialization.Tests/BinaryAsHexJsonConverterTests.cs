// <copyright file="BinaryAsHexJsonConverterTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using System.Text.Json;
using MUnique.OpenMU.Persistence.EntityFramework.Json;

/// <summary>
/// Tests for the <see cref="BinaryAsHexJsonConverter"/>.
/// </summary>
[TestFixture]
public class BinaryAsHexJsonConverterTests
{
    /// <summary>
    /// Tests if a byte array is read back completely.
    /// It previously lost its last two bytes, because the prefix was subtracted twice.
    /// </summary>
    /// <param name="length">The length of the tested byte array.</param>
    [TestCase(0)]
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    [TestCase(10)]
    [TestCase(65537)] // the size of the terrain data of a game map
    public void ByteArrayIsReadCompletely(int length)
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new BinaryAsHexJsonConverter());

        var data = new byte[length];
        new Random(42).NextBytes(data);

        var json = JsonSerializer.Serialize(data, options);
        var readData = JsonSerializer.Deserialize<byte[]>(json, options);

        Assert.That(readData, Is.EqualTo(data));
    }
}

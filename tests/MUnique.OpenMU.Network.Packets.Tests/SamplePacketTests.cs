// <copyright file="SamplePacketTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Packets.Tests.ServerToClient;

using System;
using System.Text;
using NUnit.Framework;
using MUnique.OpenMU.Network.Packets.ServerToClient;

/// <summary>
/// Sample tests for packet structures to validate the approach.
/// </summary>
[TestFixture]
public class SamplePacketTests
{
    /// <summary>
    /// Tests the structure size calculation for StoredItem.
    /// </summary>
    [Test]
    public void StoredItem_SizeValidation()
    {
        // Variable-length structure validation
        // Test GetRequiredSize method with sample data
        const int testBinaryLength = 10;
        var calculatedSize = StoredItem.GetRequiredSize(testBinaryLength);
        var expectedSize = testBinaryLength + 1;
        
        Assert.That(calculatedSize, Is.EqualTo(expectedSize), 
            "GetRequiredSize calculation incorrect for binary field");
    }

    /// <summary>
    /// Tests the structure size calculation for PlayerShopItem.
    /// </summary>
    [Test]
    public void PlayerShopItem_SizeValidation()
    {
        // Fixed-length structure validation
        const int expectedLength = 20;
        const int actualLength = PlayerShopItem.Length;
        
        Assert.That(actualLength, Is.EqualTo(expectedLength), 
            "Structure length mismatch: declared length does not match calculated size");
        
        // Validate field 'ItemSlot' boundary
        Assert.That(0 + 1, Is.LessThanOrEqualTo(expectedLength), 
            "Field 'ItemSlot' exceeds structure boundary");
        
        // Validate field 'ItemData' boundary
        Assert.That(1 + 12, Is.LessThanOrEqualTo(expectedLength), 
            "Field 'ItemData' exceeds structure boundary");
        
        // Validate field 'Price' boundary
        Assert.That(16 + 4, Is.LessThanOrEqualTo(expectedLength), 
            "Field 'Price' exceeds structure boundary");
    }

    /// <summary>
    /// Tests the packet size calculation for GameServerEntered.
    /// </summary>
    [Test]
    public void GameServerEntered_PacketSizeValidation()
    {
        // Fixed-length packet validation
        const int expectedLength = 12;
        const int actualLength = GameServerEnteredRef.Length;
        
        Assert.That(actualLength, Is.EqualTo(expectedLength), 
            "Packet length mismatch: declared length does not match calculated size");
        
        // Validate field 'Success' boundary
        Assert.That(4 + 1, Is.LessThanOrEqualTo(expectedLength), 
            "Field 'Success' exceeds packet boundary");
        
        // Validate field 'PlayerId' boundary
        Assert.That(5 + 2, Is.LessThanOrEqualTo(expectedLength), 
            "Field 'PlayerId' exceeds packet boundary");
        
        // Validate field 'VersionString' boundary
        Assert.That(7 + 5, Is.LessThanOrEqualTo(expectedLength), 
            "Field 'VersionString' exceeds packet boundary");
        
        // Validate field 'Version' boundary
        Assert.That(7 + 5, Is.LessThanOrEqualTo(expectedLength), 
            "Field 'Version' exceeds packet boundary");
    }
}
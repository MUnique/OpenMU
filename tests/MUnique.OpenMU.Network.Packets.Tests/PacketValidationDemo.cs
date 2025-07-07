// <copyright file="PacketValidationDemo.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Packets.Tests;

using System;
using NUnit.Framework;

/// <summary>
/// Demonstration of how the packet validation tests would catch incorrect packet definitions.
/// </summary>
[TestFixture]
public class PacketValidationDemo
{
    /// <summary>
    /// Demo test showing how field boundary validation would catch buffer overruns.
    /// This simulates the kind of error that would be caught by our generated tests.
    /// </summary>
    [Test]
    public void FieldBoundaryValidation_WouldCatchBufferOverrun()
    {
        // Simulate a packet with declared length 10 but fields that exceed this
        const int declaredLength = 10;
        const int fieldIndex = 8;
        const int fieldSize = 4; // This would cause field to end at index 12, exceeding declared length of 10
        
        // This is the kind of assertion our generated tests would include
        Assert.That(fieldIndex + fieldSize, Is.LessThanOrEqualTo(declaredLength), 
            "Field exceeds packet boundary - this would catch the issue mentioned in PR #622");
    }
    
    /// <summary>
    /// Demo test showing how variable-length packet size validation would work.
    /// </summary>
    [Test]
    public void VariableLengthPacketValidation_WouldCatchSizeErrors()
    {
        // Simulate a GetRequiredSize calculation
        const string testString = "TestData";
        const int baseSize = 5;
        const int expectedSize = 8 + 1 + baseSize; // UTF8 bytes + null terminator + header
        
        // This simulates what our generated tests would do
        var calculatedSize = System.Text.Encoding.UTF8.GetByteCount(testString) + 1 + baseSize;
        
        Assert.That(calculatedSize, Is.EqualTo(expectedSize), 
            "GetRequiredSize calculation should match expected size");
    }
    
    /// <summary>
    /// Demo test showing validation of overlapping fields.
    /// </summary>
    [Test]
    public void OverlappingFieldValidation_WouldCatchFieldConflicts()
    {
        // Simulate two fields that would overlap
        const int field1Index = 5;
        const int field1Size = 4;
        const int field2Index = 7; // This starts before field1 ends (at index 9)
        const int field2Size = 2;
        
        // This type of validation could be added to catch overlapping fields
        Assert.That(field2Index, Is.GreaterThanOrEqualTo(field1Index + field1Size), 
            "Field 2 starts before field 1 ends - fields overlap");
    }
}
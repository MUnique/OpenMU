// <copyright file="ChatCommandPacketTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.Network.Packets.ServerToClient;

/// <summary>
/// Tests for the packets which describe the chat commands.
/// </summary>
[TestFixture]
public class ChatCommandPacketTest
{
    /// <summary>
    /// Tests that the values of a command survive the way through the packet, so that
    /// none of the fields overlaps another one.
    /// </summary>
    [Test]
    public void AvailableChatCommandKeepsItsValues()
    {
        const int parameterCount = 3;
        var data = new byte[AvailableChatCommandRef.GetRequiredSize(parameterCount)];

        var written = new AvailableChatCommandRef(data)
        {
            Index = 7,
            Count = 42,
            MinimumCharacterStatus = CharacterStatus.GameMaster,
            ParameterCount = parameterCount,
            Command = "/item",
            Name = "Item chat command",
            Description = "Drops a specific item next to the character.",
        };

        var group = written[0];
        group.Name = "Group";
        group.ShortName = "group";
        group.IsRequired = true;
        group.Type = ChatCommandParameterType.Number;

        var ancient = written[1];
        ancient.Name = "Ancient";
        ancient.ShortName = "anc";
        ancient.ValidValues = "0|1|2";
        ancient.Type = ChatCommandParameterType.Number;

        var skill = written[2];
        skill.Name = "Skill";
        skill.ShortName = "sk";
        skill.Type = ChatCommandParameterType.Boolean;

        var read = new AvailableChatCommandRef(data);

        Assert.That(read.Header.Code, Is.EqualTo(0xF5));
        Assert.That(read.Header.SubCode, Is.EqualTo(0x01));
        Assert.That(read.Index, Is.EqualTo(7));
        Assert.That(read.Count, Is.EqualTo(42));
        Assert.That(read.MinimumCharacterStatus, Is.EqualTo(CharacterStatus.GameMaster));
        Assert.That(read.ParameterCount, Is.EqualTo(parameterCount));
        Assert.That(read.Command, Is.EqualTo("/item"));
        Assert.That(read.Name, Is.EqualTo("Item chat command"));
        Assert.That(read.Description, Is.EqualTo("Drops a specific item next to the character."));

        Assert.That(read[0].Name, Is.EqualTo("Group"));
        Assert.That(read[0].ShortName, Is.EqualTo("group"));
        Assert.That(read[0].IsRequired, Is.True);
        Assert.That(read[0].ValidValues, Is.Empty);

        Assert.That(read[1].Name, Is.EqualTo("Ancient"));
        Assert.That(read[1].ShortName, Is.EqualTo("anc"));
        Assert.That(read[1].ValidValues, Is.EqualTo("0|1|2"));
        Assert.That(read[1].IsRequired, Is.False);

        Assert.That(read[2].Name, Is.EqualTo("Skill"));
        Assert.That(read[2].Type, Is.EqualTo(ChatCommandParameterType.Boolean));
    }

    /// <summary>
    /// Tests that the size grows by exactly one parameter entry, so that the parameters
    /// don't overlap the fields before them.
    /// </summary>
    [Test]
    public void AvailableChatCommandSizeDependsOnParameterCount()
    {
        var withoutParameters = AvailableChatCommandRef.GetRequiredSize(0);
        var withOneParameter = AvailableChatCommandRef.GetRequiredSize(1);

        Assert.That(withOneParameter - withoutParameters, Is.EqualTo(AvailableChatCommand.ChatCommandParameter.Length));
    }
}

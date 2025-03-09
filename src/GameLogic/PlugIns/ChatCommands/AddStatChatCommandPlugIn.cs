// <copyright file="AddStatChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which handles the command to add stat points.
/// </summary>
[Guid("042EC5C6-27C8-4E00-A48B-C5458EDEA0BC")]
[PlugIn("Add Stat chat command", "Handles the chat command '/add (ene|agi|vit|str|cmd) (amount)'. Adds the specified amount of stat points to the specified attribute of the character.")]
[ChatCommandHelp(Command, "Adds the specified amount of stat points to the specified attribute of the character.", typeof(Arguments), MinimumStatus)]
public class AddStatChatCommandPlugIn : AddStatChatCommandPlugInBase
{
    private const string Command = "/add";

    private const CharacterStatus MinimumStatus = CharacterStatus.Normal;

    /// <inheritdoc />
    public override string Key => Command;

    /// <inheritdoc />
    public override CharacterStatus MinCharacterStatusRequirement => MinimumStatus;

    /// <inheritdoc />
    protected override (AttributeDefinition Stat, ushort Amount) GetStatAndAmount(string command)
    {
        var arguments = command.ParseArguments<Arguments>();
        var stat = arguments.StatType switch
        {
            "str" => Stats.BaseStrength,
            "agi" => Stats.BaseAgility,
            "vit" => Stats.BaseVitality,
            "ene" => Stats.BaseEnergy,
            "cmd" => Stats.BaseLeadership,
            _ => throw new ArgumentException($"Unknown stat: '{arguments.StatType}'."),
        };
        return (Stat: stat, Amount: arguments.Amount);
    }

    private class Arguments : ArgumentsBase
    {
        [ValidValues("str", "agi", "vit", "ene", "cmd")]
        public string? StatType { get; set; }

        public ushort Amount { get; set; }
    }
}
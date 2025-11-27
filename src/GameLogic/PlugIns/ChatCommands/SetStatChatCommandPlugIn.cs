// <copyright file="SetStatChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which handles the command to add stat points.
/// </summary>
[Guid("D074E8AB-9D6E-49A4-956F-1F4818188AF1")]
[PlugIn("Set Stat chat command", "Set stat points. Usage: /set (ene|agi|vit|str|cmd) (amount) (optional:character)")]
[ChatCommandHelp(Command, "Set stat points. Usage: /set (ene|agi|vit|str|cmd) (amount) (optional:character)", typeof(Arguments), MinimumStatus)]
public class SetStatChatCommandPlugIn : ChatCommandPlugInBase<SetStatChatCommandPlugIn.Arguments>, IDisabledByDefault
{
    private const string Command = "/set";

    private const CharacterStatus MinimumStatus = CharacterStatus.GameMaster;

    /// <inheritdoc />
    public override string Key => Command;

    /// <inheritdoc />
    public override CharacterStatus MinCharacterStatusRequirement => MinimumStatus;

    /// <inheritdoc />
    protected override async ValueTask DoHandleCommandAsync(Player player, Arguments arguments)
    {
        if (arguments is null)
        {
            return;
        }

        var targetPlayer = player;
        if (arguments.CharacterName is { } characterName)
        {
            targetPlayer = player.GameContext.GetPlayerByCharacterName(characterName);
            if (targetPlayer?.SelectedCharacter is null ||
                !targetPlayer.SelectedCharacter.Name.Equals(characterName, StringComparison.OrdinalIgnoreCase))
            {
                await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.CharacterNotFound), characterName).ConfigureAwait(false);
                return;
            }
        }

        if (targetPlayer.SelectedCharacter is not { } selectedCharacter)
        {
            return;
        }

        var attribute = this.GetAttribute(selectedCharacter, arguments.StatType);
        if (attribute.MaximumValue is null && arguments.Amount < 0)
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.InvalidStatValue), arguments.StatType).ConfigureAwait(false);
            return;
        }

        if (attribute.MaximumValue < 0 || arguments.Amount > attribute.MaximumValue)
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.InvalidStatValueRange), arguments.StatType, attribute.MaximumValue).ConfigureAwait(false);
            return;
        }

        targetPlayer.Attributes![attribute] = arguments.Amount;
        await targetPlayer.InvokeViewPlugInAsync<IUpdateCharacterBaseStatsPlugIn>(p => p.UpdateCharacterBaseStatsAsync()).ConfigureAwait(false);
        await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.SetStatResult), arguments.StatType, arguments.Amount, targetPlayer.SelectedCharacter.Name).ConfigureAwait(false);
    }

    private AttributeDefinition GetAttribute(Character selectedCharacter, string? statType)
    {
        var attribute = statType switch
        {
            "str" => Stats.BaseStrength,
            "agi" => Stats.BaseAgility,
            "vit" => Stats.BaseVitality,
            "ene" => Stats.BaseEnergy,
            "cmd" => Stats.BaseLeadership,
            _ => throw new ArgumentException($"Unknown stat: '{statType}'."),
        };

        if (selectedCharacter.Attributes.All(sa => sa.Definition != attribute))
        {
            throw new ArgumentException($"The character has no stat attribute '{statType}'.");
        }

        return attribute;
    }

    /// <summary>
    /// Arguments for the Get Stat chat command.
    /// </summary>
    public class Arguments : ArgumentsBase
    {
        /// <summary>
        /// Gets or sets the stat type to set.
        /// </summary>
        [ValidValues("str", "agi", "vit", "ene", "cmd")]
        public string? StatType { get; set; }

        /// <summary>
        /// Gets or sets the amount to set.
        /// </summary>
        public ushort Amount { get; set; }

        /// <summary>
        /// Gets or sets the character name to set stat for.
        /// </summary>
        public string? CharacterName { get; set; }
    }
}
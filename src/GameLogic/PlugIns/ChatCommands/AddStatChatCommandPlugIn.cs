// <copyright file="AddStatChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlayerActions.Character;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which handles the command to add stat points.
/// </summary>
[Guid("042EC5C6-27C8-4E00-A48B-C5458EDEA0BC")]
[PlugIn]
[Display(Name = nameof(PlugInResources.AddStatChatCommandPlugIn_Name), Description = nameof(PlugInResources.AddStatChatCommandPlugIn_Description), ResourceType = typeof(PlugInResources))]
[ChatCommandHelp(Command, typeof(Arguments), MinimumStatus)]
public class AddStatChatCommandPlugIn : ChatCommandPlugInBase<AddStatChatCommandPlugIn.Arguments>
{
    private const string Command = "/add";

    private const CharacterStatus MinimumStatus = CharacterStatus.Normal;

    private readonly IncreaseStatsAction _action = new();

    /// <inheritdoc />
    public override string Key => Command;

    /// <inheritdoc />
    public override CharacterStatus MinCharacterStatusRequirement => MinimumStatus;

    /// <inheritdoc />
    protected override async ValueTask DoHandleCommandAsync(Player player, Arguments arguments)
    {
        if (player.SelectedCharacter is null)
        {
            return;
        }

        var attribute = await this.TryGetAttributeAsync(player, arguments.StatType).ConfigureAwait(false);
        if (attribute is null)
        {
            return;
        }

        var selectedCharacter = player.SelectedCharacter;

        if (!selectedCharacter.CanIncreaseStats(arguments.Amount))
        {
            return;
        }

        if (player.CurrentMiniGame is not null)
        {
            await player.ShowLocalizedBlueMessageAsync(PlayerMessage.AddingMultiplePointsWhileMiniGameNotAllowed).ConfigureAwait(false);
            return;
        }

        await this._action.IncreaseStatsAsync(player, attribute, arguments.Amount).ConfigureAwait(false);
    }

    /// <summary>
    /// Arguments for this command.
    /// </summary>
    public class Arguments : ArgumentsBase
    {
        /// <summary>
        /// Gets or sets the type of the stat.
        /// </summary>
        [ValidValues("str", "agi", "vit", "ene", "cmd")]
        public string? StatType { get; set; }

        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        public ushort Amount { get; set; }
    }
}
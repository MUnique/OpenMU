// <copyright file="ResetInfoChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Resets;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which shows reset costs and rewards.
/// </summary>
[Guid("79F2C2C2-2E4C-4F4B-8A74-4227D1209D27")]
[PlugIn]
[Display(Name = "Reset Info Command", Description = "Shows required costs and granted points for the next reset.")]
[ChatCommandHelp(Command, "Shows required costs and gained points for the next reset.", null)]
public class ResetInfoChatCommandPlugIn : IChatCommandPlugIn
{
    private const string Command = "/resetinfo";

    /// <inheritdoc />
    public string Key => Command;

    /// <inheritdoc />
    public CharacterStatus MinCharacterStatusRequirement => CharacterStatus.Normal;

    /// <inheritdoc />
    public async ValueTask HandleCommandAsync(Player player, string command)
    {
        var configuration = player.GameContext.FeaturePlugIns.GetPlugIn<ResetFeaturePlugIn>()?.Configuration;
        if (configuration is null)
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.ResetSystemInactive)).ConfigureAwait(false);
            return;
        }

        if (player.Attributes is null || player.SelectedCharacter is null)
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.NotEnteredTheGame)).ConfigureAwait(false);
            return;
        }

        var progression = ResetProgressionCalculator.Calculate(
            (int)player.Attributes[Stats.Resets],
            (int)player.Attributes[Stats.PointsPerReset],
            configuration);
        var itemCostText = progression.RequiredItemAmount <= 0 || configuration.RequiredResetItem is not { Name: { } itemName }
            ? "none"
            : $"{progression.RequiredItemAmount} x {itemName}";
        var pointsText = configuration.ReplacePointsPerReset
            ? $"{progression.PointsForReset} this reset ({progression.TotalPointsAfterReset} total after reset)"
            : $"{progression.PointsForReset}";

        var message = $"Next reset #{progression.NextResetCount}: requires level {configuration.RequiredLevel}, zen {progression.RequiredZen}, item cost {itemCostText}; grants {pointsText} stat points.";
        await player.ShowBlueMessageAsync(message).ConfigureAwait(false);
    }
}

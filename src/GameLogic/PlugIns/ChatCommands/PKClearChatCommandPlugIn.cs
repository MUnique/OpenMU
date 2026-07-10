// <copyright file="PKClearChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which handles pk clear commands.
/// </summary>
[Guid("EB97A8F6-F6BD-460A-BCBE-253BF679361A")]
[PlugIn]
[Display(Name = nameof(PlugInResources.PkClearChatCommandPlugIn_Name), Description = nameof(PlugInResources.PkClearChatCommandPlugIn_Description), ResourceType = typeof(PlugInResources))]
[ChatCommandHelp(Command, typeof(PkClearChatCommandArgs), CharacterStatus.Normal)]
public class PkClearChatCommandPlugIn : ChatCommandPlugInBase<PkClearChatCommandArgs>, ISupportCustomConfiguration<PkClearChatCommandPlugIn.PKClearConfiguration>, ISupportDefaultCustomConfiguration
{
    private const string Command = "/pkclear";

    /// <summary>
    /// Gets or sets the configuration.
    /// </summary>
    public PKClearConfiguration? Configuration { get; set; }

    /// <inheritdoc />
    public override string Key => Command;

    /// <inheritdoc/>
    public override CharacterStatus MinCharacterStatusRequirement => CharacterStatus.Normal;

    /// <inheritdoc />
    public object CreateDefaultConfig() => new PKClearConfiguration();

    /// <inheritdoc />
    protected override async ValueTask DoHandleCommandAsync(Player player, PkClearChatCommandArgs arguments)
    {
        var selectedCharacter = player.SelectedCharacter;
        if (selectedCharacter is null)
        {
            return;
        }

        var configuration = this.Configuration ?? (PKClearConfiguration)this.CreateDefaultConfig();
        var isGameMaster = selectedCharacter.CharacterStatus >= CharacterStatus.GameMaster;

        Player? targetPlayer;
        if (isGameMaster)
        {
            targetPlayer = await this.GetPlayerByCharacterNameAsync(player, arguments.CharacterName ?? string.Empty).ConfigureAwait(false);
            if (targetPlayer is null)
            {
                return;
            }
        }
        else
        {
            if (!configuration.AllowRegularPlayers)
            {
                await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.PkClearCommandOnlyForGameMasters)).ConfigureAwait(false);
                return;
            }

            if (!string.IsNullOrEmpty(arguments.CharacterName) && !arguments.CharacterName.Equals(player.Name, StringComparison.OrdinalIgnoreCase))
            {
                await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.PkClearOnlyClearOwnPk)).ConfigureAwait(false);
                return;
            }

            targetPlayer = player;
        }

        var targetCharacter = targetPlayer.SelectedCharacter;
        if (targetCharacter is null)
        {
            return;
        }

        if (targetCharacter.PlayerKillCount == 0 && targetCharacter.State < HeroState.PlayerKillWarning)
        {
            if (isGameMaster)
            {
                await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.PkClearTargetNotPlayerKiller), targetCharacter.Name).ConfigureAwait(false);
            }
            else
            {
                await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.PkClearYouAreNotPlayerKiller)).ConfigureAwait(false);
            }

            return;
        }

        if (!isGameMaster)
        {
            var cost = (long)targetCharacter.PlayerKillCount * configuration.ZenCostPerKill;
            if (cost > int.MaxValue)
            {
                cost = int.MaxValue;
            }

            if (cost > 0 && !player.TryRemoveMoney((int)cost))
            {
                await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.NotEnoughMoney)).ConfigureAwait(false);
                return;
            }
        }

        targetCharacter.State = HeroState.Normal;
        targetCharacter.StateRemainingSeconds = 0;
        targetCharacter.PlayerKillCount = 0;
        await targetPlayer.ForEachWorldObserverAsync<IUpdateCharacterHeroStatePlugIn>(p => p.UpdateCharacterHeroStateAsync(targetPlayer), true).ConfigureAwait(false);

        if (!targetPlayer.Name.Equals(player.Name))
        {
            await targetPlayer.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.PkStatusClearedByGameMaster)).ConfigureAwait(false);
        }

        await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.PkClearedResult), this.Key, targetPlayer.Name).ConfigureAwait(false);
    }

    /// <summary>
    /// Configuration for the <see cref="PkClearChatCommandPlugIn"/>.
    /// </summary>
    public class PKClearConfiguration
    {
        /// <summary>
        /// Gets or sets the Zen cost per kill.
        /// </summary>
        [Display(Name = "Zen Cost Per Kill", Description = "The amount of Zen required to clear one PK count.")]
        public int ZenCostPerKill { get; set; } = 10_000_000;

        /// <summary>
        /// Gets or sets a value indicating whether regular players are allowed to clear their PK status.
        /// </summary>
        [Display(Name = "Allow Regular Players", Description = "Allows regular players to use this command to clear their own PK status for Zen.")]
        public bool AllowRegularPlayers { get; set; } = true;
    }
}
// <copyright file="NpcChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which opens NPC windows.
/// </summary>
[Guid("D8AC2F15-AB30-4432-A042-A41ACA1B274D")]
[PlugIn("NPC open merchant chat command", "Opens the merchant NPC store.")]
[ChatCommandHelp(Command, "Opens the NPC store.", null)]
public class NpcChatCommandPlugIn : ChatCommandPlugInBase<NpcChatCommandPlugIn.Arguments>, ISupportCustomConfiguration<NpcChatCommandPlugIn.NpcChatCommandConfiguration>, ISupportDefaultCustomConfiguration, IDisabledByDefault
{
    private const string Command = "/npc";
    private const CharacterStatus MinimumStatus = CharacterStatus.Normal;
    private const string InvalidNpcIdMessage = "Invalid NPC ID \"{0}\". Please provide a valid merchant NPC ID.";
    private const string InvalidMerchantMessage = "Not a valid merchant NPC.";

    private readonly TalkNpcAction _talkNpcAction = new();

    /// <summary>
    /// Gets or sets the configuration.
    /// </summary>
    public NpcChatCommandConfiguration? Configuration { get; set; }

    /// <inheritdoc />
    public override string Key => Command;

    /// <inheritdoc />
    public override CharacterStatus MinCharacterStatusRequirement => MinimumStatus;

    /// <inheritdoc />
    public object CreateDefaultConfig() => new NpcChatCommandConfiguration();

    /// <inheritdoc />
    protected override async ValueTask DoHandleCommandAsync(Player player, Arguments arguments)
    {
        if (player.CurrentMap is not { } currentMap)
        {
            return;
        }

        var configuration = this.Configuration ??= (NpcChatCommandConfiguration)this.CreateDefaultConfig();
        var npcDefinition = configuration.OpenMerchantNpc;

        if (player.SelectedCharacter?.CharacterStatus >= CharacterStatus.GameMaster && arguments?.NpcId is { } npcIdStr)
        {
            if (int.TryParse(npcIdStr, out var npcId) && player.GameContext.Configuration.Monsters.FirstOrDefault(m => m.Number == npcId) is { } definition)
            {
                npcDefinition = definition;
            }
            else
            {
                await this.ShowMessageToAsync(player, string.Format(CultureInfo.InvariantCulture, InvalidNpcIdMessage, npcIdStr)).ConfigureAwait(false);
                return;
            }
        }

        if (npcDefinition is null)
        {
            return;
        }

        if (npcDefinition!.MerchantStore is null)
        {
            if (player.SelectedCharacter?.CharacterStatus >= CharacterStatus.GameMaster)
            {
                await this.ShowMessageToAsync(player, InvalidMerchantMessage).ConfigureAwait(false);
            }

            return;
        }

        if (configuration.MinimumVipLevel > 0 && (player.Attributes?[Stats.IsVip] ?? 0) < configuration.MinimumVipLevel)
        {
            await this.ShowMessageToAsync(player, configuration.InsufficientVipLevelMessage).ConfigureAwait(false);
            return;
        }

        var npc = new NonPlayerCharacter(new MonsterSpawnArea { MonsterDefinition = npcDefinition }, npcDefinition, currentMap);
        await this._talkNpcAction.TalkToNpcAsync(player, npc).ConfigureAwait(false);
    }

    /// <summary>
    /// Arguments for the NPC chat command.
    /// </summary>
    public class Arguments : ArgumentsBase
    {
        /// <summary>
        /// Gets or sets the NPC ID to open the merchant store for (GM only).
        /// </summary>
        public string? NpcId { get; set; }
    }

    /// <summary>
    /// The configuration of a <see cref="NpcChatCommandPlugIn"/>.
    /// </summary>
    public class NpcChatCommandConfiguration
    {
        /// <summary>
        /// Gets or sets the NPC ID of the NPC to open the merchant store.
        /// </summary>
        // TODO: Change to a list of possible NPCs merchants
        [Display(Name = "NPC ID", Description = @"The ID of the NPC to open the merchant store. Default: Potion Girl Amy - 253.")]
        public MonsterDefinition? OpenMerchantNpc { get; set; }

        /// <summary>
        /// Gets or sets the minimum VIP level to use the command.
        /// </summary>
        [Display(Name = "Minimum VIP Level", Description = @"The minimum VIP level to use the command. Default: 0.")]
        public int MinimumVipLevel { get; set; }

        /// <summary>
        /// Gets or sets the message to show when the player does not have the required VIP level for this command (excluding GM).
        /// </summary>
        [Display(Name = "Insufficient VIP Level Message", Description = @"The message to show when the player does not have the required VIP level for this command (excluding GM).")]
        public string InsufficientVipLevelMessage { get; set; } = "Insufficient VIP level to use this command";
    }
}
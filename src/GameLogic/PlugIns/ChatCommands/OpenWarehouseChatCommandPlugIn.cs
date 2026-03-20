// <copyright file="OpenWarehouseChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which opens the warehouse NPC window.
/// </summary>
[Guid("62027B6B-D8E7-4DDB-A16B-7070D1BC4A56")]
[PlugIn]
[Display(Name = nameof(PlugInResources.OpenWarehouseChatCommandPlugIn_Name), Description = nameof(PlugInResources.OpenWarehouseChatCommandPlugIn_Description), ResourceType = typeof(PlugInResources))]
[ChatCommandHelp(Command, "Opens the warehouse.", null)]
public class OpenWarehouseChatCommandPlugIn : ChatCommandPlugInBase<OpenWarehouseChatCommandPlugIn.Arguments>, ISupportCustomConfiguration<OpenWarehouseChatCommandPlugIn.OpenWarehouseChatCommandConfiguration>, ISupportDefaultCustomConfiguration, IDisabledByDefault
{
    private const string Command = "/openware";
    private const CharacterStatus MinimumStatus = CharacterStatus.Normal;

    private readonly TalkNpcAction _talkNpcAction = new();

    /// <summary>
    /// Gets or sets the configuration.
    /// </summary>
    public OpenWarehouseChatCommandConfiguration? Configuration { get; set; }

    /// <inheritdoc />
    public override string Key => Command;

    /// <inheritdoc />
    public override CharacterStatus MinCharacterStatusRequirement => MinimumStatus;

    /// <inheritdoc />
    public object CreateDefaultConfig() => new OpenWarehouseChatCommandConfiguration();

    /// <inheritdoc />
    protected override async ValueTask DoHandleCommandAsync(Player player, Arguments arguments)
    {
        if (player.CurrentMap is not { } currentMap)
        {
            return;
        }

        var configuration = this.Configuration ??= (OpenWarehouseChatCommandConfiguration)this.CreateDefaultConfig();

        if (configuration.MinimumVipLevel > 0 && (player.Attributes?[Stats.IsVip] ?? 0) < configuration.MinimumVipLevel)
        {
            if (configuration.InsufficientVipLevelMessage.GetTranslation(player.Culture) is { Length: > 0 } message)
            {
                await player.ShowBlueMessageAsync(message).ConfigureAwait(false);
            }

            return;
        }

        if (player.GameContext.Configuration.Monsters.FirstOrDefault(m => m.NpcWindow == NpcWindow.VaultStorage) is not { } definition)
        {
            if (player.SelectedCharacter?.CharacterStatus >= CharacterStatus.GameMaster)
            {
                await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.NoWarehouseNpcFound)).ConfigureAwait(false);
            }

            return;
        }

        var npc = new NonPlayerCharacter(new MonsterSpawnArea { MonsterDefinition = definition }, definition, currentMap);
        await this._talkNpcAction.TalkToNpcAsync(player, npc).ConfigureAwait(false);
    }

    /// <summary>
    /// Arguments for the Open Warehouse chat command.
    /// </summary>
    public class Arguments : ArgumentsBase
    {
    }

    /// <summary>
    /// The configuration of a <see cref="OpenWarehouseChatCommandPlugIn"/>.
    /// </summary>
    public class OpenWarehouseChatCommandConfiguration
    {
        /// <summary>
        /// Gets or sets the minimum VIP level to use the command (excluding GM).
        /// </summary>
        [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.OpenWarehouseChatCommandConfiguration_MinimumVipLevel_Name), Description = nameof(PlugInResources.OpenWarehouseChatCommandConfiguration_MinimumVipLevel_Description))]
        public int MinimumVipLevel { get; set; }

        /// <summary>
        /// Gets or sets the message to show when the player does not have the required VIP level for this command (excluding GM).
        /// </summary>
        [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.OpenWarehouseChatCommandConfiguration_InsufficientVipLevelMessage_Name), Description = nameof(PlugInResources.OpenWarehouseChatCommandConfiguration_InsufficientVipLevelMessage_Description))]
        public LocalizedString InsufficientVipLevelMessage { get; set; } = "Insufficient VIP level to use this command";
    }
}
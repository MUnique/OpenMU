// <copyright file="WaitingItemsChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.WaitingItems;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command which lists the items which wait for space in the inventory.
/// </summary>
[Guid("75B3625C-6E8E-4FFF-A0A4-3FA9C3895C70")]
[PlugIn]
[Display(Name = nameof(PlugInResources.WaitingItemsChatCommandPlugIn_Name), Description = nameof(PlugInResources.WaitingItemsChatCommandPlugIn_Description), ResourceType = typeof(PlugInResources))]
[ChatCommandHelp(Command, typeof(WaitingItemsChatCommandPlugIn.Arguments), CharacterStatus.Normal)]
public class WaitingItemsChatCommandPlugIn : ChatCommandPlugInBase<WaitingItemsChatCommandPlugIn.Arguments>, ISupportCustomConfiguration<WaitingItemsChatCommandPlugIn.WaitingItemsCommandConfiguration>, ISupportDefaultCustomConfiguration
{
    /// <summary>
    /// The command.
    /// </summary>
    internal const string Command = "/waiting";

    /// <inheritdoc />
    public override string Key => Command;

    /// <inheritdoc />
    public override CharacterStatus MinCharacterStatusRequirement => CharacterStatus.Normal;

    /// <inheritdoc />
    public WaitingItemsCommandConfiguration? Configuration { get; set; }

    /// <inheritdoc />
    public object CreateDefaultConfig() => new WaitingItemsCommandConfiguration();

    /// <inheritdoc />
    protected override async ValueTask DoHandleCommandAsync(Player player, Arguments arguments)
    {
        var itemsNamed = this.Configuration?.ItemsNamed ?? new WaitingItemsCommandConfiguration().ItemsNamed;
        await player.ShowWaitingItemsAsync(itemsNamed, true).ConfigureAwait(false);
    }

    /// <summary>
    /// The configuration of the command.
    /// </summary>
    public class WaitingItemsCommandConfiguration
    {
        /// <summary>
        /// Gets or sets the number of waiting items which are named.
        /// </summary>
        [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.WaitingItemsCommandConfiguration_ItemsNamed_Name), Description = nameof(PlugInResources.WaitingItemsCommandConfiguration_ItemsNamed_Description))]
        public int ItemsNamed { get; set; } = 10;
    }

    /// <summary>
    /// The arguments of the command; it has none.
    /// </summary>
    public class Arguments : ArgumentsBase
    {
    }
}

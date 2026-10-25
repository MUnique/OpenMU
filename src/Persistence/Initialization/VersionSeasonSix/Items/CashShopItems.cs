// <copyright file="CashShopItems.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.Initialization.Properties;

/// <summary>
/// Initializer for the items of the cash shop which aren't put into the inventory. A delivery
/// plugin applies them when a player uses them from the cash shop storage, e.g. the
/// <see cref="GameLogic.CashShop.VaultExtensionDeliveryPlugIn"/>.
/// </summary>
/// <remarks>
/// Existing items aren't changed, so it can be used by an update, too.
/// </remarks>
internal class CashShopItems : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CashShopItems"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public CashShopItems(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    public override void Initialize()
    {
        this.CreateItem(91, LocalizedString.FromResource(() => ItemNames.SummonerCharacterCard));
        this.CreateItem(162, LocalizedString.FromResource(() => ItemNames.MagicBackpack));
        this.CreateItem(163, LocalizedString.FromResource(() => ItemNames.VaultExpansionCertificate));
        this.CreateItem(169, LocalizedString.FromResource(() => ItemNames.RageFighterCharacterCard));
    }

    private void CreateItem(byte number, LocalizedString name)
    {
        const byte group = 14;
        if (this.GameConfiguration.Items.Any(item => item.Group == group && item.Number == number))
        {
            return;
        }

        var item = this.Context.CreateNew<ItemDefinition>();
        item.Group = group;
        item.Number = number;
        item.Name = name;
        item.Width = 1;
        item.Height = 1;
        item.Durability = 1;
        item.SetGuid(item.Group, item.Number);
        this.GameConfiguration.Items.Add(item);
    }
}

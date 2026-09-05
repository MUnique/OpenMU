// <copyright file="CastleSiegeLifeStoneConsumeHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.CastleSiege;
using MUnique.OpenMU.GameLogic.CastleSiege.Actions;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Consumes a Life Stone only after it was successfully placed during Castle Siege.
/// </summary>
[Guid("DBF4B0D3-1A6B-4A7B-8D2E-A4440F0E7B79")]
[PlugIn]
[Display(Name = nameof(PlugInResources.CastleSiegeLifeStoneConsumeHandlerPlugIn_Name), Description = nameof(PlugInResources.CastleSiegeLifeStoneConsumeHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
public sealed class CastleSiegeLifeStoneConsumeHandlerPlugIn : BaseConsumeHandlerPlugIn
{
    private readonly Func<Player, CastleSiegeContext?> _contextResolver;

    /// <summary>
    /// Initializes a new instance of the <see cref="CastleSiegeLifeStoneConsumeHandlerPlugIn"/> class.
    /// </summary>
    public CastleSiegeLifeStoneConsumeHandlerPlugIn()
        : this(CastleSiegeContextResolver.GetContext)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CastleSiegeLifeStoneConsumeHandlerPlugIn"/> class.
    /// </summary>
    /// <param name="contextResolver">The Castle Siege context resolver.</param>
    internal CastleSiegeLifeStoneConsumeHandlerPlugIn(Func<Player, CastleSiegeContext?> contextResolver)
    {
        this._contextResolver = contextResolver;
    }

    /// <inheritdoc />
    public override ItemIdentifier Key => ItemConstants.CastleSiegeLifeStone;

    /// <inheritdoc />
    public override async ValueTask<bool> ConsumeItemAsync(Player player, Item item, Item? targetItem, FruitUsage fruitUsage)
    {
        if (!this.CheckPreconditions(player, item)
            || !await CastleSiegeSummonLifeStoneAction
                .SummonAsync(player, this._contextResolver.Invoke(player))
                .ConfigureAwait(false))
        {
            return false;
        }

        await this.ConsumeSourceItemAsync(player, item).ConfigureAwait(false);
        return true;
    }
}

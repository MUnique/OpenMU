// <copyright file="CastleSiegeGuardsmanTalkPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views.CastleSiege;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Opens the Land of Trials entry dialog when a player talks to a Castle Siege guardsman.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.CastleSiegeGuardsmanTalkPlugIn_Name), Description = nameof(PlugInResources.CastleSiegeGuardsmanTalkPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("3E2AD5FD-E5D0-4464-91EE-70DF686BBB6A")]
public sealed class CastleSiegeGuardsmanTalkPlugIn : IPlayerTalkToNpcPlugIn
{
    private const short GuardsmanNumber = 224;
    private readonly CastleSiegeTaxProvider _taxProvider = new();

    /// <inheritdoc />
    public async ValueTask PlayerTalksToNpcAsync(Player player, NonPlayerCharacter npc, NpcTalkEventArgs eventArgs)
    {
        var context = CastleSiegeTaxProvider.GetContext(player);
        if (npc.Definition.Number != GuardsmanNumber
            || context is not { Configuration.Enabled: true }
            || player.CurrentMap?.Definition.Number != context.Configuration.CastleSiegeMapDefinition?.Number)
        {
            return;
        }

        eventArgs.HasBeenHandled = true;
        eventArgs.LeavesDialogOpen = true;
        var isOwnerMaster = await this._taxProvider.IsOwnerGuildMasterAsync(player, context).ConfigureAwait(false);
        var isExempt = isOwnerMaster
                       || await this._taxProvider.IsExemptAsync(player, context).ConfigureAwait(false);
        var accessType = !context.SiegeData.IsOccupied
            ? CastleSiegeHuntZoneAccessType.Failed
            : isOwnerMaster
                ? CastleSiegeHuntZoneAccessType.OwnerGuildMaster
                : isExempt
                    ? CastleSiegeHuntZoneAccessType.OwnerAllianceMember
                    : CastleSiegeHuntZoneAccessType.Guest;
        var configuredFee = Math.Clamp(context.SiegeData.TaxHunt, 0, CastleSiegeTaxProvider.MaximumHuntTax);
        var fee = accessType is CastleSiegeHuntZoneAccessType.Guest
            or CastleSiegeHuntZoneAccessType.OwnerGuildMaster
            ? configuredFee
            : 0;
        await player.InvokeViewPlugInAsync<ICastleSiegeHuntZoneGuardInfoPlugIn>(
                view => view.ShowHuntZoneGuardInfoAsync(
                    accessType,
                    context.SiegeData.IsHuntZoneEnabled,
                    fee,
                    CastleSiegeTaxProvider.MaximumHuntTax,
                    CastleSiegeTaxProvider.HuntTaxStep))
            .ConfigureAwait(false);
    }
}

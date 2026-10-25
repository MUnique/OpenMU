// <copyright file="CastleSiegeRegistrationGuardsmanTalkPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views.NPC;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Opens the Castle Siege registration window when a player talks to a guardsman at the bridge of the Castle Siege map.
/// </summary>
/// <remarks>
/// The window shows the Castle Siege status, which the client requests when it opens, and offers the guild registration.
/// Guardsmen on other maps, e.g. in Crywolf Fortress, aren't handled by this plug-in.
/// </remarks>
[PlugIn]
[Display(Name = nameof(PlugInResources.CastleSiegeRegistrationGuardsmanTalkPlugIn_Name), Description = nameof(PlugInResources.CastleSiegeRegistrationGuardsmanTalkPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("FD52C99B-4D0B-4547-B4B4-7DE66FA79819")]
public sealed class CastleSiegeRegistrationGuardsmanTalkPlugIn : NpcTalkPlugInBase
{
    /// <summary>
    /// The number of the guardsman NPC.
    /// </summary>
    internal const short GuardsmanNumber = 224;

    private readonly Func<Player, CastleSiegeContext?> _contextResolver;

    /// <summary>
    /// Initializes a new instance of the <see cref="CastleSiegeRegistrationGuardsmanTalkPlugIn"/> class.
    /// </summary>
    public CastleSiegeRegistrationGuardsmanTalkPlugIn()
        : this(CastleSiegeContextResolver.GetContext)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CastleSiegeRegistrationGuardsmanTalkPlugIn"/> class.
    /// </summary>
    /// <param name="contextResolver">The Castle Siege context resolver.</param>
    internal CastleSiegeRegistrationGuardsmanTalkPlugIn(Func<Player, CastleSiegeContext?> contextResolver)
    {
        this._contextResolver = contextResolver;
    }

    /// <inheritdoc />
    public override short DefaultNpcNumber => GuardsmanNumber;

    /// <inheritdoc />
    protected override async ValueTask PlayerTalksToConfiguredNpcAsync(Player player, NonPlayerCharacter npc, NpcTalkEventArgs eventArgs)
    {
        var context = this._contextResolver(player);
        if (context is not { Configuration.Enabled: true }
            || player.CurrentMap?.Definition.Number != context.Configuration.CastleSiegeMapDefinition?.Number)
        {
            return;
        }

        eventArgs.HasBeenHandled = true;
        eventArgs.LeavesDialogOpen = true;
        await player.InvokeViewPlugInAsync<IOpenNpcWindowPlugIn>(view => view.OpenNpcWindowAsync(NpcWindow.CastleSiegeGuardsman)).ConfigureAwait(false);
    }
}

// <copyright file="MapHitNotificationPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Map.Map;

using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A plugin which forwards the hits of a game server to the live map views which show the map on which the hit happened.
/// </summary>
/// <remarks>
/// The live map views are no world observers of the hit, so they can't be informed by a view plugin.
/// Instead, this plugin listens at the <see cref="IAttackableGotHitPlugIn"/> plugin point, which provides the
/// attacker, too. It's registered by <see cref="EnsureRegistered"/> when the live map of a game server is
/// set up, and not discovered by itself. The live map views subscribe with <see cref="Subscribe"/> as long as they are shown.
/// </remarks>
[Guid("4E750DF4-28FB-4A1E-AEF1-1A5D05BD40E5")]
[Display(Name = "Live map hit forwarding", Description = "Forwards the hits of the game server to the live maps of the admin panel, which show the damage of the selected object. Without it, the live maps don't show any damage.")]
public class MapHitNotificationPlugIn : IAttackableGotHitPlugIn
{
    private static readonly object RegistrationLock = new();

    private static readonly ConcurrentDictionary<Subscription, byte> Subscriptions = new();

    /// <summary>
    /// Registers this plugin at the plugin point of the specified plugin manager, if that didn't happen yet.
    /// </summary>
    /// <param name="plugInManager">The plugin manager of a game server.</param>
    public static void EnsureRegistered(PlugInManager? plugInManager)
    {
        if (plugInManager is null)
        {
            return;
        }

        // Each registration adds another instance to the plugin point, which would forward every hit again.
        lock (RegistrationLock)
        {
            if (plugInManager.KnownPlugInTypes.Contains(typeof(MapHitNotificationPlugIn)))
            {
                return;
            }

            plugInManager.RegisterPlugIn<IAttackableGotHitPlugIn, MapHitNotificationPlugIn>();
        }
    }

    /// <summary>
    /// Subscribes to the hits which happen on the specified map.
    /// </summary>
    /// <param name="mapId">The unique identifier of the map instance.</param>
    /// <param name="handler">
    /// The handler which is called by the game logic for each hit on the map.
    /// It must not block and must not throw.
    /// </param>
    /// <returns>The subscription. Dispose it to unsubscribe.</returns>
    public static IDisposable Subscribe(Guid mapId, Action<IAttackable, IAttacker, HitInfo> handler)
    {
        var subscription = new Subscription(mapId, handler);
        Subscriptions.TryAdd(subscription, 0);
        return subscription;
    }

    /// <inheritdoc />
    public void AttackableGotHit(IAttackable attackable, IAttacker attacker, HitInfo hitInfo)
    {
        if (Subscriptions.IsEmpty || attackable.CurrentMap?.Id is not { } mapId)
        {
            return;
        }

        foreach (var (subscription, _) in Subscriptions)
        {
            if (subscription.MapId == mapId)
            {
                subscription.Handler(attackable, attacker, hitInfo);
            }
        }
    }

    private sealed class Subscription : IDisposable
    {
        public Subscription(Guid mapId, Action<IAttackable, IAttacker, HitInfo> handler)
        {
            this.MapId = mapId;
            this.Handler = handler;
        }

        public Guid MapId { get; }

        public Action<IAttackable, IAttacker, HitInfo> Handler { get; }

        public void Dispose()
        {
            Subscriptions.TryRemove(this, out _);
        }
    }
}

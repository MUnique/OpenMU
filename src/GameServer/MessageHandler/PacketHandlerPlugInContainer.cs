// <copyright file="PacketHandlerPlugInContainer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler;

using System.Collections.Concurrent;
using System.ComponentModel.Design;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameServer.RemoteView;
using MUnique.OpenMU.Network.Packets;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A plugin container which provides the effective packet handler plugins for the specified version.
/// Base class for different kind of packet handler interfaces.
/// </summary>
/// <typeparam name="THandler">The type of the handler interface.</typeparam>
public class PacketHandlerPlugInContainer<THandler> : StrategyPlugInProvider<byte, THandler>
    where THandler : class, IPacketHandlerPlugInBase
{
    /// <summary>
    /// Since packet handler plugins are not holding any player state, they can be kept as singleton instances to save some memory.
    /// </summary>
    private static readonly ConcurrentDictionary<Type, THandler> HandlerCache = new();

    private readonly IClientVersionProvider _clientVersionProvider;

    /// <summary>
    /// The services for the creation of the handlers. It's never disposed: it only holds services which this
    /// container doesn't own (the player, the plugin manager and the logger factory), and disposing a
    /// <see cref="ServiceContainer"/> disposes its services.
    /// </summary>
    private readonly ServiceContainer _serviceContainer;

    /// <summary>
    /// Initializes a new instance of the <see cref="PacketHandlerPlugInContainer{THandler}" /> class.
    /// </summary>
    /// <param name="clientVersionProvider">The client version provider.</param>
    /// <param name="manager">The manager.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    public PacketHandlerPlugInContainer(IClientVersionProvider clientVersionProvider, PlugInManager manager, ILoggerFactory loggerFactory)
        : base(manager, loggerFactory)
    {
        this._clientVersionProvider = clientVersionProvider;
        this._clientVersionProvider.ClientVersionChanged += this.OnClientVersionChanged;
        this._serviceContainer = new ServiceContainer();
        this._serviceContainer.AddService(typeof(IClientVersionProvider), clientVersionProvider);
        this._serviceContainer.AddService(typeof(PlugInManager), manager);
        this._serviceContainer.AddService(typeof(ILoggerFactory), loggerFactory);
    }

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    public void Initialize()
    {
        foreach (var plugInType in this.Manager.GetKnownPlugInsOf<THandler>().Where(this.Manager.IsPlugInActive))
        {
            this.BeforeActivatePlugInType(plugInType);
        }
    }

    /// <summary>
    /// Handles the incoming data packet by selecting the correct plugin.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="packet">The packet.</param>
    public async ValueTask HandlePacketAsync(RemotePlayer player, Memory<byte> packet)
    {
        var typeIndex = packet.Span[0] % 2 == 1 ? 2 : 3;
        var packetType = packet.Span[typeIndex];
        var handler = this[packetType];
        await this.HandlePacketAsync(player, packet, handler).ConfigureAwait(false);
    }

    /// <inheritdoc />
    /// <remarks>
    /// Also disposes the handlers which were created for this container alone, e.g. the
    /// <see cref="GroupPacketHandlerPlugIn"/>s of a player, which are containers themselves.
    /// The shared handlers of the <see cref="HandlerCache"/> stay untouched.
    /// </remarks>
    protected override void Dispose(bool disposing)
    {
        if (this.IsDisposed)
        {
            return;
        }

        // Unsubscribe from the plugin manager first, so that no handler gets created while the owned ones are disposed.
        base.Dispose(disposing);
        if (disposing)
        {
            this._clientVersionProvider.ClientVersionChanged -= this.OnClientVersionChanged;
            foreach (var ownedHandler in this.KnownPlugIns.Where(IsOwnedHandler).OfType<IDisposable>())
            {
                ownedHandler.Dispose();
            }
        }
    }

    /// <summary>
    /// Handles the incoming data packet by using the specified handler plugin.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="packet">The packet.</param>
    /// <param name="handler">The handler.</param>
    protected async ValueTask HandlePacketAsync(Player player, Memory<byte> packet, THandler? handler)
    {
        if (handler is null)
        {
            // unknown packet
            return;
        }

        using var loggingScope = this.Logger.BeginScope(("EventId", handler.GetType().Name));
        if (handler.IsEncryptionExpected && (packet.Span[0] < 0xC3))
        {
            this.Logger.LogWarning($"Packet was not encrypted and will not be handled: {packet.Span.AsString()}");
            return;
        }

        await handler.HandlePacketAsync(player, packet).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    protected override void BeforeActivatePlugInType(Type plugInType)
    {
        base.BeforeActivatePlugInType(plugInType);

        var knownPlugIn = this.FindKnownPlugin(plugInType);
        if (knownPlugIn is null && this._clientVersionProvider.ClientVersion.IsPlugInSuitable(plugInType))
        {
            var plugIn = this.CreatePlugIn(plugInType);
            this.AddPlugIn(plugIn, true);
        }
    }

    /// <inheritdoc />
    protected override void ActivatePlugIn(THandler plugIn)
    {
        var newPlugInIsEffective = false;
        if (this.TryGetPlugIn(plugIn.Key, out var currentlyActivePlugIn))
        {
            if (currentlyActivePlugIn == plugIn)
            {
                return;
            }

            if (currentlyActivePlugIn.IsPreferedTo(plugIn))
            {
                return;
            }

            newPlugInIsEffective = true;
            this.DeactivatePlugIn(currentlyActivePlugIn);
        }

        base.ActivatePlugIn(plugIn);
        if (newPlugInIsEffective)
        {
            this.SetEffectivePlugin(plugIn);
        }
    }

    /// <inheritdoc />
    protected override void AfterDeactivatePlugInType(THandler deactivatedPlugIn)
    {
        base.AfterDeactivatePlugInType(deactivatedPlugIn);

        if (!this.TryGetPlugIn(deactivatedPlugIn.Key, out _))
        {
            // the plugin was effective before, so we need to set a replacement active.
            var replacement = this.KnownPlugIns
                .Where(p => p.Key == deactivatedPlugIn.Key && p != deactivatedPlugIn)
                .OrderByDescending(p => p.GetType().GetCustomAttribute(typeof(MinimumClientAttribute), inherit: false))
                .FirstOrDefault();
            if (replacement != null)
            {
                this.SetEffectivePlugin(replacement);
            }
        }
    }

    /// <summary>
    /// Determines whether the handler was created for this container alone, so that its lifetime ends with this container.
    /// </summary>
    /// <param name="handler">The handler.</param>
    /// <returns><c>true</c>, if the handler isn't a shared instance of the <see cref="HandlerCache"/>.</returns>
    private static bool IsOwnedHandler(THandler handler)
    {
        return !(HandlerCache.TryGetValue(handler.GetType(), out var sharedHandler) && ReferenceEquals(sharedHandler, handler));
    }

    private THandler CreatePlugIn(Type plugInType)
    {
        if (!HandlerCache.TryGetValue(plugInType, out var plugIn))
        {
            plugIn = (THandler)ActivatorUtilities.CreateInstance(this._serviceContainer, plugInType);
            if (plugInType.GetConstructors().Any(c => c.GetParameters().All(p => p.ParameterType != typeof(IClientVersionProvider))))
            {
                HandlerCache.TryAdd(plugInType, plugIn);
            }

            if (plugIn is GroupPacketHandlerPlugIn groupPacketHandler)
            {
                groupPacketHandler.Initialize();
            }
        }

        return plugIn;
    }

    private void OnClientVersionChanged(object? sender, EventArgs e)
    {
        if (this.IsDisposed)
        {
            // A group handler which got disposed by its parent container during the same event invocation.
            return;
        }

        this.Logger.LogWarning("Client version changed");

        foreach (var knownPlugIn in this.KnownPlugIns)
        {
            if (!this._clientVersionProvider.ClientVersion.IsPlugInSuitable(knownPlugIn.GetType()))
            {
                this.DeactivatePlugIn(knownPlugIn);
                this.RemovePlugIn(knownPlugIn);
                if (IsOwnedHandler(knownPlugIn) && knownPlugIn is IDisposable ownedHandler)
                {
                    ownedHandler.Dispose();
                }
            }
        }

        this.Initialize();
    }
}
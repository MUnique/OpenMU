// <copyright file="ViewPlugInContainer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView;

using System.ComponentModel.Design;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A plugin container which selects plugin based on the provided version/season metadata.
/// </summary>
/// <seealso cref="IViewPlugIn" />
/// <remarks>
/// Simplified example: View plugin container is meant for season 6.
/// There are some IChatMessageViewPlugIns available for the seasons 1, 2, 6 and 7.
/// Which one to choose?
///   In this case, it's easy, we just select the one which is equal.
/// Now, if we remove the plugin for season 6, so that 1, 2 and 7 are available?
///   In this case, we assume that the last available season before the target season is the correct one, season 2.
/// </remarks>
internal sealed class ViewPlugInContainer : CustomPlugInContainerBase<IViewPlugIn>, IDisposable
{
    private readonly RemotePlayer _player;

    private readonly ServiceContainer _serviceContainer;

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewPlugInContainer"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="clientVersion">The client information.</param>
    /// <param name="manager">The manager.</param>
    public ViewPlugInContainer(RemotePlayer player, ClientVersion clientVersion, PlugInManager manager)
        : base(manager)
    {
        this._player = player;
        this.Client = clientVersion;
        this._player.ClientVersionChanged += this.OnClientVersionChanged;
        this._serviceContainer = new ServiceContainer();
        this._serviceContainer.AddService(typeof(RemotePlayer), this._player);
        this.Initialize();
    }

    /// <summary>
    /// Gets the client.
    /// </summary>
    /// <value>
    /// The client.
    /// </value>
    public ClientVersion Client { get; private set; }

    /// <inheritdoc />
    public void Dispose()
    {
        this._serviceContainer.Dispose();
        this.KnownPlugIns.OfType<IDisposable>().ForEach(d => d.Dispose());
    }

    /// <inheritdoc/>
    /// <remarks>We look if the activated plugin is rated at a higher client version than the current one.</remarks>
    protected override bool IsNewPlugInReplacingOld(IViewPlugIn currentEffectivePlugIn, IViewPlugIn activatedPlugIn)
    {
        var currentPlugInClient = currentEffectivePlugIn.GetType().GetCustomAttribute<MinimumClientAttribute>(inherit: false)?.Client ?? default;
        var activatedPluginClient = activatedPlugIn.GetType().GetCustomAttribute<MinimumClientAttribute>(inherit: false)?.Client ?? default;
        return currentPlugInClient.CompareTo(activatedPluginClient) < 0;
    }

    /// <inheritdoc />
    /// <remarks>We sort by version and choose the highest one.</remarks>
    protected override IViewPlugIn? DetermineEffectivePlugIn(Type interfaceType)
    {
        return this.ActivePlugIns.OrderByDescending(p => p.GetType().GetCustomAttribute(typeof(MinimumClientAttribute), inherit: false)).FirstOrDefault(interfaceType.IsInstanceOfType);
    }

    /// <inheritdoc />
    /// <remarks>We just take the plugins which have a equal or lower version than our target <see cref="Client"/>.</remarks>
    protected override void CreatePlugInIfSuitable(Type plugInType)
    {
        if (this.Client.IsPlugInSuitable(plugInType))
        {
            var plugIn = (IViewPlugIn)ActivatorUtilities.CreateInstance(this._serviceContainer, plugInType)!;
            this.AddPlugIn(plugIn, true);
        }
    }

    private void OnClientVersionChanged(object? sender, EventArgs e)
    {
        this.Client = this._player.ClientVersion;
        foreach (var activePlugIn in this.ActivePlugIns)
        {
            if (!this.Client.IsPlugInSuitable(activePlugIn.GetType()))
            {
                this.DeactivatePlugIn(activePlugIn);
            }
        }

        this.Initialize();
    }
}
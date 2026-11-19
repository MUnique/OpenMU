// <copyright file="GameEventPlugInListener.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.GameEvents;

using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Passes the published <see cref="GameEvent"/>s to the active <see cref="IGameEventPublishedPlugIn"/>s.
/// </summary>
public sealed class GameEventPlugInListener : IGameEventListener
{
    private readonly PlugInManager _plugInManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameEventPlugInListener"/> class.
    /// </summary>
    /// <param name="plugInManager">The plugin manager.</param>
    public GameEventPlugInListener(PlugInManager plugInManager)
    {
        this._plugInManager = plugInManager;
    }

    /// <inheritdoc />
    public ValueTask OnGameEventAsync(GameEvent gameEvent)
    {
        return this._plugInManager.GetPlugInPoint<IGameEventPublishedPlugIn>()?.GameEventPublishedAsync(gameEvent) ?? ValueTask.CompletedTask;
    }
}

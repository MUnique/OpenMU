// <copyright file="GameEventController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord.Host;

using global::Dapr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Receives the <see cref="GameEvent"/>s which the game servers publish, and passes them to the <see cref="IGameEventListener"/>s.
/// </summary>
[ApiController]
[Route("events")]
public class GameEventController : ControllerBase
{
    private readonly IEnumerable<IGameEventListener> _listeners;
    private readonly ILogger<GameEventController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameEventController"/> class.
    /// </summary>
    /// <param name="listeners">The listeners.</param>
    /// <param name="logger">The logger.</param>
    public GameEventController(IEnumerable<IGameEventListener> listeners, ILogger<GameEventController> logger)
    {
        this._listeners = listeners;
        this._logger = logger;
    }

    /// <summary>
    /// Passes the game event to the listeners.
    /// </summary>
    /// <param name="gameEvent">The game event.</param>
    /// <returns>The task.</returns>
    [Topic("pubsub", nameof(IEventPublisher.GameEventAsync))]
    [HttpPost(nameof(IEventPublisher.GameEventAsync))]
    public async Task GameEventAsync([FromBody] GameEvent gameEvent)
    {
        foreach (var listener in this._listeners)
        {
            try
            {
                await listener.OnGameEventAsync(gameEvent).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error when passing the game event {gameEvent} to {listener}.", gameEvent, listener);
            }
        }
    }
}

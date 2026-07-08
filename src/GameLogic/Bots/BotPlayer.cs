// <copyright file="BotPlayer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using MUnique.OpenMU.GameLogic.Offline;

/// <summary>
/// A connection-less bot player. It reuses the whole offline-player intelligence (combat, buffs,
/// healing, pickup) and adds a <see cref="BotNavigator"/> which makes it roam to level-appropriate
/// hunting grounds instead of standing on a fixed spawn position.
/// </summary>
public sealed class BotPlayer : OfflinePlayer
{
    private BotNavigator? _navigator;

    /// <summary>
    /// Initializes a new instance of the <see cref="BotPlayer"/> class.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    public BotPlayer(IGameContext gameContext)
        : base(gameContext)
    {
    }

    /// <inheritdoc />
    public override bool RespawnAndContinue => true;

    /// <inheritdoc />
    public override async ValueTask StopAsync()
    {
        await this.StopNavigatorAsync().ConfigureAwait(false);
        await base.StopAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override void StartIntelligence()
    {
        base.StartIntelligence();
        this._navigator = new BotNavigator(this);
        this._navigator.Start();
    }

    /// <inheritdoc />
    protected override async ValueTask DisposeAsyncCore()
    {
        await this.StopNavigatorAsync().ConfigureAwait(false);
        await base.DisposeAsyncCore().ConfigureAwait(false);
    }

    private async ValueTask StopNavigatorAsync()
    {
        if (this._navigator is { } navigator)
        {
            this._navigator = null;
            await navigator.DisposeAsync().ConfigureAwait(false);
        }
    }
}

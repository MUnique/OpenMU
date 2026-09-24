// <copyright file="RaklionPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Raklion;

using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The plugin of the raklion event, in which the players fight against Selupan in the hatchery.
/// </summary>
/// <remarks>
/// The event runs continuously: when the spider eggs of the hatchery are destroyed, Selupan appears.
/// After the battle, the hatchery is closed for some time until the spider eggs appear again.
/// </remarks>
[PlugIn]
[Display(Name = nameof(PlugInResources.RaklionPlugIn_Name), Description = nameof(PlugInResources.RaklionPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("C5A1E7D3-8B24-4F69-9E0A-6D3B2F7C4E18")]
public sealed class RaklionPlugIn : IFeaturePlugIn, IPeriodicTaskPlugIn, ISupportCustomConfiguration<RaklionEventDefinition>, ISupportDefaultCustomConfiguration, IDisposable
{
    private readonly ConcurrentDictionary<IGameContext, RaklionContext> _contexts = new();
    private readonly ConcurrentDictionary<IGameContext, int> _runningTicks = new();

    /// <inheritdoc />
    public RaklionEventDefinition? Configuration { get; set; }

    /// <summary>
    /// Gets the context of the raklion event of the game context, if it's running.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    /// <returns>The context of the raklion event.</returns>
    public static RaklionContext? GetContext(IGameContext gameContext)
    {
        var plugIn = gameContext.FeaturePlugIns.GetPlugIn<RaklionPlugIn>();
        return plugIn is not null && plugIn._contexts.TryGetValue(gameContext, out var context) ? context : null;
    }

    /// <inheritdoc />
    public async ValueTask ExecuteTaskAsync(GameContext gameContext)
    {
        if (this._runningTicks.GetOrAdd(gameContext, 0) != 0
            || !this._runningTicks.TryUpdate(gameContext, 1, 0))
        {
            return;
        }

        try
        {
            var definition = this.Configuration ??= new RaklionEventDefinition();
            if (!this._contexts.TryGetValue(gameContext, out var context))
            {
                context = new RaklionContext(gameContext, definition);
                await context.InitializeAsync().ConfigureAwait(false);
                this._contexts[gameContext] = context;
            }
            else
            {
                context.UpdateDefinition(definition);
            }

            await context.TickAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            gameContext.LoggerFactory.CreateLogger<RaklionPlugIn>().LogError(ex, "Unexpected error in the raklion event.");
        }
        finally
        {
            this._runningTicks[gameContext] = 0;
        }
    }

    /// <inheritdoc />
    public void ForceStart()
    {
        foreach (var context in this._contexts.Values)
        {
            context.SkipWaitingTime();
        }
    }

    /// <inheritdoc />
    public object CreateDefaultConfig()
    {
        return new RaklionEventDefinition();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        foreach (var context in this._contexts.Values)
        {
            context.Dispose();
        }

        this._contexts.Clear();
    }
}

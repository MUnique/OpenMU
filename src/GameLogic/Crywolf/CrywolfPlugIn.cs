// <copyright file="CrywolfPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Crywolf;

using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The plugin of the crywolf event, in which the players defend the crywolf fortress against the army of Balgass.
/// </summary>
/// <remarks>
/// The event starts at the configured times. The result of the event, the occupation state of the fortress,
/// is kept until the next event and saved in the database.
/// </remarks>
[PlugIn]
[Display(Name = nameof(PlugInResources.CrywolfPlugIn_Name), Description = nameof(PlugInResources.CrywolfPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("3E8B5C27-9D41-4A6F-B2C8-7F1D6E0A9B34")]
public sealed class CrywolfPlugIn : IFeaturePlugIn, IPeriodicTaskPlugIn, ISupportCustomConfiguration<CrywolfEventDefinition>, ISupportDefaultCustomConfiguration, IDisposable
{
    private readonly ConcurrentDictionary<IGameContext, CrywolfContext> _contexts = new();
    private readonly ConcurrentDictionary<IGameContext, int> _runningTicks = new();

    /// <inheritdoc />
    public CrywolfEventDefinition? Configuration { get; set; }

    /// <summary>
    /// Gets the context of the crywolf event of the game context, if it's running.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    /// <returns>The context of the crywolf event.</returns>
    public static CrywolfContext? GetContext(IGameContext gameContext)
    {
        var plugIn = gameContext.FeaturePlugIns.GetPlugIn<CrywolfPlugIn>();
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
            var definition = this.Configuration ??= new CrywolfEventDefinition();
            if (!this._contexts.TryGetValue(gameContext, out var context))
            {
                context = new CrywolfContext(gameContext, definition);
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
            gameContext.LoggerFactory.CreateLogger<CrywolfPlugIn>().LogError(ex, "Unexpected error in the crywolf event.");
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
        return new CrywolfEventDefinition();
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

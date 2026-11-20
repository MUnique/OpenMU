// <copyright file="GuildMasterInactivityPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Takes the guild master position from guild masters which haven't been seen in the game for the configured time.
/// </summary>
/// <remarks>
/// A guild whose master left the game for good can't invite or kick members anymore. The position goes to the most
/// senior active member, and a guild without any active member is dissolved. The plugin is deactivated by default,
/// because dissolving guilds is not reversible.
/// </remarks>
[PlugIn]
[Display(Name = nameof(PlugInResources.GuildMasterInactivityPlugIn_Name), Description = nameof(PlugInResources.GuildMasterInactivityPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("B3F5D1A8-6C24-4E79-9A0D-8F17C2E5B346")]
public class GuildMasterInactivityPlugIn : IPeriodicTaskPlugIn, ISupportCustomConfiguration<GuildMasterInactivityConfiguration>, ISupportDefaultCustomConfiguration, IDisabledByDefault
{
    /// <summary>
    /// The time after the start before the first check, so that the players can reconnect after a restart.
    /// </summary>
    private static readonly TimeSpan FirstCheckDelay = TimeSpan.FromMinutes(5);

    private DateTime _nextRunUtc = DateTime.UtcNow + FirstCheckDelay;

    /// <inheritdoc />
    public GuildMasterInactivityConfiguration? Configuration { get; set; }

    /// <inheritdoc />
    public async ValueTask ExecuteTaskAsync(GameContext gameContext)
    {
        var now = DateTime.UtcNow;
        if (now < this._nextRunUtc || gameContext is not IGameServerContext gameServerContext)
        {
            return;
        }

        var configuration = this.Configuration ??= new GuildMasterInactivityConfiguration();
        this._nextRunUtc = now + (configuration.CheckInterval > TimeSpan.Zero ? configuration.CheckInterval : TimeSpan.FromHours(1));
        if (configuration.InactiveDays <= 0)
        {
            return;
        }

        try
        {
            // The guild server logs what it does.
            await gameServerContext.GuildServer.RemoveInactiveGuildMastersAsync(TimeSpan.FromDays(configuration.InactiveDays), configuration.MaximumGuildsPerCheck).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            gameContext.LoggerFactory.CreateLogger<GuildMasterInactivityPlugIn>().LogError(ex, "Error when handling inactive guild masters.");
        }
    }

    /// <inheritdoc />
    public void ForceStart()
    {
        this._nextRunUtc = DateTime.UtcNow;
    }

    /// <inheritdoc />
    public object CreateDefaultConfig()
    {
        return new GuildMasterInactivityConfiguration();
    }
}

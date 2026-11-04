// <copyright file="ServerAnnouncementsPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Shows a configured list of announcements to all players of a game server, one after another
/// in a fixed interval, so that they don't have to be sent by a game master.
/// </summary>
/// <remarks>
/// Only one announcement is shown per interval, so they never overlap each other.
/// The rotation doesn't coordinate with other announcements, like the ones of the events.
/// </remarks>
[PlugIn]
[Display(Name = nameof(PlugInResources.ServerAnnouncementsPlugIn_Name), Description = nameof(PlugInResources.ServerAnnouncementsPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("2D6C9E4A-7B3E-4F8C-9E1D-8A4B6F0C2E7D")]
public class ServerAnnouncementsPlugIn : IPeriodicTaskPlugIn, ISupportCustomConfiguration<ServerAnnouncementsConfiguration>, ISupportDefaultCustomConfiguration, IDisabledByDefault
{
    /// <summary>
    /// The rotation state per game server. The plugin instance is shared between the
    /// game servers of a process, but each of them has its own rotation.
    /// </summary>
    private readonly ConditionalWeakTable<IGameContext, RotationState> _states = new();

    /// <summary>
    /// The number of calls of <see cref="ForceStart"/>. A rotation which didn't see the
    /// current number yet shows its next announcement without waiting for the interval.
    /// </summary>
    private int _forceStartCount;

    /// <inheritdoc />
    public ServerAnnouncementsConfiguration? Configuration { get; set; }

    /// <inheritdoc />
    public async ValueTask ExecuteTaskAsync(GameContext gameContext)
    {
        if (this.Configuration is not { Messages.Count: > 0 } configuration)
        {
            return;
        }

        var state = this._states.GetValue(gameContext, _ => new RotationState());
        if (state.TryGetNextAnnouncement(configuration, Volatile.Read(ref this._forceStartCount)) is not { } announcement)
        {
            return;
        }

        await gameContext.ForEachPlayerAsync(player =>
        {
            var text = announcement.Text.GetTranslation(player.Culture);
            if (string.IsNullOrWhiteSpace(text))
            {
                return Task.CompletedTask;
            }

            return player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync(text, announcement.Type)).AsTask();
        }).ConfigureAwait(false);
    }

    /// <inheritdoc />
    /// <remarks>
    /// Shows the next announcement of each game server without waiting for the interval.
    /// </remarks>
    public void ForceStart()
    {
        Interlocked.Increment(ref this._forceStartCount);
    }

    /// <inheritdoc />
    public object CreateDefaultConfig()
    {
        return new ServerAnnouncementsConfiguration
        {
            Messages =
            {
                new() { Text = "Our team will never ask you for your password." },
            },
        };
    }

    /// <summary>
    /// The state of the rotation of a game server.
    /// </summary>
    private sealed class RotationState
    {
        /// <summary>
        /// The minimum interval between two announcements. It protects the players from
        /// getting an announcement every second when the configured interval is zero.
        /// </summary>
        private static readonly TimeSpan MinimumInterval = TimeSpan.FromSeconds(10);

        private readonly Lock _lock = new();

        private int _nextIndex;

        private DateTime _nextRunUtc;

        private int _handledForceStartCount;

        /// <summary>
        /// Gets the announcement which is due, and advances the rotation.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="forceStartCount">The current number of forced starts.</param>
        /// <returns>The due announcement, or <c>null</c>, if the interval isn't over yet.</returns>
        public ServerAnnouncementsConfiguration.Announcement? TryGetNextAnnouncement(ServerAnnouncementsConfiguration configuration, int forceStartCount)
        {
            lock (this._lock)
            {
                var now = DateTime.UtcNow;
                if (now < this._nextRunUtc && forceStartCount == this._handledForceStartCount)
                {
                    return null;
                }

                this._handledForceStartCount = forceStartCount;

                // The configuration may have changed since the last announcement, so the index is wrapped here.
                var messages = configuration.Messages;
                var announcement = messages.ElementAt(this._nextIndex % messages.Count);
                this._nextIndex = (this._nextIndex + 1) % messages.Count;
                this._nextRunUtc = now + (configuration.Interval > MinimumInterval ? configuration.Interval : MinimumInterval);
                return announcement;
            }
        }
    }
}

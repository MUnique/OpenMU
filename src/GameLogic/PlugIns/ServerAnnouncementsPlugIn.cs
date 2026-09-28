// <copyright file="ServerAnnouncementsPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Periodically broadcasts a rotating list of server-wide messages (rules reminders, Discord invite, etc.),
/// so they don't have to be typed by hand with /post, /goldnotice or /slidenotice.
/// </summary>
/// <remarks>
/// Only one message is shown per interval, and the plugin waits out the whole configured interval before
/// showing the next one, so the messages never overlap each other. It does not coordinate with other,
/// independently scheduled announcements (Happy Hour, event start/end messages); those are spaced far apart
/// by design, so a real collision is rare.
/// </remarks>
[PlugIn]
[Display(Name = "Server Announcements", Description = "Rotates through a configured list of messages, showing one every few minutes, without needing a GM to type them.")]
[Guid("2D6C9E4A-7B3E-4F8C-9E1D-8A4B6F0C2E7D")]
public class ServerAnnouncementsPlugIn : IPeriodicTaskPlugIn, ISupportCustomConfiguration<ServerAnnouncementsConfiguration>, ISupportDefaultCustomConfiguration, IDisabledByDefault
{
    private static readonly ConcurrentDictionary<IGameContext, State> States = new();

    /// <inheritdoc />
    public ServerAnnouncementsConfiguration? Configuration { get; set; }

    /// <inheritdoc />
    public object CreateDefaultConfig() => ServerAnnouncementsConfiguration.Default;

    /// <inheritdoc />
    public void ForceStart()
    {
        // Not applicable: there's no single "event" to start, just a rotation. A game master can always
        // send a message immediately with /post, /goldnotice or /slidenotice.
    }

    /// <inheritdoc />
    public async ValueTask ExecuteTaskAsync(GameContext gameContext)
    {
        var configuration = this.Configuration;
        if (configuration is not { Messages.Count: > 0 })
        {
            return;
        }

        var state = States.GetOrAdd(gameContext, _ => new State());
        if (DateTime.UtcNow < state.NextRunUtc)
        {
            return;
        }

        var message = configuration.Messages[state.Index % configuration.Messages.Count];
        state.Index = (state.Index + 1) % configuration.Messages.Count;
        state.NextRunUtc = DateTime.UtcNow.AddMinutes(Math.Max(1, configuration.IntervalMinutes));

        await gameContext.SendGlobalMessageAsync(message.Text, message.Type).ConfigureAwait(false);
    }

    private sealed class State
    {
        public int Index { get; set; }

        public DateTime NextRunUtc { get; set; }
    }
}

// <copyright file="AutoBroadcastMessagesPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.PlugIns;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.DataModel.Composition;

/// <summary>
/// Periodically broadcasts golden-center messages to all players.
/// Each message has its own interval and optional initial offset to avoid collisions.
/// Configure messages in Admin Panel → Plugins.
/// </summary>
[PlugIn("Auto Broadcast Messages", "Sends configurable golden messages globally at individual intervals.")]
[Guid("F1A4CF77-7B1C-420B-9B0F-3AAB7F8E7A3D")]
public class AutoBroadcastMessagesPlugIn : IPeriodicTaskPlugIn, ISupportCustomConfiguration<AutoBroadcastMessagesConfiguration>, ISupportDefaultCustomConfiguration
{
    private static readonly ConcurrentDictionary<IGameContext, PerContextState> States = new();

    /// <inheritdoc />
    public AutoBroadcastMessagesConfiguration? Configuration { get; set; }

    /// <inheritdoc />
    public object CreateDefaultConfig()
    {
        // Two example messages with different intervals and offsets.
        var config = new AutoBroadcastMessagesConfiguration();
        config.Messages.Add(new BroadcastMessageEntry
        {
            Message = "¡Gracias por jugar!",
            Interval = TimeSpan.FromMinutes(10),
            InitialDelay = TimeSpan.FromMinutes(1),
            MessageType = MessageType.GoldenCenter,
            Enabled = true,
        });
        config.Messages.Add(new BroadcastMessageEntry
        {
            Message = "Síguenos en Discord para novedades.",
            Interval = TimeSpan.FromMinutes(15),
            InitialDelay = TimeSpan.FromMinutes(3),
            MessageType = MessageType.GoldenCenter,
            Enabled = true,
        });

        return config;
    }

    /// <inheritdoc />
    public void ForceStart()
    {
        foreach (var state in States.Values)
        {
            state.ForceNow = true;
        }
    }

    /// <inheritdoc />
    public async ValueTask ExecuteTaskAsync(GameContext gameContext)
    {
        var config = this.Configuration ??= (AutoBroadcastMessagesConfiguration)this.CreateDefaultConfig();
        if (config.Messages.Count == 0)
        {
            return;
        }

        var state = States.GetOrAdd(gameContext, _ => new PerContextState());
        var now = DateTime.UtcNow;

        // Ensure stable schedule for each message index.
        for (var i = 0; i < config.Messages.Count; i++)
        {
            var entry = config.Messages[i];
            if (!entry.Enabled || string.IsNullOrWhiteSpace(entry.Message))
            {
                continue;
            }

            if (!state.NextRunByIndex.TryGetValue(i, out var next))
            {
                // First schedule for this entry.
                var first = now + entry.InitialDelay;
                state.NextRunByIndex[i] = first;
                continue;
            }

            if (state.ForceNow || now >= next)
            {
                try
                {
                    await gameContext.SendGlobalMessageAsync(entry.Message, entry.MessageType).ConfigureAwait(false);
                }
                catch
                {
                    // ignore send errors per tick to keep scheduler stable.
                }

                // Schedule next run based on interval.
                state.NextRunByIndex[i] = now + entry.Interval;
            }
        }

        state.ForceNow = false;
    }

    private class PerContextState
    {
        public ConcurrentDictionary<int, DateTime> NextRunByIndex { get; } = new();

        public bool ForceNow { get; set; }
    }
}

/// <summary>
/// Configuration for <see cref="AutoBroadcastMessagesPlugIn"/>.
/// </summary>
public class AutoBroadcastMessagesConfiguration
{
    /// <summary>
    /// Gets or sets the list of messages with their own schedule.
    /// </summary>
    [MemberOfAggregate]
    [ScaffoldColumn(true)]
    [Display(Name = "Messages")]
    public IList<BroadcastMessageEntry> Messages { get; set; } = new List<BroadcastMessageEntry>();
}

/// <summary>
/// A scheduled broadcast message entry.
/// </summary>
public class BroadcastMessageEntry
{
    /// <summary>
    /// Gets or sets if this message is enabled.
    /// </summary>
    [Display(Name = "Enabled")]
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the message text.
    /// </summary>
    [Display(Name = "Message Text")]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the interval between broadcasts.
    /// </summary>
    [Display(Name = "Interval")]
    public TimeSpan Interval { get; set; } = TimeSpan.FromMinutes(10);

    /// <summary>
    /// Gets or sets an initial delay before the first broadcast.
    /// Useful to avoid overlaps with other messages.
    /// </summary>
    [Display(Name = "Initial Delay")]
    public TimeSpan InitialDelay { get; set; } = TimeSpan.Zero;

    /// <summary>
    /// Gets or sets the message type (e.g. GoldenCenter, BlueNormal).
    /// </summary>
    [Display(Name = "Type")]
    public MessageType MessageType { get; set; } = MessageType.GoldenCenter;
}


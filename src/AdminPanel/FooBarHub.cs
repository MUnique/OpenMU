// <copyright file="FooBarHub.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using log4net.SignalR;

    /// <summary>
    /// A test hub.
    /// </summary>
    public class FooBarHub : SignalrAppenderHubBase<ILogHubClient>
    {
        private const int MaximumCachedEntries = 200;

        // TODO: instead of a linkedlist we could implement something like a ring buffer. Currently we might add a lot of pressure to the GC.
        private static readonly IDictionary<string, LinkedList<LogEntry>> CachedEntriesPerGroup =
            new ConcurrentDictionary<string, LinkedList<LogEntry>>();

        static FooBarHub()
        {
            SignalrAppenderHub.OnMessageLoggedByGlobalHost += (sender, entry) => AddEntryToCache(entry.Entry, entry.GroupName);
        }

        /// <summary>
        /// Subscribes the specified group name. This causes <see cref="ILogHubClient.Initialize"/> to be called with the available logger names
        /// and the cached log entries which have a higher id than <paramref name="idOfLastReceivedEntry"/>.
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="idOfLastReceivedEntry">The identifier of last received entry.</param>
        public void Subscribe(string groupName, long idOfLastReceivedEntry)
        {
            this.Listen(groupName);
            var client = this.Clients.Client(this.Context.ConnectionId);
            var loggers = log4net.LogManager.GetCurrentLoggers().Select(log => log.Logger.Name).OrderBy(name => name).ToList();

            // connecting should not happen often, so a lock is sufficient here
            var cache = GetCache(groupName);
            lock (cache)
            {
                client.Initialize(loggers, cache.Where(entry => entry.Id > idOfLastReceivedEntry).ToList());
            }
        }

        /// <summary>
        /// Unsubscribes from this hub and the specified group name.
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        public void Unsubscribe(string groupName)
        {
            this.Groups.Remove(this.Context.ConnectionId, groupName);
        }

        /// <summary>
        /// Subscribes to the log entries of this hub with the <see cref="SignalrAppenderHubBase{TClient}.DefaultGroup"/>.
        /// </summary>
        public void Subscribe() => this.Listen(DefaultGroup);

        /// <summary>
        /// Unsubscribes from the hub with the <see cref="SignalrAppenderHubBase{TClient}.DefaultGroup"/>.
        /// </summary>
        public void Unsubscribe() => this.Unsubscribe(DefaultGroup);

        /// <inheritdoc />
        public override void OnMessageLogged(LogEntry e, string groupName)
        {
            AddEntryToCache(e, groupName);
            base.OnMessageLogged(e, groupName);
        }

        private static void AddEntryToCache(LogEntry e, string groupName)
        {
            var cache = GetCache(groupName);
            lock (cache)
            {
                cache.AddLast(e);
                if (cache.Count > MaximumCachedEntries)
                {
                    cache.RemoveFirst();
                }
            }
        }

        private static LinkedList<LogEntry> GetCache(string groupName)
        {
            if (!CachedEntriesPerGroup.TryGetValue(groupName, out var cache))
            {
                cache = new LinkedList<LogEntry>();
                CachedEntriesPerGroup.Add(groupName, cache);
            }

            return cache;
        }
    }
}

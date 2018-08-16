// <copyright file="SystemHub.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using System.Timers;
    using log4net;
    using Microsoft.AspNetCore.SignalR;
    using Timer = System.Timers.Timer;

    /// <summary>
    /// A SignalR hub which provides performance statistics about the system.
    /// </summary>
    public class SystemHub : Hub<ISystemHubClient>
    {
        private const string SubscriberGroup = "Subscribers";
        private const int TimerInterval = 1000;
        private static readonly ILog Log = LogManager.GetLogger(typeof(SystemHub));

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable we want to keep it from getting garbage collected
        private static readonly Timer Timer;

        private static readonly Process ThisProcess;
        private static TimeSpan lastProcessorTime;
        private static ISystemHubClient subscribers;

        static SystemHub()
        {
            ThisProcess = Process.GetCurrentProcess();
            lastProcessorTime = ThisProcess.TotalProcessorTime;
            Timer = new Timer(TimerInterval);
            Timer.Elapsed += TimerElapsed;
            Timer.Start();
        }

        /// <summary>
        /// Subscribes to this hub.
        /// </summary>
        /// <returns>The task.</returns>
        public async Task Subscribe()
        {
            AssignSubscribersIfRequired(this.Clients.Groups(SubscriberGroup));
            await this.Groups.AddToGroupAsync(this.Context.ConnectionId, SubscriberGroup).ConfigureAwait(false);
        }

        /// <summary>
        /// Unsubscribes from this hub.
        /// </summary>
        /// <returns>The task.</returns>
        public async Task Unsubscribe()
        {
            await this.Groups.RemoveFromGroupAsync(this.Context.ConnectionId, SubscriberGroup).ConfigureAwait(false);
        }

        private static void AssignSubscribersIfRequired(ISystemHubClient subscriberGroupProxy)
        {
            if (subscribers == null)
            {
                subscribers = subscriberGroupProxy;
            }
        }

        private static void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            var usedTimeInInterval = ThisProcess.TotalProcessorTime.Subtract(lastProcessorTime);
            lastProcessorTime = ThisProcess.TotalProcessorTime;
            var cpuPercentInstance = (float)usedTimeInInterval.TotalMilliseconds / TimerInterval * 100;

            // There is currently no easy way to get a total cpu percentage. Iterating through all processes would be a way, but it's slow and limited.
            // So, we just transmit the instance percentage also as total percentage.
            subscribers?.Update(cpuPercentInstance, cpuPercentInstance, 0, 0);
        }
    }
}

// <copyright file="SystemHub.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel
{
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
        private static readonly ILog Log = LogManager.GetLogger(typeof(SystemHub));

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable we want to keep it from getting garbage collected
        private static readonly Timer Timer;

        static SystemHub()
        {
            Timer = new Timer(1000);
            Timer.Elapsed += TimerElapsed;
            Timer.Start();
        }

        /// <summary>
        /// Subscribes to this hub.
        /// </summary>
        public void Subscribe()
        {
            this.Groups.AddToGroupAsync(this.Context.ConnectionId, SubscriberGroup);
        }

        /// <summary>
        /// Unsubscribes from this hub.
        /// </summary>
        public void Unsubscribe()
        {
            this.Groups.RemoveFromGroupAsync(this.Context.ConnectionId, SubscriberGroup);
        }

        private static void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            // to be implemented
        }
    }
}

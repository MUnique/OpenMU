// <copyright file="SystemHub.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel
{
    using System.Diagnostics;
    using System.Timers;
    using log4net;
    using Microsoft.AspNet.SignalR;
    using Timer = System.Timers.Timer;

    /// <summary>
    /// A SignalR hub which provides performance statistics about the system.
    /// </summary>
    public class SystemHub : Hub<ISystemHubClient>
    {
        private const string SubscriberGroup = "Subscribers";
        private static readonly ILog Log = LogManager.GetLogger(typeof(SystemHub));
        private static readonly PerformanceCounter CpuCounterTotal;
        private static readonly PerformanceCounter CpuCounterInstance;
        private static readonly PerformanceCounter BytesSentCounter;
        private static readonly PerformanceCounter BytesReceivedCounter;
        private static readonly Timer Timer;

        static SystemHub()
        {
            CpuCounterInstance = new PerformanceCounter("Process", "% Processor Time", Process.GetCurrentProcess().ProcessName); // TODO: What if there are multiple processes?
            CpuCounterTotal = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            BytesSentCounter = new PerformanceCounter(".Net CLR Networking", "Bytes Sent"); // Does always return 0 :-(
            BytesReceivedCounter = new PerformanceCounter(".Net CLR Networking", "Bytes Received"); // Does always return 0 :-(

            CpuCounterTotal.NextValue();
            CpuCounterInstance.NextValue();
            BytesSentCounter.NextValue();
            BytesReceivedCounter.NextValue();

            Timer = new Timer(1000);
            Timer.Elapsed += TimerElapsed;
            Timer.Start();
        }

        /// <summary>
        /// Subscribes to this hub.
        /// </summary>
        public void Subscribe()
        {
            this.Groups.Add(this.Context.ConnectionId, SubscriberGroup);
        }

        /// <summary>
        /// Unsubscribes from this hub.
        /// </summary>
        public void Unsubscribe()
        {
            this.Groups.Remove(this.Context.ConnectionId, SubscriberGroup);
        }

        private static void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<SystemHub, ISystemHubClient>();
            hubContext.Clients.Group(SubscriberGroup).Update(CpuCounterTotal.NextValue(), CpuCounterInstance.NextValue(), BytesReceivedCounter.NextValue(), BytesSentCounter.NextValue());
        }
    }
}

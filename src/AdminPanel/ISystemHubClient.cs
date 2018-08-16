// <copyright file="ISystemHubClient.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel
{
    using System.Threading.Tasks;

    /// <summary>
    /// A client interface of the <see cref="SystemHub"/>.
    /// </summary>
    public interface ISystemHubClient
    {
        /// <summary>
        /// Updates the specified system informations.
        /// </summary>
        /// <param name="cpuPercentTotal">The cpu percent of the total system.</param>
        /// <param name="cpuPercentInstance">The cpu percent of this process instance.</param>
        /// <param name="bytesSent">The bytes sent.</param>
        /// <param name="bytesReceived">The bytes received.</param>
        /// <returns>The task.</returns>
        Task Update(float cpuPercentTotal, float cpuPercentInstance, float bytesSent, float bytesReceived);
    }
}

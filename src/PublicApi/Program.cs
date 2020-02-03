// <copyright file="Program.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.PublicApi
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// The main entry class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The async task.</returns>
        public static Task Main(string[] args)
        {
            return ApiHost.RunAsync(new List<IGameServer>(), new List<IConnectServer>(), null);
        }
    }
}

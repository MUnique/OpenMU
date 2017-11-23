// <copyright file="WarpInfo.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration
{
    /// <summary>
    /// Defines a warp list entry.
    /// </summary>
    /// <remarks>
    /// AFAIK, this is only relevant if we want to host the server for clients of season 5 and below.
    /// With season 6 and upwards, gates are used directly. Then I ask myself how the warp price is determined -> TODO.
    /// </remarks>
    public class WarpInfo
    {
        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        public ushort Index { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the warp zen.
        /// </summary>
        public uint WarpCosts { get; set; }

        /// <summary>
        /// Gets or sets the warp level req.
        /// </summary>
        public ushort WarpLvlReq { get; set; }

        /// <summary>
        /// Gets or sets the gate.
        /// </summary>
        public virtual ExitGate Gate { get; set; }
    }
}

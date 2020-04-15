// <copyright file="MoneyDropped.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Packets.ServerToClient
{
    /// <summary>
    /// Extends the <see cref="MoneyDropped"/>.
    /// </summary>
    public readonly ref partial struct MoneyDropped
    {
        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        public uint Amount
        {
            get => (uint)(this.data[10] << 16 | this.data[11] << 8 | this.data[13]);
            set
            {
                this.data[10] = (byte)(value >> 16 & 0xFF);
                this.data[11] = (byte)(value >> 8 & 0xFF);
                this.data[13] = (byte)(value & 0xFF);
            }
        }
    }
}

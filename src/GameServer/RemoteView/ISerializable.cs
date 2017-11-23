// <copyright file="ISerializable.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView
{
    /// <summary>
    /// Description of ISerializable.
    /// </summary>
    public interface ISerializable
    {
        /// <summary>
        /// Serializes the object to binary.
        /// </summary>
        /// <returns>The serialized object.</returns>
        byte[] Serialize();
    }
}

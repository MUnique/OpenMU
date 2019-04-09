// <copyright file="BelongsToGroupAttribute.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using System;

    /// <summary>
    /// Attribute for <see cref="ISubPacketHandlerPlugIn"/>s to define their parent group.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Class)]
    public class BelongsToGroupAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BelongsToGroupAttribute"/> class.
        /// </summary>
        /// <param name="groupKey">The group key.</param>
        public BelongsToGroupAttribute(byte groupKey)
        {
            this.GroupKey = groupKey;
        }

        /// <summary>
        /// Gets the group key where a sub packet handler belongs to.
        /// </summary>
        public byte GroupKey { get; }
    }
}
// <copyright file="PowerUpDefinitionWithDuration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Attributes
{
    using MUnique.OpenMU.DataModel.Composition;

    /// <summary>
    /// A power up definition which got a duration.
    /// </summary>
    public class PowerUpDefinitionWithDuration : PowerUpDefinition
    {
        /// <summary>
        /// Gets or sets the duration which describes how long the boost applies, in seconds.
        /// </summary>
        [MemberOfAggregate]
        public virtual PowerUpDefinitionValue? Duration { get; set; }
    }
}

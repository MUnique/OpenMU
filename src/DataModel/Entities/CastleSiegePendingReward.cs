// <copyright file="CastleSiegePendingReward.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Entities;

/// <summary>
/// Stores an item reward which could not yet be delivered to a Castle Siege participant.
/// </summary>
[AggregateRoot]
public class CastleSiegePendingReward
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the rewarded character.
    /// </summary>
    public Guid CharacterId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the item definition to deliver.
    /// </summary>
    public Guid ItemDefinitionId { get; set; }
}

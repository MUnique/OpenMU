// <copyright file="PartyExperienceBonusDefaults.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// The default values of the party experience attributes. They are shared by the data initialization,
/// which is used for new databases, and by the update plugins for existing databases.
/// </summary>
internal static class PartyExperienceBonusDefaults
{
    /// <summary>
    /// Gets the party experience attributes with their default values.
    /// </summary>
    public static IReadOnlyList<(AttributeDefinition Attribute, float Value)> Values { get; } =
    [
        (Stats.PartyExperienceMultiplier, 1.05f),
        (Stats.SetPartyExperienceBonus, 0f),
    ];
}

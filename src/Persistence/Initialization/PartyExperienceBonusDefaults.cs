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
/// <remarks>
/// The values are fractions, so 0.01 is a bonus of 1%. With the defaults, every party member adds 1% to the
/// experience multiplier. The set party bonus defaults to 0, so set parties get the same bonus as general
/// parties until it is configured:
/// <code>
/// | Classification | 2 Players | 3 Players | 4 Players | 5 Players |
/// |----------------|-----------|-----------|-----------|-----------|
/// | General Party  | 2%        | 3%        | 4%        | 5%        |
/// | Set Party      | -         | 3%        | 4%        | 5%        |
/// </code>
/// </remarks>
internal static class PartyExperienceBonusDefaults
{
    /// <summary>
    /// Gets the party experience attributes with their default values.
    /// </summary>
    public static IReadOnlyList<(AttributeDefinition Attribute, float Value)> Values { get; } =
    [
        (Stats.PartyExperienceMultiplier, 0.01f),
        (Stats.SetPartyExperienceBonus, 0f),
    ];
}

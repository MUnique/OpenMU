// <copyright file="ResolvedItemStatProfile.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.ItemFormulas;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// The item formulas of an item definition: its <see cref="ItemDefinition.StatProfile"/> laid over the profile named
/// <see cref="ItemStatProfile.DefaultName"/> of the game configuration, part by part, as the game client lays the
/// sections of its item stat file. An item without a profile has the formulas of the default.
/// </summary>
/// <remarks>
/// Nothing is cached, so that a change of a profile in the admin panel applies at once.
/// </remarks>
public sealed class ResolvedItemStatProfile
{
    private readonly ItemStatProfile? _profile;
    private readonly ItemStatProfile _defaultProfile;
    private readonly GameConfiguration _gameConfiguration;

    private ResolvedItemStatProfile(ItemStatProfile? profile, ItemStatProfile defaultProfile, GameConfiguration gameConfiguration)
    {
        this._profile = profile;
        this._defaultProfile = defaultProfile;
        this._gameConfiguration = gameConfiguration;
    }

    /// <summary>
    /// Gets what the item formulas add to the drop level of an excellent item.
    /// </summary>
    public int ExcellentDropLevelBonus => this._gameConfiguration.ExcellentDropLevelBonus;

    /// <summary>
    /// Gets what the item formulas add to the drop level of an ancient item.
    /// </summary>
    public int AncientDropLevelBonus => this._gameConfiguration.AncientDropLevelBonus;

    /// <summary>
    /// Gets what each item level adds to the level requirement.
    /// </summary>
    public int LevelPerItemLevel => this._profile?.LevelPerItemLevel ?? this._defaultProfile.LevelPerItemLevel ?? 0;

    /// <summary>
    /// Gets what an excellent item needs more as level.
    /// </summary>
    public int ExcellentLevelAdd => this._profile?.ExcellentLevelAdd ?? this._defaultProfile.ExcellentLevelAdd ?? 0;

    /// <summary>
    /// Gets what each level of the item's option adds to its strength requirement.
    /// </summary>
    public int StrengthPerOptionLevel => this._profile?.StrengthPerOptionLevel ?? this._defaultProfile.StrengthPerOptionLevel ?? 0;

    /// <summary>
    /// Gets the item formulas of the item definition.
    /// </summary>
    /// <param name="definition">The item definition.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <returns>The item formulas of the item definition.</returns>
    /// <exception cref="InvalidOperationException">The game configuration has no profile named <see cref="ItemStatProfile.DefaultName"/>.</exception>
    public static ResolvedItemStatProfile Of(ItemDefinition definition, GameConfiguration gameConfiguration)
    {
        var defaultProfile = FindDefaultProfile(gameConfiguration)
                             ?? throw new InvalidOperationException($"The game configuration has no item stat profile named '{ItemStatProfile.DefaultName}'. Apply the pending configuration updates on the update page of the admin panel.");
        var profile = definition.StatProfile is { } own && own != defaultProfile ? own : null;
        return new ResolvedItemStatProfile(profile, defaultProfile, gameConfiguration);
    }

    /// <summary>
    /// Determines whether the game configuration has the profile named <see cref="ItemStatProfile.DefaultName"/>,
    /// which the item formulas need.
    /// </summary>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <returns><c>true</c>, if the game configuration has the default profile.</returns>
    public static bool HasDefaultProfile(GameConfiguration gameConfiguration) => FindDefaultProfile(gameConfiguration) is not null;

    /// <summary>
    /// Gets the requirement formula of the requirement attribute: the one of the item's profile, or else the one of the default.
    /// </summary>
    /// <param name="attribute">The attribute of the requirement.</param>
    /// <returns>The requirement formula, or <c>null</c> when neither profile has one for the attribute.</returns>
    public ItemRequirementFormula? GetRequirementFormula(AttributeDefinition attribute)
    {
        return this._profile?.RequirementFormulas.FirstOrDefault(formula => formula.Attribute == attribute)
               ?? this._defaultProfile.RequirementFormulas.FirstOrDefault(formula => formula.Attribute == attribute);
    }

    /// <summary>
    /// Gets the excellent part of the terms of the attribute: the one the item's profile writes, or else the one of the default.
    /// </summary>
    /// <param name="attribute">The attribute.</param>
    /// <returns>The excellent part.</returns>
    public ItemStatTermPart GetExcellentPart(AttributeDefinition attribute) => this.GetPart(attribute, isAncient: false);

    /// <summary>
    /// Gets the ancient part of the terms of the attribute: the one the item's profile writes, or else the one of the default.
    /// </summary>
    /// <param name="attribute">The attribute.</param>
    /// <returns>The ancient part.</returns>
    public ItemStatTermPart GetAncientPart(AttributeDefinition attribute) => this.GetPart(attribute, isAncient: true);

    private static ItemStatProfile? FindDefaultProfile(GameConfiguration gameConfiguration)
    {
        return gameConfiguration.ItemStatProfiles.FirstOrDefault(profile => profile.Name == ItemStatProfile.DefaultName);
    }

    private static ItemStatTermPart GetWrittenPart(ItemStatProfile? profile, AttributeDefinition attribute, bool isAncient)
    {
        if (profile?.StatTerms.FirstOrDefault(terms => terms.Attribute == attribute) is not { } terms)
        {
            return default;
        }

        return isAncient
            ? new ItemStatTermPart(terms.AncientState, terms.AncientTimes, terms.AncientAdd, terms.AncientDropLevelDivisor, terms.AncientAfterLevelBonus)
            : new ItemStatTermPart(terms.ExcellentState, terms.ExcellentTimes, terms.ExcellentAdd, terms.ExcellentDropLevelDivisor, false);
    }

    private ItemStatTermPart GetPart(AttributeDefinition attribute, bool isAncient)
    {
        var ownPart = GetWrittenPart(this._profile, attribute, isAncient);
        return ownPart.State != ItemStatTermState.NotWritten
            ? ownPart
            : GetWrittenPart(this._defaultProfile, attribute, isAncient);
    }
}

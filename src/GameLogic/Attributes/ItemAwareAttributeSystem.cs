// <copyright file="ItemAwareAttributeSystem.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Attributes;

using MUnique.OpenMU.AttributeSystem;

/// <summary>
/// An attribute system which considers items of a character.
/// </summary>
public class ItemAwareAttributeSystem : AttributeSystem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ItemAwareAttributeSystem"/> class.
    /// </summary>
    /// <param name="character">The character.</param>
    public ItemAwareAttributeSystem(Character character)
        : base(character.Attributes, character.CharacterClass!.BaseAttributeValues, character.CharacterClass.AttributeCombinations)
    {
        this.ItemPowerUps = new Dictionary<Item, IReadOnlyList<PowerUpWrapper>>();
    }

    /// <summary>
    /// Gets the item power ups.
    /// </summary>
    public IDictionary<Item, IReadOnlyList<PowerUpWrapper>> ItemPowerUps { get; }

    /// <summary>
    /// Gets or sets the item set power ups.
    /// </summary>
    public IReadOnlyList<PowerUpWrapper>? ItemSetPowerUps { get; set; }
}
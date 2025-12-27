// <copyright file="RingOfWarrior.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version097d.Items;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Initializes the ring of warrior used by new characters.
/// </summary>
public class RingOfWarrior : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RingOfWarrior"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public RingOfWarrior(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    public override void Initialize()
    {
        if (this.GameConfiguration.Items.Any(item => item.Group == 13 && item.Number == 20))
        {
            return;
        }

        var ring = this.Context.CreateNew<ItemDefinition>();
        ring.SetGuid(13, 20);
        ring.Group = 13;
        ring.Number = 20;
        ring.Name = "Ring of Warrior";
        ring.Width = 1;
        ring.Height = 1;
        ring.Durability = 30;
        ring.MaximumItemLevel = 2;
        ring.IsBoundToCharacter = true;
        ring.DropLevel = 0;
        ring.DropsFromMonsters = false;
        ring.ItemSlot = this.GameConfiguration.ItemSlotTypes.FirstOrDefault(slotType => slotType.ItemSlots.Contains(10));

        this.CreateItemRequirementIfNeeded(ring, Stats.Level, 0);
        foreach (var characterClass in this.GameConfiguration.CharacterClasses)
        {
            ring.QualifiedCharacters.Add(characterClass);
        }

        this.GameConfiguration.Items.Add(ring);
    }
}

// <copyright file="Scrolls.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version075.Items;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;

/// <summary>
/// Initializer for scroll items which allow a character to learn <see cref="Skill"/>s.
/// </summary>
/// <seealso cref="MUnique.OpenMU.Persistence.Initialization.InitializerBase" />
public class Scrolls : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Scrolls"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public Scrolls(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <summary>
    /// Initializes the scroll data.
    /// </summary>
    public override void Initialize()
    {
        this.CreateScroll(0, 1, "Scroll of Poison", 30, 0, 140, 17000);
        this.CreateScroll(1, 2, "Scroll of Meteorite", 21, 0, 104, 11000);
        this.CreateScroll(2, 3, "Scroll of Lighting", 13, 0, 72, 3000);
        this.CreateScroll(3, 4, "Scroll of Fire Ball", 5, 0, 40, 300);
        this.CreateScroll(4, 5, "Scroll of Flame", 35, 0, 160, 21000);
        this.CreateScroll(5, 6, "Scroll of Teleport", 17, 0, 88, 5000);
        this.CreateScroll(6, 7, "Scroll of Ice", 25, 0, 120, 14000);
        this.CreateScroll(7, 8, "Scroll of Twister", 40, 0, 180, 25000);
        this.CreateScroll(8, 9, "Scroll of Evil Spirit", 50, 0, 220, 35000);
        this.CreateScroll(9, 10, "Scroll of Hellfire", 60, 0, 260, 60000);
        this.CreateScroll(10, 11, "Scroll of Power Wave", 9, 0, 56, 1100);
        this.CreateScroll(11, 12, "Scroll of Aqua Beam", 74, 0, 345, 100000);
    }

    /// <summary>
    /// Creates the scroll.
    /// </summary>
    /// <param name="number">The number.</param>
    /// <param name="skillNumber">The skill number.</param>
    /// <param name="name">The name.</param>
    /// <param name="dropLevel">The drop level.</param>
    /// <param name="levelRequirement">The level requirement.</param>
    /// <param name="energyRequirement">The energy requirement.</param>
    /// <param name="money">The money.</param>
    protected void CreateScroll(byte number, int skillNumber, string name, byte dropLevel, int levelRequirement, int energyRequirement, int money)
    {
        var scroll = this.Context.CreateNew<ItemDefinition>();
        this.GameConfiguration.Items.Add(scroll);
        scroll.Group = 15;
        scroll.Number = number;
        scroll.Skill = this.GameConfiguration.Skills.First(skill => skill.Number == skillNumber);
        scroll.Width = 1;
        scroll.Height = 2;
        scroll.Name = name;
        scroll.DropLevel = dropLevel;
        scroll.DropsFromMonsters = true;
        scroll.Durability = 1;
        scroll.QualifiedCharacters.Add(this.GameConfiguration.CharacterClasses.First(c => c.Number == (int)CharacterClassNumber.DarkWizard));

        this.CreateItemRequirementIfNeeded(scroll, Stats.Level, levelRequirement);
        this.CreateItemRequirementIfNeeded(scroll, Stats.TotalEnergyRequirementValue, energyRequirement);

        scroll.Value = money;
        scroll.SetGuid(scroll.Group, scroll.Number);
    }
}
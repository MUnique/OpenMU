// <copyright file="GameConfigurationInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version097k;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Persistence.Initialization.Items;
using MUnique.OpenMU.Persistence.Initialization.Version095d.Events;
using MUnique.OpenMU.Persistence.Initialization.Version095d.Items;
using MUnique.OpenMU.Persistence.Initialization.Version097k.Events;
using EventTicketItems = MUnique.OpenMU.Persistence.Initialization.Version097k.Items.EventTicketItems;
using Jewels = MUnique.OpenMU.Persistence.Initialization.Version097d.Items.Jewels;
using Quest = MUnique.OpenMU.Persistence.Initialization.Version097k.Items.Quest;
using Wings = MUnique.OpenMU.Persistence.Initialization.Version097k.Items.Wings;

/// <summary>
/// Initializes the <see cref="GameConfiguration"/> of version 0.97k.
/// </summary>
public class GameConfigurationInitializer : GameConfigurationInitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GameConfigurationInitializer"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public GameConfigurationInitializer(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    protected override IEnumerable<ItemOptionType> OptionTypes
    {
        get
        {
            yield return ItemOptionTypes.Option;
            yield return ItemOptionTypes.Luck;
            yield return ItemOptionTypes.Excellent;
            yield return ItemOptionTypes.Wing;
        }
    }

    /// <inheritdoc />
    public override void Initialize()
    {
        base.Initialize();

        new CharacterClassInitialization(this.Context, this.GameConfiguration).Initialize();

        new Version095d.SkillsInitializer(this.Context, this.GameConfiguration).Initialize();
        new Orbs(this.Context, this.GameConfiguration).Initialize();
        new Scrolls(this.Context, this.GameConfiguration).Initialize();
        new EventTicketItems(this.Context, this.GameConfiguration).Initialize();
        new Quest(this.Context, this.GameConfiguration).Initialize();
        new Jewels(this.Context, this.GameConfiguration).Initialize();
        new ExcellentOptions(this.Context, this.GameConfiguration).Initialize();
        new Armors(this.Context, this.GameConfiguration).Initialize();
        new Wings(this.Context, this.GameConfiguration).Initialize();
        new Pets(this.Context, this.GameConfiguration).Initialize();
        new Weapons(this.Context, this.GameConfiguration).Initialize();
        new Version075.Items.Potions(this.Context, this.GameConfiguration).Initialize();
        new Jewelery(this.Context, this.GameConfiguration).Initialize();
        new BoxOfLuck(this.Context, this.GameConfiguration).Initialize();
        new NpcInitialization(this.Context, this.GameConfiguration).Initialize();
        new Version095d.InvasionMobsInitialization(this.Context, this.GameConfiguration).Initialize();

        new GameMapsInitializer(this.Context, this.GameConfiguration).Initialize();
        this.AssignCharacterClassHomeMaps();
        new ChaosMixes(this.Context, this.GameConfiguration).Initialize();
        new Gates(this.Context, this.GameConfiguration).Initialize();
        new DevilSquareInitializer(this.Context, this.GameConfiguration).Initialize();
        new BloodCastleInitializer(this.Context, this.GameConfiguration).Initialize();
        new Quests(this.Context, this.GameConfiguration).Initialize();

        this.QualifySecondClasses();
    }

    /// <summary>
    /// Makes every item and skill which is usable by a first character class also usable by its second class.
    /// </summary>
    /// <remarks>
    /// The initializers of version 0.75 and 0.95d, which are reused here, qualify just the first classes,
    /// because these versions don't have a second class.
    /// </remarks>
    private void QualifySecondClasses()
    {
        var evolutions = this.GameConfiguration.CharacterClasses
            .Where(c => c.NextGenerationClass is not null)
            .ToDictionary(c => c, c => c.NextGenerationClass!);

        void AddEvolutions(ICollection<CharacterClass> qualifiedCharacters)
        {
            foreach (var characterClass in qualifiedCharacters.ToList())
            {
                if (evolutions.TryGetValue(characterClass, out var nextClass)
                    && !qualifiedCharacters.Contains(nextClass))
                {
                    qualifiedCharacters.Add(nextClass);
                }
            }
        }

        foreach (var item in this.GameConfiguration.Items)
        {
            AddEvolutions(item.QualifiedCharacters);
        }

        foreach (var skill in this.GameConfiguration.Skills)
        {
            AddEvolutions(skill.QualifiedCharacters);
        }
    }
}

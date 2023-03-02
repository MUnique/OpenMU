// <copyright file="GameConfigurationInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version095d;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Persistence.Initialization.Items;
using MUnique.OpenMU.Persistence.Initialization.Version095d.Events;
using MUnique.OpenMU.Persistence.Initialization.Version095d.Items;

/// <summary>
/// Initializes the <see cref="GameConfiguration"/>.
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
        }
    }

    /// <inheritdoc />
    public override void Initialize()
    {
        base.Initialize();
        
        new CharacterClassInitialization(this.Context, this.GameConfiguration).Initialize();

        new SkillsInitializer(this.Context, this.GameConfiguration).Initialize();
        new Orbs(this.Context, this.GameConfiguration).Initialize();
        new Scrolls(this.Context, this.GameConfiguration).Initialize();
        new EventTicketItems(this.Context, this.GameConfiguration).Initialize();
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
        new InvasionMobsInitialization(this.Context, this.GameConfiguration).Initialize();

        new GameMapsInitializer(this.Context, this.GameConfiguration).Initialize();
        this.AssignCharacterClassHomeMaps();
        new ChaosMixes(this.Context, this.GameConfiguration).Initialize();
        new Gates(this.Context, this.GameConfiguration).Initialize();
        new DevilSquareInitializer(this.Context, this.GameConfiguration).Initialize();
    }
}
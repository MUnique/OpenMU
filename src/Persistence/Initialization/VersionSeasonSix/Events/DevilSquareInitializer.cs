// <copyright file="DevilSquareInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Events;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// The initializer for the devil square event.
/// </summary>
internal class DevilSquareInitializer : Version095d.Events.DevilSquareInitializer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DevilSquareInitializer" /> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public DevilSquareInitializer(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    public override void Initialize()
    {
        base.Initialize();

        var devilSquare5 = this.CreateDevilSquareDefinition(5);
        devilSquare5.MinimumCharacterLevel = 281;
        devilSquare5.MaximumCharacterLevel = 330;
        devilSquare5.MinimumSpecialCharacterLevel = 261;
        devilSquare5.MaximumSpecialCharacterLevel = 310;

        var devilSquare6 = this.CreateDevilSquareDefinition(6);
        devilSquare6.MinimumCharacterLevel = 331;
        devilSquare6.MaximumCharacterLevel = 400;
        devilSquare6.MinimumSpecialCharacterLevel = 311;
        devilSquare6.MaximumSpecialCharacterLevel = 400;

        var devilSquare7 = this.CreateDevilSquareDefinition(7);
        devilSquare7.RequiresMasterClass = true;
        devilSquare7.MinimumCharacterLevel = 0;
        devilSquare7.MaximumCharacterLevel = 400;
        devilSquare7.MinimumSpecialCharacterLevel = 0;
        devilSquare7.MaximumSpecialCharacterLevel = 400;
    }
}
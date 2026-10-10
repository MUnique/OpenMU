// <copyright file="BloodCastleInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version097k.Events;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// The initializer for the blood castle event of version 0.97k, which has six levels.
/// </summary>
internal class BloodCastleInitializer : VersionSeasonSix.Events.BloodCastleInitializer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BloodCastleInitializer" /> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public BloodCastleInitializer(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    public override void Initialize()
    {
        var bloodCastle1 = this.CreateBloodCastleDefinition(1, 11);
        bloodCastle1.MinimumCharacterLevel = 15;
        bloodCastle1.MaximumCharacterLevel = 80;
        bloodCastle1.MinimumSpecialCharacterLevel = 10;
        bloodCastle1.MaximumSpecialCharacterLevel = 60;

        var bloodCastle2 = this.CreateBloodCastleDefinition(2, 12);
        bloodCastle2.MinimumCharacterLevel = 81;
        bloodCastle2.MaximumCharacterLevel = 130;
        bloodCastle2.MinimumSpecialCharacterLevel = 61;
        bloodCastle2.MaximumSpecialCharacterLevel = 110;

        var bloodCastle3 = this.CreateBloodCastleDefinition(3, 13);
        bloodCastle3.MinimumCharacterLevel = 131;
        bloodCastle3.MaximumCharacterLevel = 180;
        bloodCastle3.MinimumSpecialCharacterLevel = 111;
        bloodCastle3.MaximumSpecialCharacterLevel = 160;

        var bloodCastle4 = this.CreateBloodCastleDefinition(4, 14);
        bloodCastle4.MinimumCharacterLevel = 181;
        bloodCastle4.MaximumCharacterLevel = 230;
        bloodCastle4.MinimumSpecialCharacterLevel = 161;
        bloodCastle4.MaximumSpecialCharacterLevel = 210;

        var bloodCastle5 = this.CreateBloodCastleDefinition(5, 15);
        bloodCastle5.MinimumCharacterLevel = 231;
        bloodCastle5.MaximumCharacterLevel = 280;
        bloodCastle5.MinimumSpecialCharacterLevel = 211;
        bloodCastle5.MaximumSpecialCharacterLevel = 260;

        // The last level has no upper level limit, because there is no seventh level.
        var bloodCastle6 = this.CreateBloodCastleDefinition(6, 16);
        bloodCastle6.MinimumCharacterLevel = 281;
        bloodCastle6.MaximumCharacterLevel = 400;
        bloodCastle6.MinimumSpecialCharacterLevel = 261;
        bloodCastle6.MaximumSpecialCharacterLevel = 400;
    }
}

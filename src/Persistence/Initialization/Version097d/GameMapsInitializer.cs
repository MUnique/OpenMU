// <copyright file="GameMapsInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version097d;

using MUnique.OpenMU.DataModel.Configuration;
using Maps095 = MUnique.OpenMU.Persistence.Initialization.Version095d.Maps;
using Maps097 = MUnique.OpenMU.Persistence.Initialization.Version097d.Maps;
using MapsS6 = MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

/// <summary>
/// Initializes the <see cref="GameMapDefinition"/>s.
/// </summary>
public class GameMapsInitializer : GameMapsInitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GameMapsInitializer"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public GameMapsInitializer(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    protected override IEnumerable<Type> MapInitializerTypes
    {
        get
        {
            yield return typeof(Maps095.Lorencia);
            yield return typeof(Version075.Maps.Dungeon);
            yield return typeof(Maps097.Devias);
            yield return typeof(Maps095.Noria);
            yield return typeof(Version075.Maps.LostTower);
            yield return typeof(Version075.Maps.Exile);
            yield return typeof(Version075.Maps.Arena);
            yield return typeof(Version075.Maps.Atlans);
            yield return typeof(Maps095.Tarkan);
            yield return typeof(Maps095.Icarus);
            yield return typeof(Maps095.DevilSquare1);
            yield return typeof(Maps095.DevilSquare2);
            yield return typeof(Maps095.DevilSquare3);
            yield return typeof(Maps095.DevilSquare4);
            yield return typeof(MapsS6.BloodCastle1);
            yield return typeof(MapsS6.BloodCastle2);
            yield return typeof(MapsS6.BloodCastle3);
            yield return typeof(MapsS6.BloodCastle4);
            yield return typeof(MapsS6.BloodCastle5);
            yield return typeof(MapsS6.BloodCastle6);
            yield return typeof(MapsS6.ChaosCastle1);
            yield return typeof(MapsS6.ChaosCastle2);
            yield return typeof(MapsS6.ChaosCastle3);
        }
    }
}

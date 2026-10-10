// <copyright file="GameMapsInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version097k;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.Version095d.Maps;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;
using Devias = MUnique.OpenMU.Persistence.Initialization.Version097k.Maps.Devias;
using Icarus = MUnique.OpenMU.Persistence.Initialization.Version095d.Maps.Icarus;
using Lorencia = MUnique.OpenMU.Persistence.Initialization.Version095d.Maps.Lorencia;
using Noria = MUnique.OpenMU.Persistence.Initialization.Version095d.Maps.Noria;

/// <summary>
/// Initializes the <see cref="GameMapDefinition"/>s of version 0.97k.
/// </summary>
/// <remarks>
/// Compared to version 0.95d, Blood Castle 1 to 6 are added. The Blood Castle maps are the same as in season 6.
/// </remarks>
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
            yield return typeof(Lorencia);
            yield return typeof(Version075.Maps.Dungeon);
            yield return typeof(Devias);
            yield return typeof(Noria);
            yield return typeof(Version075.Maps.LostTower);
            yield return typeof(Version075.Maps.Exile);
            yield return typeof(Version075.Maps.Arena);
            yield return typeof(Version075.Maps.Atlans);
            yield return typeof(Tarkan);
            yield return typeof(Icarus);
            yield return typeof(DevilSquare1);
            yield return typeof(DevilSquare2);
            yield return typeof(DevilSquare3);
            yield return typeof(DevilSquare4);
            yield return typeof(BloodCastle1);
            yield return typeof(BloodCastle2);
            yield return typeof(BloodCastle3);
            yield return typeof(BloodCastle4);
            yield return typeof(BloodCastle5);
            yield return typeof(BloodCastle6);
        }
    }
}

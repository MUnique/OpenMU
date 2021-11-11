// <copyright file="GameMapsInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version095d;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.Version095d.Maps;

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
        }
    }
}
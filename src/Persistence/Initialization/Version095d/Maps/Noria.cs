// <copyright file="Noria.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version095d.Maps;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// The initialization for the Noria map.
/// </summary>
internal class Noria : Version075.Maps.Noria
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Noria"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public Noria(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    protected override IEnumerable<MonsterSpawnArea> CreateNpcSpawns()
    {
        foreach (var npc in base.CreateNpcSpawns())
        {
            yield return npc;
        }

        yield return this.CreateMonsterSpawn(7, this.NpcDictionary[237], 171, 105, Direction.SouthEast); // Charon (Devil Square)
    }
}
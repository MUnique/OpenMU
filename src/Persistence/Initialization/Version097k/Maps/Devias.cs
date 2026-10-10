// <copyright file="Devias.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version097k.Maps;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// The initialization for the Devias map of version 0.97k.
/// </summary>
/// <remarks>
/// Compared to version 0.95d, the Messenger of Archangel is added, who lets players enter Blood Castle.
/// </remarks>
internal class Devias : Version095d.Maps.Devias
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Devias"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public Devias(IContext context, GameConfiguration gameConfiguration)
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

        yield return this.CreateMonsterSpawn(21, this.NpcDictionary[233], 208, 30, Direction.SouthWest); // Messenger of Archangel
    }
}

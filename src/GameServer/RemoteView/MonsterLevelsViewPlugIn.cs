// <copyright file="MonsterLevelsViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IMonsterLevelsViewPlugIn"/> which sends one
/// <see cref="MonsterLevels"/> message with the level of all monsters.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.MonsterLevelsViewPlugIn_Name), Description = nameof(PlugInResources.MonsterLevelsViewPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("95526FCF-0408-4ACD-AC2B-8D9D41EAB9F3")]
[MinimumClient(106, 3, ClientLanguage.Invariant)]
public class MonsterLevelsViewPlugIn : IMonsterLevelsViewPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="MonsterLevelsViewPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public MonsterLevelsViewPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public ValueTask ShowMonsterLevelsAsync()
    {
        if (this._player.Connection is not { Connected: true } connection)
        {
            return ValueTask.CompletedTask;
        }

        // The count has to fit into the C2 message, which the client couldn't read otherwise.
        var maximumCount = (ushort.MaxValue - MonsterLevelsRef.GetRequiredSize(0)) / MonsterLevelsRef.MonsterLevelRef.Length;
        var monsters = GetMonsterLevels(this._player.GameContext.Configuration).Take(maximumCount).ToList();

        int Write()
        {
            var size = MonsterLevelsRef.GetRequiredSize(monsters.Count);
            var span = connection.Output.GetSpan(size)[..size];
            var packet = new MonsterLevelsRef(span)
            {
                MonsterCount = (ushort)monsters.Count,
            };

            for (var i = 0; i < monsters.Count; i++)
            {
                var (number, level) = monsters[i];
                var target = packet[i];
                target.MonsterNumber = number;
                target.Level = level;
            }

            return size;
        }

        return connection.SendAsync(Write);
    }

    /// <summary>
    /// Gets the level of each monster which has one, ordered by the monster number.
    /// </summary>
    /// <param name="configuration">The game configuration.</param>
    /// <returns>The number and level of each monster.</returns>
    internal static IEnumerable<(ushort Number, ushort Level)> GetMonsterLevels(GameConfiguration configuration)
    {
        return configuration.Monsters
            .Where(m => m.ObjectKind == NpcObjectKind.Monster && m.Number >= 0)
            .Select(m => (m.Number, Level: m.Attributes.FirstOrDefault(a => a.AttributeDefinition == Stats.Level)?.Value ?? 0))
            .Where(m => m.Level > 0)
            .OrderBy(m => m.Number)
            .Select(m => ((ushort)m.Number, (ushort)Math.Clamp(m.Level, 0, ushort.MaxValue)));
    }
}

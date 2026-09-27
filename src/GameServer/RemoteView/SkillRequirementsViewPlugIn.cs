// <copyright file="SkillRequirementsViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="ISkillRequirementsViewPlugIn"/> which sends one
/// <see cref="SkillRequirements"/> message with all skills, and one <see cref="LearnableItemRequirements"/>
/// message with all items which teach a skill.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.SkillRequirementsViewPlugIn_Name), Description = nameof(PlugInResources.SkillRequirementsViewPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("3E9B2D71-8C4A-4F06-B5D8-1A7E6C0F92B4")]
[MinimumClient(106, 3, ClientLanguage.Invariant)]
public class SkillRequirementsViewPlugIn : ISkillRequirementsViewPlugIn
{
    /// <summary>
    /// The value of <see cref="LearnableItemRequirementsRef.LearnableItemRequirementRef.ItemLevel"/> for requirements which apply to every item level.
    /// </summary>
    private const byte AnyItemLevel = 0xFF;

    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="SkillRequirementsViewPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public SkillRequirementsViewPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ShowSkillRequirementsAsync()
    {
        if (this._player.Connection is not { Connected: true } connection)
        {
            return;
        }

        var configuration = this._player.GameContext.Configuration;
        await SendSkillsAsync(connection, configuration).ConfigureAwait(false);
        await SendItemsAsync(connection, configuration).ConfigureAwait(false);
    }

    private static ValueTask SendSkillsAsync(IConnection connection, GameConfiguration configuration)
    {
        // Both counts have to fit into the C2 message, which the client couldn't read otherwise.
        var maximumCount = (ushort.MaxValue - SkillRequirementsRef.GetRequiredSize(0)) / SkillRequirementsRef.SkillRequirementRef.Length;
        var skills = configuration.Skills
            .Where(s => s.Number >= 0)
            .OrderBy(s => s.Number)
            .Take(maximumCount)
            .ToList();

        int Write()
        {
            var size = SkillRequirementsRef.GetRequiredSize(skills.Count);
            var span = connection.Output.GetSpan(size)[..size];
            var packet = new SkillRequirementsRef(span)
            {
                SkillCount = (ushort)skills.Count,
            };

            for (var i = 0; i < skills.Count; i++)
            {
                var skill = skills[i];
                var target = packet[i];
                target.SkillNumber = (ushort)skill.Number;
                target.Level = GetValue(skill.Requirements, Stats.Level);
                target.Energy = GetValue(skill.Requirements, Stats.TotalEnergy);
                target.Leadership = GetValue(skill.Requirements, Stats.TotalLeadership);
                target.Strength = GetValue(skill.Requirements, Stats.TotalStrength);
                target.Agility = GetValue(skill.Requirements, Stats.TotalAgility);
                target.Mana = GetValue(skill.ConsumeRequirements, Stats.CurrentMana);
                target.AbilityGauge = GetValue(skill.ConsumeRequirements, Stats.CurrentAbility);
            }

            return size;
        }

        return connection.SendAsync(Write);
    }

    private static ValueTask SendItemsAsync(IConnection connection, GameConfiguration configuration)
    {
        var maximumCount = (ushort.MaxValue - LearnableItemRequirementsRef.GetRequiredSize(0)) / LearnableItemRequirementsRef.LearnableItemRequirementRef.Length;
        var entries = GetLearnableItems(configuration).Take(maximumCount).ToList();

        int Write()
        {
            var size = LearnableItemRequirementsRef.GetRequiredSize(entries.Count);
            var span = connection.Output.GetSpan(size)[..size];
            var packet = new LearnableItemRequirementsRef(span)
            {
                ItemCount = (ushort)entries.Count,
            };

            for (var i = 0; i < entries.Count; i++)
            {
                var (definition, itemLevel, skill) = entries[i];
                var requirements = LearnableSkillRequirements.GetRequirements(definition, skill);
                var target = packet[i];
                target.Group = definition.Group;
                target.ItemLevel = itemLevel;
                target.Number = (ushort)definition.Number;
                target.Level = GetValue(requirements, Stats.Level);
                target.Energy = GetValue(requirements, Stats.TotalEnergy);
                target.Leadership = GetValue(requirements, Stats.TotalLeadership);
                target.Strength = GetValue(requirements, Stats.TotalStrength);
                target.Agility = GetValue(requirements, Stats.TotalAgility);
                target.SkillNumber = (ushort)skill.Number;
            }

            return size;
        }

        return connection.SendAsync(Write);
    }

    /// <summary>
    /// Gets the items which teach a skill when they're consumed, with the item level they're for.
    /// </summary>
    private static IEnumerable<(ItemDefinition Definition, byte ItemLevel, Skill Skill)> GetLearnableItems(GameConfiguration configuration)
    {
        foreach (var definition in configuration.Items.Where(d => d.Skill is not null && d.ItemSlot is null).OrderBy(d => d.Group).ThenBy(d => d.Number))
        {
            if (!LearnableSkillRequirements.IsSummoningOrb(definition))
            {
                yield return (definition, AnyItemLevel, definition.Skill!);
                continue;
            }

            for (var level = 0; level <= definition.MaximumItemLevel && level < AnyItemLevel; level++)
            {
                if (LearnableSkillRequirements.GetLearnableSkill(definition, (byte)level, configuration) is { } skill)
                {
                    yield return (definition, (byte)level, skill);
                }
            }
        }
    }

    private static ushort GetValue(IEnumerable<AttributeRequirement> requirements, AttributeDefinition attribute)
    {
        var value = requirements.Where(r => r.Attribute == attribute).Select(r => r.MinimumValue).DefaultIfEmpty(0).Max();
        return (ushort)Math.Clamp(value, 0, ushort.MaxValue);
    }

    private static ushort GetValue(IReadOnlyDictionary<AttributeDefinition, int> requirements, AttributeDefinition attribute)
    {
        return requirements.TryGetValue(attribute, out var value) ? (ushort)Math.Clamp(value, 0, ushort.MaxValue) : (ushort)0;
    }
}

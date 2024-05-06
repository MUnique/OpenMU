// <copyright file="InfinityArrowSkillOnQuestCompletionPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Quests;
using MUnique.OpenMU.GameServer.MessageHandler.Quests;
using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;
using MUnique.OpenMU.Persistence.Initialization.Skills;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update adds the infinity arrow skill as quest reward for 'Gain Hero Status (Muse Elf)'.
/// </summary>
[PlugIn(PlugInName, PlugInDescription)]
[Guid("A78B7540-75AC-494C-9AEC-BC943D929C98")]
public class InfinityArrowSkillOnQuestCompletionPlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Adds infinity arrow skill with quest";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update adds the infinity arrow skill as quest reward for 'Gain Hero Status (Muse Elf)'.";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.InfinityArrowSkillOnQuestCompletion;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2023, 08, 28, 20, 30, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var marlon = gameConfiguration.Monsters.First(m => m.Number == 229);
        var quest = marlon.Quests.First(q => q.Number == 2 && q.Group == QuestConstants.LegacyQuestGroup && q.QualifiedCharacter?.Number == (byte)CharacterClassNumber.MuseElf);

        if (quest.Rewards.Any(r => r.RewardType == QuestRewardType.Skill))
        {
            return;
        }

        var skillReward = context.CreateNew<QuestReward>();
        skillReward.Value = 1;
        skillReward.SkillReward = gameConfiguration.Skills.First(s => s.Number == (short)SkillNumber.InfinityArrow);
        skillReward.RewardType = QuestRewardType.Skill;
        quest.Rewards.Add(skillReward);
    }
}
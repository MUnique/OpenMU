// <copyright file="ScopeExtraProjectilesPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;
using MUnique.OpenMU.Persistence.Initialization.Skills;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update scopes the extra projectiles per skill. The generic pool stays global,
/// but the triple shot mastery gets its own attribute, so its extra projectile only applies
/// to the triple shot skill and no longer leaks into other multi-projectile skills like multi shot.
/// </summary>
[PlugIn]
[Display(Name = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.ScopeExtraProjectilesPlugIn_Name), Description = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.ScopeExtraProjectilesPlugIn_Description), ResourceType = typeof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources))]
[Guid("F2A9C4E7-8B3D-4F1A-9E6C-7D2B5A8C3E41")]
public class ScopeExtraProjectilesPlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Scope extra projectiles per skill";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "Makes the extra projectile of the triple shot mastery apply to the triple shot skill only, while equipping certain (cross)bows keeps boosting every multi-projectile skill.";

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 10, 17, 12, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    public override IEnumerable<UpdateDependency> DependsOn =>
    [
        typeof(FinishElfMasterTreePlugIn).GUID,
        typeof(AddMasterSkillPassivePowerUpsPlugIn).GUID,
    ];

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        this.AddStatIfNotExists(context, gameConfiguration, Stats.TripleShotExtraProjectiles);
        this.AddStatIfNotExists(context, gameConfiguration, Stats.SkillExtraProjectiles);

        var extraProjectiles = Stats.ExtraProjectiles.GetPersistent(gameConfiguration);
        var tripleShotExtraProjectiles = Stats.TripleShotExtraProjectiles.GetPersistent(gameConfiguration);

        // Move the triple shot mastery's bonus from the generic pool to its own attribute.
        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.TripleShotMastery)?.MasterDefinition is { } tripleShotMastery)
        {
            if (tripleShotMastery.TargetAttribute?.Id == extraProjectiles.Id)
            {
                tripleShotMastery.TargetAttribute = tripleShotExtraProjectiles;
            }

            foreach (var powerUp in tripleShotMastery.PassivePowerUps.Where(p => p.TargetAttribute?.Id == extraProjectiles.Id))
            {
                powerUp.TargetAttribute = tripleShotExtraProjectiles;
            }
        }

        // Skills opt in to the pools they use.
        this.AddRelationshipIfMissing(context, gameConfiguration, SkillNumber.TripleShot, Stats.SkillExtraProjectiles, Stats.ExtraProjectiles);
        this.AddRelationshipIfMissing(context, gameConfiguration, SkillNumber.TripleShot, Stats.SkillExtraProjectiles, Stats.TripleShotExtraProjectiles);
        this.AddRelationshipIfMissing(context, gameConfiguration, SkillNumber.MultiShot, Stats.SkillExtraProjectiles, Stats.ExtraProjectiles);

        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Adds the attribute relationship to the skill, if it doesn't exist yet.
    /// </summary>
    /// <param name="context">The persistence context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <param name="skillNumber">The skill.</param>
    /// <param name="targetAttribute">The target attribute of the relationship.</param>
    /// <param name="sourceAttribute">The source attribute of the relationship.</param>
    private void AddRelationshipIfMissing(IContext context, GameConfiguration gameConfiguration, SkillNumber skillNumber, AttributeDefinition targetAttribute, AttributeDefinition sourceAttribute)
    {
        var skill = gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)skillNumber);
        if (skill is null
            || skill.AttributeRelationships.Any(r => r.TargetAttribute?.Id == targetAttribute.Id && r.InputAttribute?.Id == sourceAttribute.Id))
        {
            return;
        }

        skill.AttributeRelationships.Add(CharacterClassHelper.CreateAttributeRelationship(context, gameConfiguration, targetAttribute, 1, sourceAttribute));
    }
}

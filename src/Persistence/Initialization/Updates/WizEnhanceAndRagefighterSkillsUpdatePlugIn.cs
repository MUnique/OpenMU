// <copyright file="WizEnhanceAndRagefighterSkillsUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Skills;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update sets the right settings for the chain lightning skill.
/// </summary>
[PlugIn(PlugInName, PlugInDescription)]
[Guid("426497DB-A1D7-4EC5-BE6A-C8AEABC288E2")]
public class WizEnhanceAndRagefighterSkillsUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Some added skills";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update adds the following skills: Wizardry Enhance, Increase Health, Increase Block, Ignore Defense.";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.WizEnhanceAndRagefighterSkills;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2024, 07, 20, 12, 00, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        if (gameConfiguration.MagicEffects.Any(m => m.Number == (int)MagicEffectNumber.WizEnhance))
        {
            return;
        }

        // First, we add the required effects:
        new IgnoreDefenseEffectInitializer(context, gameConfiguration).Initialize();
        new WizardryEnhanceEffectInitializer(context, gameConfiguration).Initialize();
        new IncreaseHealthEffectInitializer(context, gameConfiguration).Initialize();
        new IncreaseBlockEffectInitializer(context, gameConfiguration).Initialize();

        this.UpdateWizardryEnhance(gameConfiguration);
        this.UpdateIncreaseHealth(gameConfiguration);
        this.UpdateIncreaseBlock(gameConfiguration);
        this.UpdateIgnoreDefense(gameConfiguration);
    }

    private void UpdateIgnoreDefense(GameConfiguration gameConfiguration)
    {
        var skill = gameConfiguration.Skills.First(s => s.Number == (int)SkillNumber.IgnoreDefense);
        skill.SkillType = SkillType.Buff;
        skill.Target = SkillTarget.ImplicitPlayer;
        skill.MagicEffectDef = gameConfiguration.MagicEffects.First(m => m.Number == (int)MagicEffectNumber.IgnoreDefense);
    }

    private void UpdateIncreaseBlock(GameConfiguration gameConfiguration)
    {
        var skill = gameConfiguration.Skills.First(s => s.Number == (int)SkillNumber.IncreaseBlock);
        skill.SkillType = SkillType.Buff;
        skill.Target = SkillTarget.ImplicitParty;
        skill.TargetRestriction = SkillTargetRestriction.Player;
        skill.MagicEffectDef = gameConfiguration.MagicEffects.First(m => m.Number == (int)MagicEffectNumber.IncreaseBlock);
    }

    private void UpdateIncreaseHealth(GameConfiguration gameConfiguration)
    {
        var skill = gameConfiguration.Skills.First(s => s.Number == (int)SkillNumber.IncreaseHealth);
        skill.SkillType = SkillType.Buff;
        skill.Target = SkillTarget.ImplicitParty;
        skill.TargetRestriction = SkillTargetRestriction.Player;
        skill.MagicEffectDef = gameConfiguration.MagicEffects.First(m => m.Number == (int)MagicEffectNumber.IncreaseHealth);
    }

    private void UpdateWizardryEnhance(GameConfiguration gameConfiguration)
    {
        var skill = gameConfiguration.Skills.First(s => s.Number == (int)SkillNumber.ExpansionofWizardry);
        skill.SkillType = SkillType.Buff;
        skill.Target = SkillTarget.ImplicitPlayer;
        skill.MagicEffectDef = gameConfiguration.MagicEffects.First(m => m.Number == (int)MagicEffectNumber.WizEnhance);

        var skill2 = gameConfiguration.Skills.First(s => s.Number == (int)SkillNumber.ExpansionofWizStreng);
        skill2.SkillType = SkillType.Buff;
        skill2.Target = SkillTarget.ImplicitPlayer;
        skill2.MagicEffectDef = gameConfiguration.MagicEffects.First(m => m.Number == (int)MagicEffectNumber.WizEnhance);
        skill2.MasterDefinition!.ValueFormula = SkillsInitializer.Formula120Value;
        skill2.MasterDefinition.TargetAttribute = Stats.MaximumWizBaseDmg;
        skill2.MasterDefinition.Aggregation = AggregateType.Multiplicate;

        var skill3 = gameConfiguration.Skills.First(s => s.Number == (int)SkillNumber.ExpansionofWizMas);
        skill3.SkillType = SkillType.Buff;
        skill3.Target = SkillTarget.ImplicitPlayer;
        skill3.MagicEffectDef = gameConfiguration.MagicEffects.First(m => m.Number == (int)MagicEffectNumber.WizEnhance);
        skill3.MasterDefinition!.ValueFormula = SkillsInitializer.Formula120Value;
        skill3.MasterDefinition.TargetAttribute = Stats.CriticalDamageChance;
        skill3.MasterDefinition.Aggregation = AggregateType.Multiplicate;
    }
}
// <copyright file="TargetedSkillDefaultPlugin.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Skills;

using System.Runtime.InteropServices;
using System.Reflection;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.PlugIns;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence;

/// <summary>
/// Action to perform a skill which is explicitly aimed to a target.
/// </summary>
[PlugIn(nameof(TargetedSkillDefaultPlugin), "Default (catch-all) handler for targeted skills")]
[Guid("eb2949fb-5ed2-407e-a4e8-e3015ed5692b")]
public class TargetedSkillDefaultPlugin : TargetedSkillPluginBase
{
    private static readonly bool SummonDiagEnabled =
        string.Equals(Environment.GetEnvironmentVariable("SUMMON_DIAG"), "1", StringComparison.OrdinalIgnoreCase)
        || string.Equals(Environment.GetEnvironmentVariable("SUMMON_DIAG"), "true", StringComparison.OrdinalIgnoreCase)
        || string.Equals(Environment.GetEnvironmentVariable("SUMMON_DIAG"), "yes", StringComparison.OrdinalIgnoreCase);
    private static readonly Dictionary<short, short> SummonSkillToMonsterMapping = new()
    {
        { 30, 26 }, // Goblin
        { 31, 32 }, // Stone Golem
        { 32, 21 }, // Assassin
        { 33, 20 }, // Elite Yeti
        { 34, 10 }, // Dark Knight
        { 35, 150 }, // Bali
        { 36, 151 }, // Soldier
    };

    /// <inheritdoc/>
    public override short Key => 0;

    /// <summary>
    /// Performs the skill.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="target">The target.</param>
    /// <param name="skillId">The skill identifier.</param>
    public override async ValueTask PerformSkillAsync(Player player, IAttackable target, ushort skillId)
    {
        using var loggerScope = player.Logger.BeginScope(this.GetType());

        if (target is null)
        {
            return;
        }

        if (player.Attributes is not { } attributes)
        {
            return;
        }

        if (attributes[Stats.IsStunned] > 0)
        {
            player.Logger.LogWarning($"Probably Hacker - player {player} is attacking in stunned state");
            return;
        }

        var skillEntry = player.SkillList?.GetSkill(skillId);
        var skill = skillEntry?.Skill;
        if (skill is null || skill.SkillType == SkillType.PassiveBoost)
        {
            return;
        }

        var miniGame = player.CurrentMiniGame;
        var inMiniGame = miniGame is { };
        var isBuff = skill.SkillType is SkillType.Buff or SkillType.Regeneration;
        if (player.IsAtSafezone() && !(inMiniGame && isBuff))
        {
            return;
        }

        if (inMiniGame && !miniGame!.IsSkillAllowed(skill, player, target))
        {
            return;
        }

        if (!target.IsActive())
        {
            return;
        }

        if (!target.CheckSkillTargetRestrictions(player, skill))
        {
            return;
        }

        if (!player.IsInRange(target.Position, skill.Range + 2))
        {
            // target position might be out of sync so we send the current coordinates to the client again.
            if (!(target is ISupportWalk { IsWalking: true }))
            {
                await player.InvokeViewPlugInAsync<IObjectMovedPlugIn>(p => p.ObjectMovedAsync(target, MoveType.Instant)).ConfigureAwait(false);
            }

            return;
        }

        if (skill.SkillType == SkillType.SummonMonster && player.Summon is { })
        {
            await player.RemoveSummonAsync().ConfigureAwait(false);
            return;
        }

        // enough mana, ag etc?
        if (!await player.TryConsumeForSkillAsync(skill).ConfigureAwait(false))
        {
            return;
        }

        if (skill.MovesToTarget)
        {
            await player.MoveAsync(target.Position).ConfigureAwait(false);
        }

        if (skill.MovesTarget)
        {
            await target.MoveRandomlyAsync().ConfigureAwait(false);
        }

        var effectApplied = false;
        if (skill.SkillType == SkillType.SummonMonster)
        {
            MonsterDefinition? baseDefinition = null;
            if (SummonSkillToMonsterMapping.TryGetValue(skill.Number, out var monsterNumber)
            && player.GameContext.Configuration.Monsters.FirstOrDefault(m => m.Number == monsterNumber) is { } mappedDefinition)
            {
                baseDefinition = mappedDefinition;
            }

            // Allow MonsterNumber override from plug-in configuration even if the plug-in is not active.
            try
            {
                var type = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .SelectMany(a => a.DefinedTypes)
                    .FirstOrDefault(t => typeof(ISummonConfigurationPlugIn).IsAssignableFrom(t)
                                         && !t.IsAbstract && !t.IsInterface
                                         && this.TryGetSummonKey(t) == skill.Number);

                var typeId = type?.GUID;
                var plugInConfig = typeId is null ? null : player.GameContext.Configuration.PlugInConfigurations.FirstOrDefault(c => c.TypeId == typeId.Value);
                var parsed = plugInConfig?.GetConfiguration<MUnique.OpenMU.GameLogic.PlugIns.ElfSummonSkillConfiguration>(player.GameContext.PlugInManager.CustomConfigReferenceHandler);
                if (parsed is { MonsterNumber: > 0 })
                {
                    var customDef = player.GameContext.Configuration.Monsters.FirstOrDefault(m => m.Number == (short)parsed.MonsterNumber);
                    if (customDef is { })
                    {
                        baseDefinition = customDef;
                        if (SummonDiagEnabled)
                        {
                            player.Logger.LogInformation($"[SUMMON] Override MonsterNumber by config for skill {skill.Number}: {customDef.Designation} ({customDef.Number})");
                        }
                    }
                }
            }
            catch
            {
                // ignore - fall back to default mapping
            }

            // ✅ pedir el plugin “keyed” por skill.Number
            var summonPlugin = player.GameContext.PlugInManager
                .GetStrategy<short, ISummonConfigurationPlugIn>(skill.Number);

            var monsterDefinition = summonPlugin?.CreateSummonMonsterDefinition(player, skill, baseDefinition)
                                    ?? baseDefinition;

            // Apply energy scaling as a fallback (and in addition), so it works even if the plugin is not activated
            monsterDefinition = this.CloneAndScaleSummonDefinition(player, monsterDefinition, skill.Number);

            if (monsterDefinition is not null)
            {
                if (SummonDiagEnabled)
                {
                    player.Logger.LogInformation($"[SUMMON] Creating summon for skill {skill.Number}: {monsterDefinition.Designation} ({monsterDefinition.Number})");
                }
                await player.CreateSummonedMonsterAsync(monsterDefinition).ConfigureAwait(false);
            }
        }
        else
        {
            effectApplied = await this.ApplySkillAsync(player, target, skillEntry!).ConfigureAwait(false);
        }
        await player.ForEachWorldObserverAsync<IShowSkillAnimationPlugIn>(p => p.ShowSkillAnimationAsync(player, target, skill, effectApplied), true).ConfigureAwait(false);
    }

    private MonsterDefinition? CloneAndScaleSummonDefinition(Player player, MonsterDefinition? baseDefinition, short skillNumber)
    {
        if (baseDefinition is null)
        {
            return null;
        }

        var clone = baseDefinition.Clone(player.GameContext.Configuration);

        static void AddAttributeCorrectly(IGameContext context, ICollection<MonsterAttribute>? target, MUnique.OpenMU.AttributeSystem.AttributeDefinition? definition, float value)
        {
            if (target is null || definition is null)
            {
                return;
            }

            // Resolve AttributeDefinition to the instance within the current game configuration, if present.
            var resolvedDef = context.Configuration.Attributes.FirstOrDefault(a => a.Id == definition.Id) ?? definition;

            var collectionType = target.GetType();
            if (collectionType.IsGenericType && collectionType.GetGenericTypeDefinition() == typeof(MUnique.OpenMU.Persistence.CollectionAdapter<,>))
            {
                var efItemType = collectionType.GetGenericArguments()[1];
                if (Activator.CreateInstance(efItemType) is MonsterAttribute efAttr)
                {
                    efAttr.AttributeDefinition = resolvedDef;
                    efAttr.Value = value;
                    target.Add(efAttr);
                    return;
                }
            }

            // Fallback: Plain collection which accepts base type.
            target.Add(new MonsterAttribute { AttributeDefinition = resolvedDef, Value = value });
        }

        // Fallback 1: If attributes are not populated, try to take them from any map spawn which uses the same monster number.
        if (clone.Attributes is null || clone.Attributes.Count == 0)
        {
            try
            {
                var refDef = player.GameContext.Configuration.Maps
                    .SelectMany(m => m.MonsterSpawns)
                    .Select(s => s.MonsterDefinition)
                    .FirstOrDefault(d => d is { } && d.Number == baseDefinition.Number && d.Attributes?.Any() == true);

                if (refDef?.Attributes?.Any() == true)
                {
                    foreach (var a in refDef.Attributes)
                    {
                        AddAttributeCorrectly(player.GameContext, clone.Attributes, a.AttributeDefinition, a.Value);
                    }

                    if (SummonDiagEnabled)
                    {
                        player.Logger.LogInformation($"[SUMMON] Fallback-from-map loaded {clone.Attributes?.Count ?? 0} attributes for monster {refDef.Designation} ({refDef.Number})");
                    }
                }
                else
                {
                    // Try unified cache (loads once from persistence similar to admin panel data source)
                    if (MonsterDefinitionAttributeCache.TryFillAttributes(player.GameContext, baseDefinition.Number, clone) && SummonDiagEnabled)
                    {
                        player.Logger.LogInformation($"[SUMMON] Fallback-from-cache loaded {clone.Attributes?.Count ?? 0} attributes for monster {baseDefinition.Designation} ({baseDefinition.Number})");
                    }
                }
            }
            catch { }
        }

        // Fallback 2: Try to load them from persistence (no cache).
        if (clone.Attributes is null || clone.Attributes.Count == 0)
        {
            try
            {
                using var ctx = player.GameContext.PersistenceContextProvider.CreateNewTypedContext(typeof(MUnique.OpenMU.DataModel.Configuration.MonsterDefinition), useCache: false);
                var all = ctx.GetAsync<MUnique.OpenMU.DataModel.Configuration.MonsterDefinition>().AsTask().GetAwaiter().GetResult();
                var dbDef = all.FirstOrDefault(d => d.Number == baseDefinition.Number);
                if (dbDef?.Attributes?.Any() == true)
                {
                    foreach (var a in dbDef.Attributes)
                    {
                        AddAttributeCorrectly(player.GameContext, clone.Attributes, a.AttributeDefinition, a.Value);
                    }

                    if (SummonDiagEnabled)
                    {
                        player.Logger.LogInformation($"[SUMMON] Fallback loaded {clone.Attributes?.Count ?? 0} attributes for monster {dbDef.Designation} ({dbDef.Number})");
                    }
                }
            }
            catch
            {
                // ignore - we scale what we have
            }
        }

        // Read energy scaling settings (panel-driven). Prefer in-memory map kept in sync by PlugInManager.
        // Defaults if not configured.
        int energyPerStep = 1000;
        float percentPerStep = 0.05f;

        try
        {
            if (MUnique.OpenMU.GameLogic.PlugIns.ElfSummonsConfigCore.Instance.Map.TryGetValue(skillNumber, out var entry))
            {
                if (entry.EnergyPerStep > 0)
                {
                    energyPerStep = entry.EnergyPerStep;
                }

                if (entry.PercentPerStep > 0)
                {
                    percentPerStep = entry.PercentPerStep;
                }
            }
            else
            {
                // Fallback: Read directly from plugin configuration store.
                var type = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .SelectMany(a => a.DefinedTypes)
                    .FirstOrDefault(t => typeof(ISummonConfigurationPlugIn).IsAssignableFrom(t)
                                         && !t.IsAbstract && !t.IsInterface
                                         && this.TryGetSummonKey(t) == skillNumber);

                var typeId = type?.GUID;
                var plugInConfig = typeId is null ? null : player.GameContext.Configuration.PlugInConfigurations.FirstOrDefault(c => c.TypeId == typeId.Value);
                if (plugInConfig is not null)
                {
                    var parsed = plugInConfig.GetConfiguration<MUnique.OpenMU.GameLogic.PlugIns.ElfSummonSkillConfiguration>(player.GameContext.PlugInManager.CustomConfigReferenceHandler);
                    if (parsed is not null)
                    {
                        if (parsed.EnergyPerStep > 0)
                        {
                            energyPerStep = parsed.EnergyPerStep;
                        }

                        if (parsed.PercentPerStep > 0)
                        {
                            percentPerStep = parsed.PercentPerStep;
                        }
                    }
                }
            }
        }
        catch
        {
            // ignore and use defaults
        }

        var energy = player.Attributes?[Stats.TotalEnergy] ?? 0;
        var steps = energyPerStep > 0 ? (int)(energy / energyPerStep) : 0;
        var energyScale = 1.0f + Math.Max(0, steps) * Math.Max(0, percentPerStep);
        if (SummonDiagEnabled)
        {
            player.Logger.LogInformation($"[SUMMON] Energy={energy}, steps={steps}, energyPerStep={energyPerStep}, percentPerStep={percentPerStep}, scale={energyScale:0.###}");
            try
            {
                var count = clone.Attributes?.Count ?? 0;
                var present = clone.Attributes?.Select(a => a.AttributeDefinition?.Designation ?? a.AttributeDefinition?.Id.ToString() ?? "<null>")
                    .Take(10).ToArray() ?? Array.Empty<string>();
                player.Logger.LogInformation($"[SUMMON] AttrCount={count}, sample=[{string.Join(", ", present)}]");
                player.Logger.LogInformation($"[SUMMON] StatIds: MaxHP={Stats.MaximumHealth.Id}, DefBase={Stats.DefenseBase.Id}, PhysMin={Stats.MinimumPhysBaseDmg.Id}, PhysMax={Stats.MaximumPhysBaseDmg.Id}");
            }
            catch { }
        }

        if (Math.Abs(energyScale - 1.0f) < float.Epsilon)
        {
            return clone; // nothing to scale
        }

        float GetValue(MUnique.OpenMU.AttributeSystem.AttributeDefinition stat)
        {
            var attr = clone.Attributes?.FirstOrDefault(a => a.AttributeDefinition == stat);
            return attr?.Value ?? 0;
        }

        void Scale(MUnique.OpenMU.AttributeSystem.AttributeDefinition stat)
        {
            var attr = clone.Attributes?.FirstOrDefault(a => a.AttributeDefinition == stat);
            if (attr is not null)
            {
                attr.Value *= energyScale;
            }
        }

        // HP
        Scale(Stats.MaximumHealth);
        // Defense base
        Scale(Stats.DefenseBase);
        // Damage bases
        Scale(Stats.MinimumPhysBaseDmg);
        Scale(Stats.MaximumPhysBaseDmg);
        Scale(Stats.MinimumWizBaseDmg);
        Scale(Stats.MaximumWizBaseDmg);
        Scale(Stats.MinimumCurseBaseDmg);
        Scale(Stats.MaximumCurseBaseDmg);
        // Rates to reduce MISS and improve survivability
        Scale(Stats.AttackRatePvm);
        Scale(Stats.DefenseRatePvm);
        Scale(Stats.DefenseRatePvp);

        if (SummonDiagEnabled)
        {
            player.Logger.LogInformation(
                $"[SUMMON] Stats after scale: HP={GetValue(Stats.MaximumHealth):0}, DefBase={GetValue(Stats.DefenseBase):0}, " +
                $"PhysMin/Max={GetValue(Stats.MinimumPhysBaseDmg):0}/{GetValue(Stats.MaximumPhysBaseDmg):0}, " +
                $"WizMin/Max={GetValue(Stats.MinimumWizBaseDmg):0}/{GetValue(Stats.MaximumWizBaseDmg):0}, " +
                $"CurseMin/Max={GetValue(Stats.MinimumCurseBaseDmg):0}/{GetValue(Stats.MaximumCurseBaseDmg):0}, " +
                $"AtkRatePvM={GetValue(Stats.AttackRatePvm):0}, DefRatePvM={GetValue(Stats.DefenseRatePvm):0}, DefRatePvP={GetValue(Stats.DefenseRatePvp):0}");
        }

        return clone;
    }

    private short TryGetSummonKey(Type pluginType)
    {
        try
        {
            var instance = Activator.CreateInstance(pluginType);
            var prop = pluginType.GetProperty("Key", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (prop?.GetValue(instance) is short k)
            {
                return k;
            }
        }
        catch
        {
            // ignore
        }

        return short.MinValue;
    }

    /// <summary>
    /// Determines the targets of the skill. It can be overridden by derived classes to provide custom target selection logic.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="targetedTarget">The skill target.</param>
    /// <param name="skill">The skill.</param>
    /// <returns>The list of targets.</returns>
    protected virtual IEnumerable<IAttackable> DetermineTargets(Player player, IAttackable targetedTarget, Skill skill)
    {
        if (skill.Target == SkillTarget.ImplicitPlayer)
        {
            // Include own summon for buffs/regeneration
            if (skill.SkillType is SkillType.Buff or SkillType.Regeneration)
            {
                var list = new List<IAttackable> { player };
                var summon = player.Summon?.Item1;
                if (summon is { IsAlive: true })
                {
                    list.Add(summon);
                }
                return list;
            }

            return player.GetAsEnumerable();
        }

        if (skill.Target == SkillTarget.ImplicitParty)
        {
            if (player.Party != null)
            {
                if (skill.SkillType is SkillType.Buff or SkillType.Regeneration)
                {
                    var result = new List<IAttackable>();
                    foreach (var member in player.Party.PartyList.OfType<Player>())
                    {
                        if (player.Observers.Contains(member))
                        {
                            result.Add(member);
                        }

                        var memberSummon = member.Summon?.Item1;
                        if (memberSummon is { IsAlive: true })
                        {
                            result.Add(memberSummon);
                        }
                    }

                    if (result.Count > 0)
                    {
                        return result;
                    }
                }

                return player.Party.PartyList.OfType<IAttackable>().Where(p => player.Observers.Contains((IWorldObserver)p));
            }

            if (skill.SkillType is SkillType.Buff or SkillType.Regeneration)
            {
                var list = new List<IAttackable> { player };
                var summon = player.Summon?.Item1;
                if (summon is { IsAlive: true })
                {
                    list.Add(summon);
                }
                return list;
            }

            return player.GetAsEnumerable();
        }

        if (skill.Target == SkillTarget.Explicit)
        {
            return targetedTarget.GetAsEnumerable();
        }

        if (skill.Target == SkillTarget.ExplicitWithImplicitInRange)
        {
            if (player.GameContext.PlugInManager.GetStrategy<short, IAreaSkillTargetFilter>(skill.Number) is { } filterPlugin)
            {
                var rotationToTarget = (byte)(player.Position.GetAngleDegreeTo(targetedTarget.Position) / 360.0 * 255.0);
                var attackablesInRange =
                    player.CurrentMap?
                        .GetAttackablesInRange(player.Position, skill.Range)
                        .Where(a => a != player)
                        .Where(a => player.GameContext.Configuration.AreaSkillHitsPlayer || a is NonPlayerCharacter)
                        .Where(a => !a.IsAtSafezone())
                        .Where(a => filterPlugin.IsTargetWithinBounds(player, a, player.Position, rotationToTarget))
                        .ToList();
                if (attackablesInRange is not null)
                {
                    if (!attackablesInRange.Contains(targetedTarget))
                    {
                        attackablesInRange.Add(targetedTarget);
                    }

                    return attackablesInRange;
                }
            }
            else if (skill.ImplicitTargetRange > 0)
            {
                var targetsOfTarget = targetedTarget.CurrentMap?.GetAttackablesInRange(targetedTarget.Position, skill.ImplicitTargetRange) ?? Enumerable.Empty<IAttackable>();
                if (!player.GameContext.Configuration.AreaSkillHitsPlayer && targetedTarget is Monster)
                {
                    return targetsOfTarget.OfType<Monster>();
                }

                return targetsOfTarget;
            }
            else
            {
                // do nothing.
            }

            return targetedTarget.GetAsEnumerable();
        }

        var targets = player.CurrentMap?.GetAttackablesInRange(player.Position, skill.ImplicitTargetRange) ?? Enumerable.Empty<IAttackable>();

        if (skill.Target == SkillTarget.ImplicitAllInRange)
        {
            return targets;
        }

        if (skill.Target == SkillTarget.ImplicitPlayersInRange)
        {
            return targets.OfType<Player>();
        }

        if (skill.Target == SkillTarget.ImplicitNpcsInRange)
        {
            return targets.OfType<Monster>();
        }

        return Enumerable.Empty<IAttackable>();
    }

    private async ValueTask<bool> ApplySkillAsync(Player player, IAttackable targetedTarget, SkillEntry skillEntry)
    {
        skillEntry.ThrowNotInitializedProperty(skillEntry.Skill is null, nameof(skillEntry.Skill));
        var skill = skillEntry.Skill;
        var success = false;
        var targets = this.DetermineTargets(player, targetedTarget, skill);
        bool isCombo = false;
        if (skill.SkillType is SkillType.DirectHit or SkillType.CastleSiegeSkill
            && player.ComboState is { } comboState
            && !targetedTarget.IsAtSafezone()
            && !player.IsAtSafezone()
            && targetedTarget.IsActive()
            && player.IsActive())
        {
            isCombo = await comboState.RegisterSkillAsync(skill).ConfigureAwait(false);
        }

        foreach (var target in targets)
        {
            if (skill.SkillType == SkillType.DirectHit || skill.SkillType == SkillType.CastleSiegeSkill)
            {
                // Block direct hits against own summon (client requires CTRL to send such an action for "basic"),
                // keeping classic behavior. Aggro against owner is blocked in SummonedMonsterIntelligence anyway.
                if (target is Monster { SummonedBy: { } owner } && owner == player)
                {
                    continue;
                }

                if (player.Attributes![Stats.AmmunitionConsumptionRate] > player.Attributes[Stats.AmmunitionAmount])
                {
                    break;
                }

                if (!target.IsAtSafezone() && !player.IsAtSafezone() && target != player)
                {
                    await target.AttackByAsync(player, skillEntry, isCombo).ConfigureAwait(false);
                    player.LastAttackedTarget.SetTarget(target);
                    success = await target.TryApplyElementalEffectsAsync(player, skillEntry).ConfigureAwait(false) || success;
                }
            }
            else if (skill.MagicEffectDef != null)
            {
                // Buffs are allowed in the Safezone of Blood Castle.
                var canDoBuff = !player.IsAtSafezone() || player.CurrentMiniGame is { };
                if (!canDoBuff)
                {
                    player.Logger.LogWarning($"Can't apply magic effect when being in the safezone. skill: {skill.Name} ({skill.Number}), skillType: {skill.SkillType}.");
                    break;
                }

                if (skill.SkillType == SkillType.Buff)
                {
                    await target.ApplyMagicEffectAsync(player, skillEntry).ConfigureAwait(false);
                    success = true;
                }
                else if (skill.SkillType == SkillType.Regeneration)
                {
                    await target.ApplyRegenerationAsync(player, skillEntry).ConfigureAwait(false);
                    success = true;
                }
                else
                {
                    player.Logger.LogWarning($"Skill.MagicEffectDef isn't null, but it's not a buff or regeneration skill. skill: {skill.Name} ({skill.Number}), skillType: {skill.SkillType}.");
                }
            }
            else
            {
                player.Logger.LogWarning($"Skill.MagicEffectDef is null, skill: {skill.Name} ({skill.Number}), skillType: {skill.SkillType}.");
            }
        }

        if (isCombo)
        {
            await player.ForEachWorldObserverAsync<IShowSkillAnimationPlugIn>(p => p.ShowComboAnimationAsync(player, targetedTarget), true).ConfigureAwait(false);
        }

        return success;
    }
}

// <copyright file="FixBloodCastleMonsterAttributesUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Swaps attribute values between BC7 (monsters 138-143) and BC8 (monsters 428-433)
/// so the difficulty progression is correct: BC6 → BC7 → BC8.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("D4E5F6A0-1B2C-3D4E-5F6A-7B8C9D0E1F2A")]
public class FixBloodCastleMonsterAttributesUpdatePlugIn : UpdatePlugInBase
{
    internal const string PlugInName = "Fix Blood Castle 7/8 Monster Attributes";

    internal const string PlugInDescription = "Swaps attribute values between BC7 (monsters 138-143) and BC8 (monsters 428-433) so progression is correct: BC6 → BC7 → BC8.";

    private static readonly Guid LevelId = new("560931AD-0901-4342-B7F4-FD2E2FCC0563");
    private static readonly Guid MaximumHealthId = new("A6C39A5C-295F-415E-A314-5E9F9A748D27");
    private static readonly Guid MinimumPhysBaseDmgId = new("3E8D6A02-E973-4AE4-9DF3-CDDC3D3183B3");
    private static readonly Guid MaximumPhysBaseDmgId = new("8A918EA2-893A-48B2-A684-3E71526CA71F");
    private static readonly Guid DefenseBaseId = new("EB098C46-60D4-4CA6-BBD4-5B6270A1407B");
    private static readonly Guid AttackRatePvmId = new("1129442A-E1C7-4240-8866-B781C2838C25");
    private static readonly Guid DefenseRatePvmId = new("C520DD2D-1B06-4392-95EE-3C41F33E68DA");
    private static readonly Guid PoisonResistanceId = new("3D50D0B7-63A2-4DA9-8855-12173EAE6B39");
    private static readonly Guid IceResistanceId = new("47235C36-41BB-44B4-8823-6FC415709F59");
    private static readonly Guid FireResistanceId = new("9AE4D80D-5706-48B9-AD11-EAC4FE088A81");
    private static readonly Guid LightningResistanceId = new("3E339393-2D17-452E-81D9-3987947A407F");

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.FixBloodCastleMonsterAttributes;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => false;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 05, 17, 0, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var monsters = gameConfiguration.Monsters;

        UpdateMonster(monsters, 138, 105, 29000, 475, 510, 440, 570, 250, 7f / 255, 7f / 255, 7f / 255, 7f / 255);
        UpdateMonster(monsters, 139, 106, 32000, 510, 555, 480, 640, 260, 7f / 255, 7f / 255, 7f / 255, 7f / 255);
        UpdateMonster(monsters, 140, 110, 37000, 600, 650, 500, 710, 300, 7f / 255, 7f / 255, 7f / 255, 7f / 255);
        UpdateMonster(monsters, 141, 112, 45000, 645, 690, 540, 780, 310, 7f / 255, 7f / 255, 7f / 255, 7f / 255);
        UpdateMonster(monsters, 142, 119, 55000, 780, 820, 600, 850, 360, 7f / 255, 7f / 255, 7f / 255, 7f / 255);
        UpdateMonster(monsters, 143, 125, 60000, 830, 865, 680, 920, 370, 10f / 255, 10f / 255, 10f / 255, 10f / 255);

        UpdateMonster(monsters, 428, 114, 173500, 745, 800, 600, 640, 426, 8f / 255, 8f / 255, 8f / 255, 8f / 255);
        UpdateMonster(monsters, 429, 117, 175000, 825, 872, 615, 690, 440, 8f / 255, 8f / 255, 8f / 255, 8f / 255);
        UpdateMonster(monsters, 430, 125, 184000, 890, 915, 622, 760, 465, 8f / 255, 8f / 255, 8f / 255, 8f / 255);
        UpdateMonster(monsters, 431, 129, 208000, 920, 946, 635, 830, 510, 8f / 255, 8f / 255, 8f / 255, 8f / 255);
        UpdateMonster(monsters, 432, 132, 208700, 995, 1120, 648, 900, 585, 8f / 255, 8f / 255, 8f / 255, 8f / 255);
        UpdateMonster(monsters, 433, 140, 215000, 1500, 1780, 690, 950, 750, 11f / 255, 11f / 255, 11f / 255, 11f / 255);

        return ValueTask.CompletedTask;
    }

    private static void UpdateMonster(ICollection<MonsterDefinition> monsters, short number, int level, int maxHealth, int minDmg, int maxDmg, int defense, int attackRate, int defenseRate, float poisonRes, float iceRes, float fireRes, float lightningRes)
    {
        var monster = monsters.FirstOrDefault(m => m.Number == number);
        if (monster is null)
        {
            return;
        }

        SetAttribute(monster, LevelId, level);
        SetAttribute(monster, MaximumHealthId, maxHealth);
        SetAttribute(monster, MinimumPhysBaseDmgId, minDmg);
        SetAttribute(monster, MaximumPhysBaseDmgId, maxDmg);
        SetAttribute(monster, DefenseBaseId, defense);
        SetAttribute(monster, AttackRatePvmId, attackRate);
        SetAttribute(monster, DefenseRatePvmId, defenseRate);
        SetAttribute(monster, PoisonResistanceId, poisonRes);
        SetAttribute(monster, IceResistanceId, iceRes);
        SetAttribute(monster, FireResistanceId, fireRes);
        SetAttribute(monster, LightningResistanceId, lightningRes);
    }

    private static void SetAttribute(MonsterDefinition monster, Guid attributeId, float value)
    {
        var attribute = monster.Attributes.FirstOrDefault(a => a.AttributeDefinition?.Id == attributeId);
        if (attribute is not null)
        {
            attribute.Value = value;
        }
    }
}

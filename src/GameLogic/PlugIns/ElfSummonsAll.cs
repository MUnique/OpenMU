// ElfSummonsAll.cs
namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Linq;
using System.Runtime.InteropServices;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.PlugIns;
using System.Text.Json;
using System.Text.Json.Serialization;

public sealed class NullToZeroIntConverter : JsonConverter<int>
{
    public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.TokenType == JsonTokenType.Null ? 0 : reader.GetInt32();

    public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
        => writer.WriteNumberValue(value);
}

#region Core

public sealed class ElfSummonsConfigCore
{
    public static readonly ElfSummonsConfigCore Instance = new();

    // null => no reemplaza (usa mapping por defecto del server)
    public System.Collections.Generic.Dictionary<short, SummonConfig> Map { get; }
        = new()
        {
            [30] = new SummonConfig { MonsterNumber = null },
            [31] = new SummonConfig { MonsterNumber = null },
            [32] = new SummonConfig { MonsterNumber = null },
            [33] = new SummonConfig { MonsterNumber = null },
            [34] = new SummonConfig { MonsterNumber = null },
            [35] = new SummonConfig { MonsterNumber = null },
            [36] = new SummonConfig { MonsterNumber = null },
        };

    private ElfSummonsConfigCore() { }

    public MonsterDefinition? Resolve(Player player, Skill skill, MonsterDefinition? defaultDefinition)
    {
        if (!this.Map.TryGetValue(skill.Number, out var cfg))
        {
            return null; // sin configuración -> usa mapping por defecto
        }

        // Seleccionamos la definición base: o la configurada, o la por defecto del servidor.
        MonsterDefinition? baseDef;
        if (cfg.MonsterNumber is null)
        {
            // Mantener mapeo por defecto, pero permitir modificar stats del defaultDefinition
            baseDef = defaultDefinition;
        }
        else
        {
            baseDef = player.GameContext.Configuration.Monsters
                .FirstOrDefault(m => m.Number == cfg.MonsterNumber.Value)
                      ?? defaultDefinition; // fallback al default si no existe la custom
        }

        if (baseDef is null)
        {
            // No hay nada que clonar; dejamos que el handler use su flujo por defecto
            return null;
        }

        // Only pick/clone the base monster definition here. Energy scaling is centralized
        // in TargetedSkillDefaultPlugin so it applies even when this plug-in is inactive.
        var clone = baseDef.Clone(player.GameContext.Configuration);
        cfg.Customize?.Invoke(clone);
        return clone;
    }

    public sealed class SummonConfig
    {
        public ushort? MonsterNumber { get; set; }
        // Dynamic scaling by Energy only: scale = 1 + floor(Energy / EnergyPerStep) * PercentPerStep
        public int EnergyPerStep { get; set; } = 1000; // default: per 1000 Energy
        public float PercentPerStep { get; set; } = 0.05f; // default: +5% per step
        public System.Action<MonsterDefinition>? Customize { get; set; }
    }
}

#endregion

#region POCO de configuración (lo que edita el ⚙️)

// Usamos int no-nullable para que el Admin lo renderice.
// 0 = usar mapeo por defecto del server.
public class ElfSummonSkillConfiguration
{
    [JsonConverter(typeof(NullToZeroIntConverter))]
    [System.ComponentModel.DisplayName("Monster Number (0 = default)")]
    [System.ComponentModel.DataAnnotations.Range(0, 65535)]
    public int MonsterNumber { get; set; } = 0;
    // Dynamic scaling by Energy: scale = 1 + floor(Energy / EnergyPerStep) * PercentPerStep
    public int EnergyPerStep { get; set; } = 1000; // default enabled: per 1000 energy
    public float PercentPerStep { get; set; } = 0.05f; // default: +5% per step
}


#endregion

#region Base configurable

public abstract class ElfSummonCfgBase :
    ISummonConfigurationPlugIn,
    ISupportCustomConfiguration<ElfSummonSkillConfiguration>,
    ISupportDefaultCustomConfiguration
{
    public abstract short Key { get; }

    private ElfSummonSkillConfiguration? _configuration;

    public ElfSummonSkillConfiguration? Configuration
    {
        get => _configuration;
        set
        {
            _configuration = value;
            if (value is null)
            {
                return;
            }

            var entry = ElfSummonsConfigCore.Instance.Map[this.Key];

            // Convertimos int => ushort? (0 = default/null). Validamos rango.
            if (value.MonsterNumber <= 0)
            {
                entry.MonsterNumber = null;
            }
            else
            {
                var clamped = value.MonsterNumber > ushort.MaxValue
                    ? ushort.MaxValue
                    : (ushort)value.MonsterNumber;

                entry.MonsterNumber = clamped;
            }

            entry.EnergyPerStep = value.EnergyPerStep > 0 ? value.EnergyPerStep : 1000;
            entry.PercentPerStep = value.PercentPerStep > 0 ? value.PercentPerStep : 0.05f;
        }
    }

    public object CreateDefaultConfig()
    {
        var entry = ElfSummonsConfigCore.Instance.Map[this.Key];
        return new ElfSummonSkillConfiguration
        {
            MonsterNumber = entry.MonsterNumber.HasValue ? entry.MonsterNumber.Value : 0,
            EnergyPerStep = entry.EnergyPerStep,
            PercentPerStep = entry.PercentPerStep,
        };
    }

    public MonsterDefinition? CreateSummonMonsterDefinition(Player player, Skill skill, MonsterDefinition? defaultDefinition)
    {
        // Best-effort: Pull latest configuration directly from persistence, bypassing caches,
        // so cross-process/container changes apply without restart.
        try
        {
            var typeId = this.GetType().GUID;
            using var cfgCtx = player.GameContext.PersistenceContextProvider.CreateNewTypedContext(typeof(PlugInConfiguration), useCache: false);
            var all = cfgCtx.GetAsync<PlugInConfiguration>().AsTask().GetAwaiter().GetResult();
            var plugInConfig = all.FirstOrDefault(c => c.TypeId == typeId);
            if (plugInConfig is not null)
            {
                var latest = plugInConfig.GetConfiguration<ElfSummonSkillConfiguration>(player.GameContext.PlugInManager.CustomConfigReferenceHandler);
                if (latest is not null)
                {
                    this.Configuration = latest; // updates core map
                }
            }
        }
        catch
        {
            // ignore and continue with current in-memory config
        }

        return ElfSummonsConfigCore.Instance.Resolve(player, skill, defaultDefinition);
    }
}

#endregion

#region Wrappers (30..36)

[Guid("A6E7C6A1-5D9A-4D7D-A001-000000000030")]
[PlugIn("Elf Summon cfg — Goblin (30)", "Configurable summon (Goblin)")]
public sealed class ElfSummonCfg30 : ElfSummonCfgBase { public override short Key => 30; }

[Guid("A6E7C6A1-5D9A-4D7D-A001-000000000031")]
[PlugIn("Elf Summon cfg — Stone Golem (31)", "Configurable summon (Stone Golem)")]
public sealed class ElfSummonCfg31 : ElfSummonCfgBase { public override short Key => 31; }

[Guid("A6E7C6A1-5D9A-4D7D-A001-000000000032")]
[PlugIn("Elf Summon cfg — Assassin (32)", "Configurable summon (Assassin)")]
public sealed class ElfSummonCfg32 : ElfSummonCfgBase { public override short Key => 32; }

[Guid("A6E7C6A1-5D9A-4D7D-A001-000000000033")]
[PlugIn("Elf Summon cfg — Elite Yeti (33)", "Configurable summon (Elite Yeti)")]
public sealed class ElfSummonCfg33 : ElfSummonCfgBase { public override short Key => 33; }

[Guid("A6E7C6A1-5D9A-4D7D-A001-000000000034")]
[PlugIn("Elf Summon cfg — Dark Knight (34)", "Configurable summon (Dark Knight)")]
public sealed class ElfSummonCfg34 : ElfSummonCfgBase { public override short Key => 34; }

[Guid("A6E7C6A1-5D9A-4D7D-A001-000000000035")]
[PlugIn("Elf Summon cfg — Bali (35)", "Configurable summon (Bali)")]
public sealed class ElfSummonCfg35 : ElfSummonCfgBase { public override short Key => 35; }

[Guid("A6E7C6A1-5D9A-4D7D-A001-000000000036")]
[PlugIn("Elf Summon cfg — Soldier (36)", "Configurable summon (Soldier)")]
public sealed class ElfSummonCfg36 : ElfSummonCfgBase { public override short Key => 36; }

#endregion

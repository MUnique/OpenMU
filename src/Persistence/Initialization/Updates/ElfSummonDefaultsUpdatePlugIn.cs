// This update sets sensible defaults for Elf Summon configuration
// on already-initialized servers: EnergyPerStep=1000 and PercentPerStep=0.05

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using System.Text.Json;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.PlugIns;

[PlugIn(PlugInName, PlugInDescription)]
[Guid("4C92E2E8-1E5B-4E1D-B2B4-0D5A29B87C42")]
public class ElfSummonDefaultsUpdatePlugIn : UpdatePlugInBase
{
    internal const string PlugInName = "Elf Summon energy scaling defaults";
    internal const string PlugInDescription = "Sets default EnergyPerStep=1000 and PercentPerStep=0.05 for Elf Summon configuration (skills 30..36).";

    // TypeIds of ElfSummonCfg30..36 (see ElfSummonsAll.cs)
    private static readonly Guid[] ElfSummonTypeIds =
    [
        new("A6E7C6A1-5D9A-4D7D-A001-000000000030"),
        new("A6E7C6A1-5D9A-4D7D-A001-000000000031"),
        new("A6E7C6A1-5D9A-4D7D-A001-000000000032"),
        new("A6E7C6A1-5D9A-4D7D-A001-000000000033"),
        new("A6E7C6A1-5D9A-4D7D-A001-000000000034"),
        new("A6E7C6A1-5D9A-4D7D-A001-000000000035"),
        new("A6E7C6A1-5D9A-4D7D-A001-000000000036"),
    ];

    public override UpdateVersion Version => UpdateVersion.ElfSummonDefaults;

    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    public override string Name => PlugInName;

    public override string Description => PlugInDescription;

    public override bool IsMandatory => false;

    public override DateTime CreatedAt => new(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        foreach (var cfg in gameConfiguration.PlugInConfigurations.Where(c => ElfSummonTypeIds.Contains(c.TypeId)))
        {
            cfg.CustomConfiguration = UpdateJson(cfg.CustomConfiguration);
        }

        return ValueTask.CompletedTask;
    }

    private static string UpdateJson(string? json)
    {
        int monsterNumber = 0;
        int energyPerStep = 1000;
        float percentPerStep = 0.05f;

        try
        {
            if (!string.IsNullOrWhiteSpace(json))
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;
                if (root.TryGetProperty("MonsterNumber", out var m))
                {
                    monsterNumber = m.GetInt32();
                }
                if (root.TryGetProperty("EnergyPerStep", out var eps))
                {
                    var val = eps.GetInt32();
                    if (val > 0)
                    {
                        energyPerStep = val;
                    }
                }
                if (root.TryGetProperty("PercentPerStep", out var pps))
                {
                    var val = pps.GetSingle();
                    if (val > 0)
                    {
                        percentPerStep = val;
                    }
                }
            }
        }
        catch
        {
            // ignore and write defaults
        }

        var result = new
        {
            MonsterNumber = monsterNumber,
            EnergyPerStep = energyPerStep,
            PercentPerStep = percentPerStep,
        };

        return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
    }
}

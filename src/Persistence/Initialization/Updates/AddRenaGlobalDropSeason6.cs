namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.PlugIns;

[PlugIn(PlugInName, PlugInDescription)]
[Guid("8C9A1C3D-09B8-4E2E-9A0A-6B9A9C8E2F31")]
public class AddRenaGlobalDropSeason6 : AddRenaGlobalDropBase
{
    internal const string PlugInName = "Add Rena Global Drop (Season 6)";
    internal const string PlugInDescription = "Adds a 0.2% Rena drop to all Season 1 maps (Season 6 data).";

    public override UpdateVersion Version => UpdateVersion.RenaGlobalDropSeason6;

    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    public override string Name => PlugInName;

    public override string Description => PlugInDescription;

    public override bool IsMandatory => false;

    public override DateTime CreatedAt => new(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        this.EnsureRenaExists(context, gameConfiguration);
        var drop = this.EnsureRenaDropGroup(context, gameConfiguration);
        this.AttachToSeason1Maps(gameConfiguration, drop);
        return ValueTask.CompletedTask;
    }
}


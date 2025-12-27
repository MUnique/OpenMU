namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.PlugIns;

[PlugIn(PlugInName, PlugInDescription)]
[Guid("B5C6B3F4-4DF3-4C71-8C7B-8E6F6B3E4B70")]
public class AddRenaGlobalDrop095d : AddRenaGlobalDropBase
{
    internal const string PlugInName = "Add Rena Global Drop (0.95d)";
    internal const string PlugInDescription = "Adds a 0.2% Rena drop to all Season 1 maps (Version 0.95d).";

    public override UpdateVersion Version => UpdateVersion.RenaGlobalDrop095d;

    public override string DataInitializationKey => Version095d.DataInitialization.Id;

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


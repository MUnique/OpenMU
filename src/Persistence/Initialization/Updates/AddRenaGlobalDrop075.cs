namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.PlugIns;

[PlugIn(PlugInName, PlugInDescription)]
[Guid("E8FD2A8E-7BBF-4A62-9D13-7C1E57B1E675")]
public class AddRenaGlobalDrop075 : AddRenaGlobalDropBase
{
    internal const string PlugInName = "Add Rena Global Drop (0.75)";
    internal const string PlugInDescription = "Adds a 0.2% Rena drop to all Season 1 maps (Version 0.75).";

    public override UpdateVersion Version => UpdateVersion.RenaGlobalDrop075;

    public override string DataInitializationKey => Version075.DataInitialization.Id;

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


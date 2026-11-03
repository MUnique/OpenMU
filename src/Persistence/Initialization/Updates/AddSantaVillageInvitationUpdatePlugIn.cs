// <copyright file="AddSantaVillageInvitationUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update adds the invitation to the santa village and a spawn gate to the santa village,
/// where the invitation warps the player to.
/// </summary>
[PlugIn]
[Display(Name = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.AddSantaVillageInvitationUpdatePlugIn_Name), Description = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.AddSantaVillageInvitationUpdatePlugIn_Description), ResourceType = typeof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources))]
[Guid("0C5DDB62-C6A7-4D54-8E6A-DB6831CB3366")]
public class AddSantaVillageInvitationUpdatePlugIn : UpdatePlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInResources.AddSantaVillageInvitationUpdatePlugIn_Name;

    /// <inheritdoc />
    public override string Description => PlugInResources.AddSantaVillageInvitationUpdatePlugIn_Description;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 11, 02, 16, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        new EventTicketItems(context, gameConfiguration).CreateSantaVillageInvitation();
        AddSpawnGate(context, gameConfiguration);
        return ValueTask.CompletedTask;
    }

    private static void AddSpawnGate(IContext context, GameConfiguration gameConfiguration)
    {
        if (gameConfiguration.Maps.FirstOrDefault(map => map.Number == SantaVillage.Number) is not { } santaVillage
            || santaVillage.ExitGates.Any(gate => gate.IsSpawnGate))
        {
            return;
        }

        var gate = context.CreateNew<ExitGate>();
        gate.Map = santaVillage;
        gate.X1 = Gates.SantaVillageSpawnGate.X1;
        gate.Y1 = Gates.SantaVillageSpawnGate.Y1;
        gate.X2 = Gates.SantaVillageSpawnGate.X2;
        gate.Y2 = Gates.SantaVillageSpawnGate.Y2;
        gate.IsSpawnGate = true;
        santaVillage.ExitGates.Add(gate);
    }
}

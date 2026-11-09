// <copyright file="AddLuckyCoinUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update adds the lucky coin, opens the lucky coin dialog at the NPC Delgado,
/// and adds the attribute which counts the registered coins of a character.
/// </summary>
[PlugIn]
[Display(Name = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.AddLuckyCoinUpdatePlugIn_Name), Description = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.AddLuckyCoinUpdatePlugIn_Description), ResourceType = typeof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources))]
[Guid("DC8775D2-78BD-4DB3-AB51-FE84B9DDF7A2")]
public class AddLuckyCoinUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Add the lucky coin";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update adds the lucky coin, which is registered or exchanged for items at the NPC Delgado in Devias, and opens his lucky coin dialog.";

    /// <summary>
    /// The number of the NPC Delgado.
    /// </summary>
    private const short DelgadoNumber = 478;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 11, 09, 14, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        if (gameConfiguration.Attributes.All(a => a.Id != Stats.RegisteredLuckyCoins.Id))
        {
            gameConfiguration.Attributes.Add(context.CreateNew<AttributeDefinition>(Stats.RegisteredLuckyCoins.Id, Stats.RegisteredLuckyCoins.Designation, Stats.RegisteredLuckyCoins.Description));
        }

        new Misc(context, gameConfiguration).CreateLuckyCoin();

        if (gameConfiguration.Monsters.FirstOrDefault(m => m.Number == DelgadoNumber) is { NpcWindow: NpcWindow.Undefined } delgado)
        {
            delgado.NpcWindow = NpcWindow.DelgadoLuckyCoinRegistration;
        }

        return ValueTask.CompletedTask;
    }
}

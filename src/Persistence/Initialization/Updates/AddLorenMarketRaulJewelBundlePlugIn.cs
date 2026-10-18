// <copyright file="AddLorenMarketRaulJewelBundlePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update makes Jeweler Raul (546) of the Loren Market open the jewel bundle window, like Lahap,
/// instead of doing nothing when talking to him.
/// </summary>
/// <remarks>
/// Combining and disbanding jewel bundles is handled by the <c>JewelMixHandlerPlugIn</c>, which doesn't depend on a specific NPC.
/// A window which was already configured for Raul is kept.
/// </remarks>
[PlugIn]
[Display(Name = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.AddLorenMarketRaulJewelBundlePlugIn_Name), Description = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.AddLorenMarketRaulJewelBundlePlugIn_Description), ResourceType = typeof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources))]
[Guid("e7b6839b-c932-4b28-8ffd-f7b9658b1ce3")]
public class AddLorenMarketRaulJewelBundlePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug-in name.
    /// </summary>
    internal const string PlugInName = "Add Loren Market Jeweler Raul jewel bundles";

    /// <summary>
    /// The plug-in description.
    /// </summary>
    internal const string PlugInDescription = "This update lets Jeweler Raul (546) of the Loren Market open the jewel bundle window, to combine jewels into bundles and disband them, like Lahap.";

    private const short RaulNpcNumber = 546;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override bool IsMandatory => false;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 10, 10, 12, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        if (gameConfiguration.Monsters.FirstOrDefault(monster => monster.Number == RaulNpcNumber) is { } raul
            && raul.NpcWindow == NpcWindow.Undefined
            && raul.MerchantStore is null)
        {
            raul.NpcWindow = NpcWindow.Lahap;
        }

        return ValueTask.CompletedTask;
    }
}

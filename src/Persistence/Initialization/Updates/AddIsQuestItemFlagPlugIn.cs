// <copyright file="AddIsQuestItemFlagPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update sets the new <see cref="ItemDefinition.IsQuestItem"/> flag on the existing quest items,
/// so that pick-up eligibility can be checked against a character's active quests instead of relying on
/// <see cref="ItemDefinition.IsBoundToCharacter"/> alone, which is also used by non-quest items.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("9374E428-CF5C-44B1-AAB9-0369C77AF7C6")]
public class AddIsQuestItemFlagPlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Add IsQuestItem flag";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update flags the existing quest items as such, so that they can only be picked up by characters with a matching active quest.";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.AddIsQuestItemFlag;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 07, 10, 20, 50, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var hashSet = new HashSet<short>
        {
            Quest.BrokenSwordNumber,
            Quest.EyeOfAbyssalNumber,
            Quest.FeatherOfDarkPhoenixNumber,
            Quest.FlameOfDeathBeamKnightNumber,
            Quest.HornOfHellMaineNumber,
            Quest.ScrollOfEmperorNumber,
            Quest.SoulShardOfWizardNumber,
            Quest.TearOfElfNumber,
        };
        var questItems = gameConfiguration.Items.Where(item => item.Group == 14 && hashSet.Contains(item.Number));
        foreach (var item in questItems)
        {
            item.IsQuestItem = true;
        }
    }
}
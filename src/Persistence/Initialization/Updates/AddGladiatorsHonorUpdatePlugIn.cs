// <copyright file="AddGladiatorsHonorUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update adds the Gladiator's Honor with its effect, which increases the experience in Vulcanus.
/// The attribute of its effect is added by the <see cref="BuffItems"/>, when it doesn't exist yet.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.AddGladiatorsHonorUpdatePlugIn_Name), Description = nameof(PlugInResources.AddGladiatorsHonorUpdatePlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("86AE7CBA-FA9B-4D1B-B112-C7FA87BE4B37")]
public class AddGladiatorsHonorUpdatePlugIn : UpdatePlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInResources.AddGladiatorsHonorUpdatePlugIn_Name;

    /// <inheritdoc />
    public override string Description => PlugInResources.AddGladiatorsHonorUpdatePlugIn_Description;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 11, 10, 6, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    /// <remarks>
    /// The item gets the item rule flags, and it's created by the initializer of the buff items.
    /// </remarks>
    public override IEnumerable<UpdateDependency> DependsOn => [typeof(AddItemRuleFlagsPlugIn).GUID, typeof(AddBuffItemsUpdatePlugIn).GUID];

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var addedItems = new BuffItems(context, gameConfiguration).CreateItems();
        ItemRules.Apply(addedItems);
        return ValueTask.CompletedTask;
    }
}

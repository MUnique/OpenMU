// <copyright file="ItemDurabilityRefactorPlugIn075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// This update resets some game configuration values which are used for item durability reduction.
/// </summary>
public abstract class ItemDurabilityRefactorPlugIn075 : ItemDurabilityRefactorPlugInBase
{
    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.ItemDurabilityRefactor075;

    /// <inheritdoc />
    public override string DataInitializationKey => Version075.DataInitialization.Id;

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        await base.ApplyAsync(context, gameConfiguration).ConfigureAwait(false);
    }
}

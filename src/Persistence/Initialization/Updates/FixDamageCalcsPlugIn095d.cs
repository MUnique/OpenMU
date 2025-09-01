// <copyright file="FixDamageCalcsPlugIn095d.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update fixes character stats, skills, magic effects, items, and options related to damage.
/// </summary>
[PlugIn(PlugInName, PlugInDescription)]
[Guid("A4410B7B-7E5F-409C-9F6F-4E216208829A")]
public class FixDamageCalcsPlugIn095D : FixDamageCalcsPlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => Version095d.DataInitialization.Id;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.FixDamageCalcs095d;

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        await base.ApplyAsync(context, gameConfiguration).ConfigureAwait(false);
        this.UpdateExcellentOptions(gameConfiguration);
        this.UpdateAmmoItems(context, gameConfiguration, [0, 0.03f, 0.05f]);
        this.UpdateWeaponItems(context, gameConfiguration);
        this.AddDinorantBasePowerUp(context, gameConfiguration);
        this.UpdateMGStaffsItemSlot(gameConfiguration);
    }
}
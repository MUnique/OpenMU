// <copyright file="FixItemOptionsAndAttackSpeedPlugIn095d.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update fixes some item options (damage, defense rate) and weapons attack speed.
/// It also refactors attack speed attributes for simplification.
/// </summary>
[PlugIn(PlugInName, PlugInDescription)]
[Guid("C7F90EDB-EC00-467D-826F-9DEFFEA1206A")]
public class FixItemOptionsAndAttackSpeedPlugIn095D : FixItemOptionsAndAttackSpeedPlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => Version095d.DataInitialization.Id;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.FixItemOptionsAndAttackSpeed095d;

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        await base.ApplyAsync(context, gameConfiguration).ConfigureAwait(false);
        this.FixWeaponsAttackSpeedStat(gameConfiguration);
        this.ChangeDinorantAttackSpeedOption(gameConfiguration);
        this.UpdateExcellentAttackSpeedAndBaseDmgOptions(gameConfiguration);
    }
}
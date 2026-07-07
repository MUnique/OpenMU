// <copyright file="WhiteWizardInvasionPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.InvasionEvents;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Enables the White Wizard Invasion feature.
/// </summary>
[PlugIn]
[Display(Name = "White Wizard Invasion", Description = "Enables the White Wizard Invasion feature.")]
[Guid("4B5D0F55-5B26-4447-B9C0-C272E5D0A141")]
public sealed class WhiteWizardInvasionPlugIn : SimpleInvasionPlugIn
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WhiteWizardInvasionPlugIn"/> class.
    /// </summary>
    public WhiteWizardInvasionPlugIn()
        : base(() => InvasionConfigurationDefaults.WhiteWizard)
    {
    }

    /// <inheritdoc />
    protected override ushort? AnnouncedMonsterId => InvasionMonsters.WhiteWizard;
}

<<<<<<< HEAD
// -----------------------------------------------------------------------
=======
﻿// -----------------------------------------------------------------------
>>>>>>> origin/slave
// <copyright file="LifeJewelConsumeHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Consume handler for the Jewel of Life which adds and increases the item option of <see cref="ItemOptionTypes.Option"/>.
/// </summary>
<<<<<<< HEAD
/// <remarks>
/// The success chance and the behaviour on a failed upgrade are configurable, see
/// <see cref="LifeJewelConsumeHandlerPlugInConfiguration"/>.
/// </remarks>
[Guid("8AC6592D-D51C-47C9-B491-4778C615691D")]
[PlugIn]
[Display(Name = nameof(PlugInResources.LifeJewelConsumeHandlerPlugIn_Name), Description = nameof(PlugInResources.LifeJewelConsumeHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
public class LifeJewelConsumeHandlerPlugIn : ItemUpgradeConsumeHandlerPlugIn, ISupportCustomConfiguration<LifeJewelConsumeHandlerPlugInConfiguration>, ISupportDefaultCustomConfiguration
{
    private LifeJewelConsumeHandlerPlugInConfiguration? _configuration;

=======
[Guid("8AC6592D-D51C-47C9-B491-4778C615691D")]
[PlugIn]
[Display(Name = nameof(PlugInResources.LifeJewelConsumeHandlerPlugIn_Name), Description = nameof(PlugInResources.LifeJewelConsumeHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
public class LifeJewelConsumeHandlerPlugIn : ItemUpgradeConsumeHandlerPlugIn
{
>>>>>>> origin/slave
    /// <summary>
    /// Initializes a new instance of the <see cref="LifeJewelConsumeHandlerPlugIn" /> class.
    /// </summary>
    public LifeJewelConsumeHandlerPlugIn()
        : base(new ItemUpgradeConfiguration(ItemOptionTypes.Option, true, true, 0.5, ItemFailResult.RemoveOption))
    {
    }

    /// <inheritdoc />
<<<<<<< HEAD
    public LifeJewelConsumeHandlerPlugInConfiguration? Configuration
    {
        get => this._configuration;
        set
        {
            this._configuration = value;
            if (value is null)
            {
                // Without a configuration, the defaults from the constructor apply.
                return;
            }

            this.UpgradeConfiguration.SuccessChance = value.SuccessChance;
            this.UpgradeConfiguration.FailResult = value.FailResult;
        }
    }

    /// <inheritdoc />
    public override ItemIdentifier Key => ItemConstants.JewelOfLife;

    /// <inheritdoc />
    public object CreateDefaultConfig() => new LifeJewelConsumeHandlerPlugInConfiguration();
}
=======
    public override ItemIdentifier Key => ItemConstants.JewelOfLife;
}
>>>>>>> origin/slave

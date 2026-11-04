// <copyright file="AddDarkHorseCanFlyPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update lets the Dark Horse fly: it gets the <see cref="Stats.CanFly"/> power-up, which the
/// Icarus map requires, like the wings and the Horn of Fenrir. The client counts it as a flying mount.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("B2A18B26-5BF6-4EBC-80D9-421CB8B6BFCE")]
public class AddDarkHorseCanFlyPlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Add CanFly to the Dark Horse";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update lets characters on a Dark Horse enter Icarus, like with wings, a Horn of Dinorant or a Horn of Fenrir.";

    private const byte DarkHorseGroup = 13;

    private const short DarkHorseNumber = 4;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 09, 26, 22, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var canFly = Stats.CanFly.GetPersistent(gameConfiguration);
        foreach (var darkHorse in gameConfiguration.Items.Where(item => item.Group == DarkHorseGroup && item.Number == DarkHorseNumber))
        {
            if (darkHorse.BasePowerUpAttributes.Any(powerUp => powerUp.TargetAttribute == canFly))
            {
                continue;
            }

            var powerUp = context.CreateNew<ItemBasePowerUpDefinition>();
            powerUp.TargetAttribute = canFly;
            powerUp.BaseValue = 1;
            powerUp.AggregateType = AggregateType.AddRaw;
            darkHorse.BasePowerUpAttributes.Add(powerUp);
        }

        return ValueTask.CompletedTask;
    }
}

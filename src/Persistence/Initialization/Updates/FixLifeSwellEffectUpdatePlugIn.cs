// <copyright file="FixLifeSwellEffectUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.Skills;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update sets the right settings for the life swell effect.
/// </summary>
[PlugIn(PlugInName, PlugInDescription)]
[Guid("FD521A61-D5B4-4CF2-B203-6FFF12C80E51")]
public class FixLifeSwellEffectUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Fixed life swell effect";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update sets the right settings for the life swell effect.";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.FixLifeSwellEffect;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2024, 08, 25, 15, 05, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var effect = gameConfiguration.MagicEffects.FirstOrDefault(m => m.Number == (short)MagicEffectNumber.GreaterFortitude);
        if (effect is null)
        {
            return;
        }

        var boost = effect.PowerUpDefinitions.FirstOrDefault()?.Boost;
        if (boost is null)
        {
            return;
        }

        foreach (var relatedValue in boost.RelatedValues)
        {
            relatedValue.InputOperator = InputOperator.ExponentiateByAttribute;
        }
    }
}

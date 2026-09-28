// <copyright file="FounderExperienceBonusPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Gives a small, permanent experience bonus (normal and master experience alike) to accounts which
/// registered before the configured cutoff — a "Founder" perk for whoever signed up during the beta.
/// </summary>
/// <remarks>
/// No account flag or migration is needed: OpenMU already stores <see cref="MUnique.OpenMU.DataModel.Entities.Account.RegistrationDate"/>
/// for every account, so "Founder" is simply "registered before the cutoff". The website can show the
/// same badge by reading that same date (see web.get_account / web.get_characters and lib/beta.ts's
/// BETA_OPENS_AT, which should be kept equal to <see cref="FounderBonusConfiguration.CutoffDate"/>, converted to UTC).
///
/// Disabled by default: the cutoff and the bonus are meant to be reviewed once (Plugins page) before
/// turning it on, since it changes what players earn.
/// </remarks>
[PlugIn]
[Display(Name = "Founder Experience Bonus", Description = "Gives a small experience bonus to accounts registered before a configured cutoff date (e.g. everyone who signed up during the beta).")]
[Guid("9F2C7B7E-8B1A-4C2D-9E3F-6D5A0C8B4F21")]
public class FounderExperienceBonusPlugIn : IExperienceCalculationPlugIn, ISupportCustomConfiguration<FounderBonusConfiguration>, ISupportDefaultCustomConfiguration, IDisabledByDefault
{
    /// <inheritdoc />
    public FounderBonusConfiguration? Configuration { get; set; }

    /// <inheritdoc />
    public object CreateDefaultConfig() => FounderBonusConfiguration.Default;

    /// <inheritdoc />
    public ValueTask CalculateExperienceAsync(Player player, ExperienceCalculationArgs args)
    {
        var configuration = this.Configuration;
        if (configuration is null || configuration.BonusMultiplier == 1f)
        {
            return ValueTask.CompletedTask;
        }

        if (player.Account is { } account && account.RegistrationDate < configuration.CutoffDate)
        {
            args.Experience *= configuration.BonusMultiplier;
        }

        return ValueTask.CompletedTask;
    }
}

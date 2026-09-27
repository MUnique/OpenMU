// <copyright file="FounderBonusConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Configuration of <see cref="FounderExperienceBonusPlugIn"/>.
/// </summary>
public class FounderBonusConfiguration
{
    /// <summary>
    /// Gets the default configuration: the cutoff matches the website's beta opening
    /// (2026-10-12 10:00 Argentina time = 13:00 UTC), and the bonus is a small +5%.
    /// </summary>
    public static FounderBonusConfiguration Default => new()
    {
        CutoffDate = new DateTime(2026, 10, 12, 13, 0, 0, DateTimeKind.Utc),
        BonusMultiplier = 1.05f,
    };

    /// <summary>
    /// Gets or sets the cutoff date and time (UTC). Accounts registered before this moment are "Founders".
    /// </summary>
    [Display(Name = "Cutoff Date (UTC)", Description = "Las cuentas registradas antes de esta fecha y hora (en UTC) son Fundadoras. Tiene que coincidir con la apertura de la beta que muestra la web (BETA_OPENS_AT), pasada a UTC.")]
    public DateTime CutoffDate { get; set; }

    /// <summary>
    /// Gets or sets the experience multiplier applied on top of everything else for Founder accounts.
    /// 1.05 means +5%.
    /// </summary>
    [Display(Name = "Bonus Multiplier", Description = "Cuánta experiencia extra reciben las cuentas Fundadoras. 1.05 = +5%. 1.00 desactiva el bonus sin apagar el plugin.")]
    public float BonusMultiplier { get; set; } = 1.05f;
}

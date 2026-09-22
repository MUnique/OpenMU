// <copyright file="ExperienceCalculationArgs.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

/// <summary>
/// The arguments of the <see cref="IExperienceCalculationPlugIn"/>. Plugin points can't return
/// values, so the calculated experience is passed in and out through this object.
/// </summary>
public sealed class ExperienceCalculationArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExperienceCalculationArgs"/> class.
    /// </summary>
    /// <param name="killedObject">The killed object which caused the experience gain.</param>
    /// <param name="isMasterExperience">If set to <c>true</c>, master experience is gained.</param>
    /// <param name="experience">The calculated experience.</param>
    public ExperienceCalculationArgs(IAttackable killedObject, bool isMasterExperience, double experience)
    {
        this.KilledObject = killedObject;
        this.IsMasterExperience = isMasterExperience;
        this.Experience = experience;
    }

    /// <summary>
    /// Gets the killed object which caused the experience gain.
    /// </summary>
    public IAttackable KilledObject { get; }

    /// <summary>
    /// Gets a value indicating whether master experience is gained instead of normal experience.
    /// </summary>
    public bool IsMasterExperience { get; }

    /// <summary>
    /// Gets or sets the calculated experience. It can be modified by the plugins.
    /// </summary>
    public double Experience { get; set; }
}

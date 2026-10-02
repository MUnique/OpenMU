// <copyright file="CaptionLinkStep.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Captions;

/// <summary>
/// The steps of <see cref="ConfigurationCaptionService.LinkBuiltInCaptionsAsync"/>, in the order of their execution.
/// </summary>
public enum CaptionLinkStep
{
    /// <summary>
    /// The configuration is loaded from the database.
    /// </summary>
    LoadingConfiguration,

    /// <summary>
    /// The reference configuration is created by executing the data initialization in memory. This usually takes the longest.
    /// </summary>
    CreatingReferenceConfiguration,

    /// <summary>
    /// The captions of the configuration are linked to the sources of the reference configuration.
    /// </summary>
    LinkingCaptions,

    /// <summary>
    /// The changes are saved to the database.
    /// </summary>
    Saving,

    /// <summary>
    /// All steps are completed.
    /// </summary>
    Completed,
}

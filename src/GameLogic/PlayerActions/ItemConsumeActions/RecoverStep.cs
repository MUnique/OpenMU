// <copyright file="RecoverStep.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

/// <summary>
/// Defines one step of a recovery.
/// </summary>
public class RecoverStep
{
    /// <summary>
    /// Gets or sets the delay after which the recovery of the defined <see cref="RecoverPercentage"/>
    /// occurs.
    /// </summary>
    public TimeSpan Delay { get; set; }

    /// <summary>
    /// Gets or sets the recover percentage of the <see cref="RecoverConsumeHandlerConfiguration.TotalRecoverPercentage"/>
    /// after waiting for the <see cref="Delay"/>.
    /// The total of all steps of a <see cref="RecoverConsumeHandlerConfiguration"/> should be 100.
    /// </summary>
    public int RecoverPercentage { get; set; }
}
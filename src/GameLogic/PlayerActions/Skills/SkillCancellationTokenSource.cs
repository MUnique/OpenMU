// <copyright file="SkillCancelTokenSource.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Skills;

using System.Threading;

/// <summary>
/// A <see cref="CancellationTokenSource"/> which allows to specify an explicit target when cancelling the nova skill.
/// </summary>
/// <seealso cref="System.Threading.CancellationTokenSource" />
public class SkillCancellationTokenSource : CancellationTokenSource
{
    /// <summary>
    /// Gets the explicit target identifier.
    /// </summary>
    public ushort? ExplicitTargetId { get; private set; }

    /// <summary>
    /// Communicates a request for cancellation, <see cref="CancellationTokenSource.Cancel()"/>.
    /// </summary>
    /// <param name="extraTarget">The extra target.</param>
    public void CancelWithExtraTarget(ushort? extraTarget)
    {
        this.ExplicitTargetId = extraTarget;
        this.Cancel();
    }
}
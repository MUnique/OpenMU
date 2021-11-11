﻿// -----------------------------------------------------------------------
// <copyright file="AppleConsumeHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

/// <summary>
/// Consume handler for apples.
/// </summary>
public class AppleConsumeHandler : HealthPotionConsumeHandler
{
    /// <inheritdoc/>
    protected override int Multiplier
    {
        get { return 0; }
    }
}
// <copyright file="ItemDropValidationResult.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Items;

/// <summary>
/// Result of item drop validation.
/// </summary>
/// <param name="IsValid">Whether the drop operation is valid.</param>
/// <param name="ErrorMessage">Error message if validation failed.</param>
public record ItemDropValidationResult(bool IsValid, string? ErrorMessage);
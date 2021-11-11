﻿// <copyright file="AggregateType.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AttributeSystem;

/// <summary>
/// The attribute aggregate type.
/// </summary>
public enum AggregateType
{
    /// <summary>
    /// Adds the value to the raw base value.
    /// </summary>
    AddRaw,

    /// <summary>
    /// Multiplicates the value.
    /// </summary>
    Multiplicate,

    /// <summary>
    /// Adds the value to the final value.
    /// </summary>
    AddFinal,
}
﻿// <copyright file="IAttacker.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using MUnique.OpenMU.AttributeSystem;

/// <summary>
/// Interface for an object which can attack.
/// </summary>
public interface IAttacker : IIdentifiable, ILocateable
{
    /// <summary>
    /// Gets the attributes.
    /// </summary>
    IAttributeSystem Attributes { get; }
}
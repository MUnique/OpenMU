// <copyright file="SecretAttribute.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.PlugIns;

/// <summary>Marks a credential or other sensitive property which agents must never read or modify.</summary>
/// <remarks>The panel masks marked strings. Audit projections omit marked properties, including nested values.</remarks>
[AttributeUsage(AttributeTargets.Property)]
public sealed class SecretAttribute : Attribute;

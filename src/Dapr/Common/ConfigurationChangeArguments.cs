// <copyright file="ConfigurationChangeArguments.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Dapr.Common;

/// <summary>
/// Arguments for the change notifications of <see cref="ConfigurationChangePublisher"/>.
/// </summary>
/// <param name="Type">The configuration type.</param>
/// <param name="Id">The configuration identifier.</param>
/// <param name="Configuration">The configuration object.</param>
public record class ConfigurationChangeArguments(Type Type, Guid Id, object? Configuration);
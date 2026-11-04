// <copyright file="PlayerCountSample.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations;

/// <summary>A sample of the online player count.</summary>
/// <param name="Timestamp">The timestamp.</param>
/// <param name="PlayerCount">The playercount.</param>
public sealed record PlayerCountSample(DateTime Timestamp, int PlayerCount);

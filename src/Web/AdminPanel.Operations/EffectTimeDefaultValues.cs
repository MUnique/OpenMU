// <copyright file="EffectTimeDefaultValues.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations;

/// <summary>
/// Which magic effects send their remaining time in the data which the installed configuration was created from.
/// </summary>
/// <param name="InitializationKey">The key of the data initialization, e.g. "season6".</param>
/// <param name="Caption">The caption of the data initialization, e.g. "0.75".</param>
/// <param name="SendsTime">Whether the effects send their remaining time, by their number.</param>
public sealed record EffectTimeDefaultValues(string InitializationKey, string Caption, IReadOnlyDictionary<short, bool> SendsTime);

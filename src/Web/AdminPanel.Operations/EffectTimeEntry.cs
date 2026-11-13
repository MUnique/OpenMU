// <copyright file="EffectTimeEntry.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations;

/// <summary>
/// A snapshot of a magic effect and whether it sends its remaining time to the client, see <see cref="EffectTimeOperations"/>.
/// </summary>
/// <param name="Id">The identifier of the magic effect definition.</param>
/// <param name="Number">The number of the effect.</param>
/// <param name="Name">The name of the effect.</param>
/// <param name="SendsTime">A value indicating whether the effect sends its remaining time to the client.</param>
public sealed record EffectTimeEntry(Guid Id, short Number, string Name, bool SendsTime);

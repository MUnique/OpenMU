// <copyright file="AdminActor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations;

/// <summary>Identifies an authenticated caller independently of a UI framework.</summary>
/// <param name="Id">The stable user or API key identifier.</param>
/// <param name="Name">The display name.</param>
/// <param name="Frontend">The trusted front end identifier.</param>
/// <param name="Roles">The effective roles.</param>
public sealed record AdminActor(string Id, string Name, string Frontend, IReadOnlyList<string> Roles);

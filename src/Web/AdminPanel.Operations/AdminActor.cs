// <copyright file="AdminActor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations;

/// <summary>Identifies a caller independently of a UI framework.</summary>
/// <param name="Id">The stable user or API key identifier.</param>
/// <param name="Name">The display name.</param>
/// <param name="Frontend">The trusted front end identifier.</param>
/// <param name="Roles">The effective roles.</param>
public sealed record AdminActor(string Id, string Name, string Frontend, IReadOnlyList<string> Roles)
{
    /// <summary>Gets the optional tool allowlist of an API key.</summary>
    public string AllowedToolGroups { get; init; } = string.Empty;

    /// <summary>Gets a value indicating whether the trusted panel admitted anonymous initial setup.</summary>
    public bool IsInitialSetup { get; init; }
}

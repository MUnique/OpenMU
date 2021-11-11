// <copyright file="SchemaNames.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

/// <summary>
/// Defines constants for the database schema names.
/// </summary>
internal static class SchemaNames
{
    /// <summary>
    /// The schema name for account data.
    /// </summary>
    internal const string AccountData = "data";

    /// <summary>
    /// The schema name for configuration data.
    /// </summary>
    internal const string Configuration = "config";

    /// <summary>
    /// The schema name for the friend server data.
    /// </summary>
    internal const string Friend = "friend";

    /// <summary>
    /// The schema name for the guild server data.
    /// </summary>
    internal const string Guild = "guild";
}
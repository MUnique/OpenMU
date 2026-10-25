// <copyright file="AdminToolGroups.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Auth;

/// <summary>Parses optional per-key MCP allowlists. Empty preserves existing role-only access.</summary>
public static class AdminToolGroups
{
    /// <summary>Gets the signed claim containing the configured tool restriction.</summary>
    public static string ClaimType => "openmu:allowed-mcp-tools";

    /// <summary>Validates a bounded comma-separated list of exact tool names or area_* patterns.</summary>
    /// <param name="value">The configured allowlist.</param>
    /// <returns>Whether the syntax is valid.</returns>
    public static bool IsValid(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return true;
        }

        return value.Length <= 1000 && value.Split(',').All(part =>
        {
            var pattern = part.Trim();
            var stem = pattern.EndsWith("_*", StringComparison.Ordinal) ? pattern[..^1] : pattern;
            return stem.Length >= 3 && stem.Contains('_', StringComparison.Ordinal) && stem[0] is >= 'a' and <= 'z'
                && stem.All(c => c is >= 'a' and <= 'z' or '_');
        });
    }

    /// <summary>Checks an exact tool name against the configured allowlist; malformed restrictions fail closed.</summary>
    /// <param name="value">The allowlist.</param>
    /// <param name="tool">The invariant MCP tool name.</param>
    /// <returns>Whether the tool is allowed in addition to its role requirement.</returns>
    public static bool Allows(string? value, string tool)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return true;
        }

        return IsValid(value) && value.Split(',', StringSplitOptions.TrimEntries).Any(pattern => pattern.EndsWith("_*", StringComparison.Ordinal)
            ? tool.StartsWith(pattern[..^1], StringComparison.Ordinal) : tool.Equals(pattern, StringComparison.Ordinal));
    }
}

// <copyright file="GuildMasterInactivityResult.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces;

/// <summary>
/// The outcome of the handling of an inactive guild master, see <see cref="IGuildServer.RemoveInactiveGuildMastersAsync"/>.
/// </summary>
/// <param name="GuildName">The name of the guild.</param>
/// <param name="OldMasterName">The name of the character which was the guild master.</param>
/// <param name="NewMasterName">The name of the character which became the guild master; <c>null</c>, if the guild was dissolved.</param>
/// <param name="Inactivity">The time which the old guild master has been inactive.</param>
public sealed record GuildMasterInactivityResult(string GuildName, string OldMasterName, string? NewMasterName, TimeSpan Inactivity)
{
    /// <summary>
    /// Gets a value indicating whether the guild was dissolved, because there was no active member which could become the guild master.
    /// </summary>
    public bool IsDissolved => this.NewMasterName is null;
}

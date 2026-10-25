// <copyright file="DiscordTestResult.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord;

/// <summary>
/// The result of a test message.
/// </summary>
/// <param name="Outcome">The outcome.</param>
/// <param name="Detail">The channel which received the message, or the reason why it failed.</param>
public sealed record DiscordTestResult(DiscordTestOutcome Outcome, string? Detail = null);

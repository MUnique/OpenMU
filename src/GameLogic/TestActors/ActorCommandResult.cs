// <copyright file="ActorCommandResult.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.TestActors;

/// <summary>
/// The outcome of one <see cref="ActorCommand"/>. A command is never silently ignored: it either
/// succeeded with a result, or it carries the code of the precondition which refused it.
/// </summary>
/// <param name="Ok"><c>true</c> when the command was carried out.</param>
/// <param name="Code">The machine readable failure code, e.g. <c>out_of_range</c>; <c>null</c> on success.</param>
/// <param name="Error">The human readable failure message; <c>null</c> on success.</param>
/// <param name="Fields">The result fields, e.g. the hits performed or the path length.</param>
public sealed record ActorCommandResult(bool Ok, string? Code, string? Error, IReadOnlyList<ActorEventField> Fields)
{
    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <param name="fields">The result fields.</param>
    /// <returns>The result.</returns>
    public static ActorCommandResult Success(params ActorEventField[] fields)
        => new(true, null, null, fields);

    /// <summary>
    /// Creates a failed result.
    /// </summary>
    /// <param name="code">The failure code.</param>
    /// <param name="error">The failure message.</param>
    /// <param name="fields">The progress made before the failure, if any.</param>
    /// <returns>The result.</returns>
    public static ActorCommandResult Failure(string code, string error, params ActorEventField[] fields)
        => new(false, code, error, fields);
}

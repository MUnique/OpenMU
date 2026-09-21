// <copyright file="WalkCommand.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.TestActors;

/// <summary>
/// Walks to a position of the current map, using the server's path finder - one request, like a
/// client's click.
/// </summary>
/// <param name="X">The target X coordinate.</param>
/// <param name="Y">The target Y coordinate.</param>
public sealed record WalkCommand(byte X, byte Y) : ActorCommand("walk");

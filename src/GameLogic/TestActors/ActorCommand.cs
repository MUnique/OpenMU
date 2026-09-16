// <copyright file="ActorCommand.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.TestActors;

/// <summary>
/// One thing a scenario tells an actor to do. Commands are executed by the actor's own
/// <see cref="ScriptedIntelligence"/>, serialized with its persistence lock.
/// </summary>
/// <param name="Name">The command name as it appears in the protocol and in the log.</param>
public abstract record ActorCommand(string Name);

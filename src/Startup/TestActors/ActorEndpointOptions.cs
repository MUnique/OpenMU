// <copyright file="ActorEndpointOptions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Startup.TestActors;

using MUnique.OpenMU.GameLogic.TestActors;

/// <summary>
/// The options of the actor control endpoint.
/// </summary>
/// <param name="Port">The TCP port to listen on inside the container.</param>
public sealed record ActorEndpointOptions(int Port);

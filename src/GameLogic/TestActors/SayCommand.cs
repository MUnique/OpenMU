// <copyright file="SayCommand.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.TestActors;

/// <summary>
/// Says something, including the chat commands a client can send (e.g. <c>/ver</c>).
/// </summary>
/// <param name="Text">The message.</param>
public sealed record SayCommand(string Text) : ActorCommand("say");

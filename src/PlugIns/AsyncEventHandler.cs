// <copyright file="AsyncEventHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.PlugIns;

/// <summary>
/// Event handler which is awaitable.
/// </summary>
/// <typeparam name="T">The type of the event args.</typeparam>
/// <param name="eventArgs">The event arguments.</param>
public delegate ValueTask AsyncEventHandler<in T>(T eventArgs);

/// <summary>
/// Event handler without arguments which is awaitable.
/// </summary>
public delegate ValueTask AsyncEventHandler();
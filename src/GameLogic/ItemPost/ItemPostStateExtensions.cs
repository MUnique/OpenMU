// <copyright file="ItemPostStateExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.ItemPost;

using System.Runtime.CompilerServices;

/// <summary>
/// Extension methods to manage the <see cref="ItemPostState"/> for a <see cref="IGameContext"/>.
/// </summary>
public static class ItemPostStateExtensions
{
    private static readonly ConditionalWeakTable<IGameContext, ItemPostState> ItemPostStates = new ();

    /// <summary>
    /// Gets the <see cref="ItemPostState"/> for the specified <paramref name="gameContext"/>.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    /// <returns>The item post state.</returns>
    public static ItemPostState GetItemPostState(this IGameContext gameContext)
    {
        return ItemPostStates.GetValue(gameContext, _ => new ItemPostState());
    }
}

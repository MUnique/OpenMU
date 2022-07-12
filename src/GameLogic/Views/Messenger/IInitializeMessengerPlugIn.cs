// <copyright file="IInitializeMessengerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Messenger;

using MUnique.OpenMU.Interfaces;

/// <summary>
/// Interface of a view whose implementation informs about the initialization of the messenger.
/// </summary>
public interface IInitializeMessengerPlugIn : IViewPlugIn
{
    /// <summary>
    /// Initializes the messenger. Adds the letters and the friends to the view.
    /// </summary>
    /// <param name="data">The data which was provided by the <see cref="IFriendServer"/>.</param>
    /// <param name="maxLetters">The maximum number of letters a player can have in its inbox.</param>
    ValueTask InitializeMessengerAsync(MessengerInitializationData data, int maxLetters);
}
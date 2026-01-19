// <copyright file="IShowMessagePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views;

using MUnique.OpenMU.Interfaces;

/// <summary>
/// Interface of a view whose implementation informs about a shown message.
/// TODO: Offer to pass LocalizedString and formatting parameters.
/// </summary>
public interface IShowMessagePlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="messageType">Type of the message.</param>
    ValueTask ShowMessageAsync(string message, MessageType messageType);
}
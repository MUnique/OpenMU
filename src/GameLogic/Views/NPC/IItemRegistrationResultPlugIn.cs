// <copyright file="IItemRegistrationResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.NPC;

using System.Threading.Tasks;

/// <summary>
/// A view plugin which provides the result of registering an item at the Golden Archer.
/// </summary>
public interface IItemRegistrationResultPlugIn : IViewPlugIn
{
    /// <summary>
    /// Registration Operations
    /// </summary>
    enum ItemRegistrationOperation
    {
        OpenRegistrationDialog,
        MissingItem,
        RegistrationCompleted
    }
    /// <summary>
    /// Sends the registration result to the client.
    /// </summary>
    ValueTask RegistrationResultAsync(int npcNumber, ItemRegistrationOperation operation);
}

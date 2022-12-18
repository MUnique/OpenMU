// <copyright file="IMuHelperConfigurationUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.MuHelper;

/// <summary>
/// Interface of a view whose implementation sends the MU Helper data back to the client.
/// </summary>
public interface IMuHelperConfigurationUpdatePlugIn : IViewPlugIn
{
    /// <summary>
    /// Sends the saved MU Helper data.
    /// </summary>
    /// <param name="data">The data.</param>
    ValueTask UpdateMuHelperConfigurationAsync(Memory<byte> data);
}
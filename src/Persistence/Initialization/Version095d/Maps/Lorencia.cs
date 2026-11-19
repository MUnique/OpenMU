// <copyright file="Lorencia.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version095d.Maps;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// The initialization for the Lorencia map.
/// </summary>
internal class Lorencia : Version075.Maps.Lorencia
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Lorencia"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public Lorencia(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }
}
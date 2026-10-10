// <copyright file="Gates.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version097k;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Initialization of the gates of version 0.97k.
/// </summary>
/// <remarks>
/// Compared to version 0.95d, it adds the entrances of the Blood Castle maps.
/// </remarks>
public class Gates : Version095d.Gates
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Gates" /> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public Gates(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    public override void Initialize()
    {
        base.Initialize();

        // Blood Castle 1 to 6, gates 66 to 71:
        foreach (var map in this.GameConfiguration.Maps.Where(map => map.Number is >= 11 and <= 16))
        {
            this.CreateExitGate(map, 12, 5, 14, 10, 0);
        }
    }
}

// <copyright file="MapIdentity.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Record to identify a map, by considering the <see cref="GameMapDefinition.Discriminator"/>.
/// </summary>
/// <param name="MapNumber">The number of the map.</param>
/// <param name="Discriminator">The dicriminator of the map.</param>
internal record MapIdentity(short MapNumber, int Discriminator = 0)
{
    public static implicit operator MapIdentity(short mapNumber) => new (mapNumber);
}
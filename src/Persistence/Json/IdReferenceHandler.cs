// <copyright file="IdReferenceHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Json;

using System.Text.Json.Serialization;

/// <summary>
/// <see cref="ReferenceHandler"/> which creates a <see cref="IdReferenceResolver"/>.
/// </summary>
public class IdReferenceHandler : ReferenceHandler, IIdReferenceHandler
{
    /// <inheritdoc />
    public ReferenceResolver? Current { get; private set; }

    /// <inheritdoc />
    public override ReferenceResolver CreateResolver()
    {
        return this.Current ??= new IdReferenceResolver();
    }
}
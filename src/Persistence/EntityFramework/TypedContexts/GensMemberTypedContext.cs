// <copyright file="GensMemberTypedContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.TypedContexts;

/// <summary>
/// The <see cref="TypedContext"/> for <see cref="MUnique.OpenMU.DataModel.Entities.GensMember"/>, which uses a compiled model.
/// </summary>
internal sealed class GensMemberTypedContext : TypedContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GensMemberTypedContext"/> class.
    /// </summary>
    public GensMemberTypedContext()
        : base(typeof(MUnique.OpenMU.DataModel.Entities.GensMember))
    {
    }
}

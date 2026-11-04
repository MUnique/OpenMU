// <copyright file="GensAbuseTypedContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.TypedContexts;

/// <summary>
/// The <see cref="TypedContext"/> for <see cref="MUnique.OpenMU.DataModel.Entities.GensAbuse"/>, which uses a compiled model.
/// </summary>
internal sealed class GensAbuseTypedContext : TypedContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GensAbuseTypedContext"/> class.
    /// </summary>
    public GensAbuseTypedContext()
        : base(typeof(MUnique.OpenMU.DataModel.Entities.GensAbuse))
    {
    }
}

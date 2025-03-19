// <copyright file="MyModelCacheKeyFactory.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

/// <summary>
/// Our own implementation for a <see cref="IModelCacheKeyFactory"/>,
/// so the models of <see cref="TypedContext"/> are cached for their corresponding types.
/// </summary>
public class MyModelCacheKeyFactory : ModelCacheKeyFactory
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MyModelCacheKeyFactory"/> class.
    /// </summary>
    /// <param name="dependencies">Parameter object containing dependencies for this service.</param>
    public MyModelCacheKeyFactory(ModelCacheKeyFactoryDependencies dependencies)
        : base(dependencies)
    {
    }

    /// <inheritdoc />
    public override object Create(DbContext context)
    {
        if (context is TypedContext typedContext)
        {
            return (typeof(TypedContext), typedContext.EditType);
        }

        return base.Create(context);
    }

    /// <inheritdoc />
    public override object Create(DbContext context, bool designTime)
    {
        if (context is TypedContext typedContext)
        {
            return (typeof(TypedContext), typedContext.EditType);
        }

        return base.Create(context, designTime);
    }
}
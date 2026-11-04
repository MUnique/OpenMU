// <copyright file="DesignTimeServices.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.DesignTime;

using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Configures the services which are used by the entity framework core tools at design time,
/// e.g. when generating migrations or compiled models.
/// It's discovered automatically by the tools.
/// </summary>
public class DesignTimeServices : IDesignTimeServices
{
    /// <inheritdoc />
    public void ConfigureDesignTimeServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<ICSharpHelper, FullyQualifyingCSharpHelper>();
    }
}

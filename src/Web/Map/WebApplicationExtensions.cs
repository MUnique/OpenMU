// <copyright file="WebApplicationExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Map;

using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using MUnique.OpenMU.Web.Map.Map;

/// <summary>
/// Extensions for the <see cref="WebApplicationBuilder"/> to add the web map app.
/// </summary>
public static class WebApplicationExtensions
{
    /// <summary>
    /// Adds the map application to the web app.
    /// When using the DaprService, call the BuildAndConfigure-Method with the parameter to add Blazor.
    /// </summary>
    /// <param name="builder">The web application builder which should be configured.</param>
    /// <returns>The web application builder.</returns>
    public static WebApplicationBuilder AddMapApp(this WebApplicationBuilder builder)
    {
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();
        builder.Services.AddScoped<IMapFactory, JavascriptMapFactory>();

        StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);
        return builder;
    }
}
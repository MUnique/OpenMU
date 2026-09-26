// <copyright file="PlugInPointCaption.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Services;

using System.Globalization;
using System.Resources;

/// <summary>Localizes plugin extension point metadata for the admin interface.</summary>
public static class PlugInPointCaption
{
    private static readonly ResourceManager ResourceManager = new("MUnique.OpenMU.Web.Shared.Properties.PlugInPointResources", typeof(PlugInPointCaption).Assembly);

    /// <summary>Gets translated metadata, preserving unknown external plugin text.</summary>
    /// <param name="text">The original extension point name or description.</param>
    /// <returns>The localized display text.</returns>
    public static string Get(string text) => ResourceManager.GetString(text, CultureInfo.CurrentUICulture) ?? text;
}

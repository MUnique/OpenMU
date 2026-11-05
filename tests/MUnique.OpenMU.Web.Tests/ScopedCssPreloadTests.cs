// <copyright file="ScopedCssPreloadTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests;

using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;

/// <summary>Scoped CSS preload headers must resolve to the same assets when a web project is embedded.</summary>
[TestFixture]
public sealed class ScopedCssPreloadTests
{
    /// <summary>Preloads of embedded application bundles remain inside the host's path base.</summary>
    /// <param name="project">The embedded web project.</param>
    /// <param name="pathBase">The path base of the hosting application.</param>
    [TestCase("AdminPanel", "/")]
    [TestCase("AdminPanel", "/admin/")]
    [TestCase("Map", "/")]
    [TestCase("Map", "/gameServer/0/")]
    public void PreloadsResolveToPublishedEndpoints(string project, string pathBase)
    {
        var assemblyName = $"MUnique.OpenMU.Web.{project}";
        using var manifest = JsonDocument.Parse(File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, $"{assemblyName}.staticwebassets.endpoints.json")));
        var endpoints = manifest.RootElement.GetProperty("Endpoints").EnumerateArray().ToArray();
        var routes = endpoints.Select(endpoint => endpoint.GetProperty("Route").GetString()).ToHashSet(StringComparer.Ordinal);
        var bundles = endpoints.Where(endpoint => endpoint.GetProperty("Route").GetString() is { } route
            && route.StartsWith(assemblyName, StringComparison.Ordinal) && route.EndsWith(".styles.css", StringComparison.Ordinal)).ToArray();
        Assert.That(bundles, Is.Not.Empty);
        var checkedLinks = 0;
        foreach (var bundle in bundles)
        {
            var bundleUrl = new Uri($"https://example.test{pathBase}_content/{assemblyName}/{bundle.GetProperty("Route").GetString()}");
            foreach (var header in bundle.GetProperty("ResponseHeaders").EnumerateArray().Where(header => header.GetProperty("Name").GetString() == "Link"))
            {
                foreach (Match match in Regex.Matches(header.GetProperty("Value").GetString()!, "<([^>]+)>"))
                {
                    var resolved = new Uri(bundleUrl, match.Groups[1].Value);
                    Assert.That(resolved.AbsolutePath, Does.StartWith(pathBase), "The preload must preserve PathBase.");
                    Assert.That(routes, Does.Contain(resolved.AbsolutePath[pathBase.Length..]), $"Broken preload: {resolved.AbsolutePath}");
                    checkedLinks++;
                }
            }
        }

        Assert.That(checkedLinks, Is.Positive);
    }
}

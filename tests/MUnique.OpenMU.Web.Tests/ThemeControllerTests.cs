// <copyright file="ThemeControllerTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests;

using MUnique.OpenMU.Web.Shared.Services;

/// <summary>
/// Tests for the theme handling of the <see cref="ThemeController"/>.
/// </summary>
[TestFixture]
public class ThemeControllerTests
{
    /// <summary>
    /// Known theme values are normalized; missing and unknown values fall back to the automatic theme.
    /// </summary>
    /// <param name="input">The raw theme value of the cookie or request.</param>
    /// <param name="expected">The expected normalized theme.</param>
    [TestCase("light", "light")]
    [TestCase("LIGHT", "light")]
    [TestCase("dark", "dark")]
    [TestCase("Dark", "dark")]
    [TestCase("auto", "auto")]
    [TestCase(null, "auto")]
    [TestCase("", "auto")]
    [TestCase("blue", "auto")]
    public void NormalizeTheme(string? input, string expected)
    {
        Assert.That(ThemeController.NormalizeTheme(input), Is.EqualTo(expected));
    }

    /// <summary>
    /// The server renders the bootstrap theme light for the automatic theme, because it can't know the color scheme of the client.
    /// </summary>
    /// <param name="theme">The normalized theme preference.</param>
    /// <param name="expected">The expected value of the data-bs-theme attribute.</param>
    [TestCase("light", "light")]
    [TestCase("dark", "dark")]
    [TestCase("auto", "light")]
    public void GetBootstrapTheme(string theme, string expected)
    {
        Assert.That(ThemeController.GetBootstrapTheme(theme), Is.EqualTo(expected));
    }
}

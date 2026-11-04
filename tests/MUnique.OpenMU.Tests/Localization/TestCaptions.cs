// <copyright file="TestCaptions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Localization;

using System.Resources;

/// <summary>
/// Strongly typed test resources with sparse translations (zh-CN: all, zh-TW and de: one each).
/// </summary>
public static class TestCaptions
{
    /// <summary>
    /// Gets the resource manager.
    /// </summary>
    public static ResourceManager ResourceManager { get; } = new("MUnique.OpenMU.Tests.Localization.TestCaptions", typeof(TestCaptions).Assembly);

    /// <summary>
    /// Gets the Lorencia caption.
    /// </summary>
    public static string Lorencia => ResourceManager.GetString(nameof(Lorencia))!;

    /// <summary>
    /// Gets the Devias caption.
    /// </summary>
    public static string Devias => ResourceManager.GetString(nameof(Devias))!;

    /// <summary>
    /// Gets a value which is not a resource property, to test invalid expressions.
    /// </summary>
    public static string NotAResource => "x";
}

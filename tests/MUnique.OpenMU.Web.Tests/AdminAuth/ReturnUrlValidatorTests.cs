// <copyright file="ReturnUrlValidatorTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.AdminAuth;

using MUnique.OpenMU.Web.AdminPanel.Auth;

/// <summary>
/// Tests for the <see cref="ReturnUrlValidator"/>, which keeps the login page from sending
/// the user to a foreign site or a script url.
/// </summary>
[TestFixture]
public class ReturnUrlValidatorTests
{
    private const string BaseUri = "https://example.com/admin/";

    /// <summary>
    /// Tests that relative urls within the same origin are accepted, including the ones which
    /// lead to another application behind the same reverse proxy.
    /// </summary>
    /// <param name="returnUrl">The return url.</param>
    [TestCase("accounts")]
    [TestCase("accounts/edit?id=1")]
    [TestCase("../grafana/d/abc?from=now-1h&var=a:b")]
    [TestCase("../../grafana/")]
    [TestCase("logs#a:b")]
    public void SafeUrlIsAccepted(string returnUrl)
    {
        Assert.That(ReturnUrlValidator.GetSafeReturnUrl(returnUrl, BaseUri), Is.EqualTo(returnUrl));
    }

    /// <summary>
    /// Tests that urls which could leave the admin panel origin or execute a script are rejected.
    /// </summary>
    /// <param name="returnUrl">The return url.</param>
    [TestCase(null)]
    [TestCase("")]
    [TestCase(" ")]
    [TestCase("javascript:alert(document.domain)")]
    [TestCase("JavaScript:alert(1)")]
    [TestCase(" javascript:alert(1)")]
    [TestCase("java\tscript:alert(1)")]
    [TestCase("java\nscript:alert(1)")]
    [TestCase("\u0001javascript:alert(1)")]
    [TestCase("data:text/html,<script>alert(1)</script>")]
    [TestCase("vbscript:msgbox(1)")]
    [TestCase("https://evil.example")]
    [TestCase("https:evil.example")]
    [TestCase("//evil.example")]
    [TestCase("/accounts")]
    [TestCase("\\\\evil.example")]
    [TestCase("\\/evil.example")]
    [TestCase("a\\..\\..\\evil")]
    public void UnsafeUrlIsRejected(string? returnUrl)
    {
        Assert.That(ReturnUrlValidator.GetSafeReturnUrl(returnUrl, BaseUri), Is.Empty);
    }
}

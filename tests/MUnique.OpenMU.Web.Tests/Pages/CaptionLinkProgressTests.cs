// <copyright file="CaptionLinkProgressTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Pages;

using System.Globalization;
using Bunit;
using MUnique.OpenMU.Persistence.Initialization.Captions;
using MUnique.OpenMU.Web.AdminPanel.Components;
using MUnique.OpenMU.Web.AdminPanel.Properties;

/// <summary>
/// Tests for the <see cref="CaptionLinkProgress"/> component.
/// </summary>
[TestFixture]
[NonParallelizable]
public class CaptionLinkProgressTests
{
    private BunitContext _context = null!;

    private CultureInfo _previousUiCulture = null!;

    /// <summary>
    /// Sets up the test context.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        this._previousUiCulture = CultureInfo.CurrentUICulture;
        CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en");
        this._context = new BunitContext();
    }

    /// <summary>
    /// Restores the culture and disposes the test context.
    /// </summary>
    [TearDown]
    public void Teardown()
    {
        this._context.Dispose();
        CultureInfo.CurrentUICulture = this._previousUiCulture;
    }

    /// <summary>
    /// The progress shows the steps, marks the current one, and shows the elapsed time.
    /// </summary>
    [Test]
    public void ShowsStepsAndElapsedTime()
    {
        var cut = this._context.Render<CaptionLinkProgress>(parameters => parameters
            .Add(p => p.CurrentStep, CaptionLinkStep.CreatingReferenceConfiguration)
            .Add(p => p.Elapsed, TimeSpan.FromSeconds(75))
            .Add(p => p.StepElapsed, TimeSpan.Zero));

        var steps = cut.FindAll("li");
        Assert.That(steps, Has.Count.EqualTo(4));
        Assert.That(steps[0].QuerySelector(".oi-check"), Is.Not.Null, "completed step");
        Assert.That(steps[1].QuerySelector(".spinner-border"), Is.Not.Null, "current step");
        Assert.That(steps[1].TextContent, Does.Contain(Resources.CaptionLinkStep_CreatingReferenceConfiguration));
        Assert.That(cut.Markup, Does.Contain(string.Format(Resources.CaptionsLinkingElapsed, "1:15")));
        Assert.That(GetPercent(cut), Is.EqualTo(10));
    }

    /// <summary>
    /// Within a long step, the progress keeps moving with the elapsed time, but doesn't reach the next step.
    /// </summary>
    [Test]
    public void ProgressMovesWithinStepButDoesNotReachNextStep()
    {
        int Render(TimeSpan stepElapsed) => GetPercent(this._context.Render<CaptionLinkProgress>(parameters => parameters
            .Add(p => p.CurrentStep, CaptionLinkStep.CreatingReferenceConfiguration)
            .Add(p => p.StepElapsed, stepElapsed)));

        var early = Render(TimeSpan.FromSeconds(5));
        var later = Render(TimeSpan.FromSeconds(20));
        var veryLate = Render(TimeSpan.FromMinutes(10));

        Assert.That(early, Is.GreaterThan(10));
        Assert.That(later, Is.GreaterThan(early));
        Assert.That(veryLate, Is.LessThan(85), "doesn't claim the reference step is done");
    }

    /// <summary>
    /// When completed, the progress is full.
    /// </summary>
    [Test]
    public void CompletedIsFull()
    {
        var cut = this._context.Render<CaptionLinkProgress>(parameters => parameters.Add(p => p.CurrentStep, CaptionLinkStep.Completed));

        Assert.That(GetPercent(cut), Is.EqualTo(100));
        Assert.That(cut.FindAll("li .oi-check"), Has.Count.EqualTo(4));
    }

    private static int GetPercent(IRenderedComponent<CaptionLinkProgress> cut)
    {
        return int.Parse(cut.Find("[role=progressbar]").GetAttribute("aria-valuenow")!, CultureInfo.InvariantCulture);
    }
}

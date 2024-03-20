// <copyright file="ViewPlugInContainerTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameServer;
using MUnique.OpenMU.GameServer.RemoteView;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Unit tests for the <see cref="ViewPlugInContainer"/>.
/// </summary>
[TestFixture]
public class ViewPlugInContainerTest
{
    private static readonly ClientVersion Season6E3English = new (6, 3, ClientLanguage.English);

    private static readonly ClientVersion Season9E2English = new (9, 2, ClientLanguage.English);

    /// <summary>
    /// A test interface for our test plugin implementations.
    /// </summary>
    public interface ISomeViewPlugIn : IViewPlugIn
    {
    }

    /// <summary>
    /// Tests if the the plug in of correct version is selected when the plugin for the exact version is available.
    /// </summary>
    [Test]
    public void SelectPlugInOfCorrectVersionWhenExactVersionIsAvailable()
    {
        var manager = new PlugInManager(null, new NullLoggerFactory(), null, null);
        manager.RegisterPlugIn<ISomeViewPlugIn, Season1PlugIn>();
        manager.RegisterPlugIn<ISomeViewPlugIn, Season6PlugIn>();
        manager.RegisterPlugIn<ISomeViewPlugIn, Season9PlugIn>();
        var containerForSeason6 = new ViewPlugInContainer(this.CreatePlayer(manager), Season6E3English, manager);
        Assert.That(containerForSeason6.GetPlugIn<ISomeViewPlugIn>()!.GetType(), Is.EqualTo(typeof(Season6PlugIn)));
    }

    /// <summary>
    /// Tests if the the plug in of correct version is selected when only plugins for lower versions are available.
    /// </summary>
    [Test]
    public void SelectPlugInOfCorrectVersionWhenLowerVersionsAreAvailable()
    {
        var manager = new PlugInManager(null, new NullLoggerFactory(), null, null);
        manager.RegisterPlugIn<ISomeViewPlugIn, InvariantSeasonPlugIn>();
        manager.RegisterPlugIn<ISomeViewPlugIn, Season1PlugIn>();
        manager.RegisterPlugIn<ISomeViewPlugIn, Season6PlugIn>();
        var containerForSeason9 = new ViewPlugInContainer(this.CreatePlayer(manager), Season9E2English, manager);
        Assert.That(containerForSeason9.GetPlugIn<ISomeViewPlugIn>()!.GetType(), Is.EqualTo(typeof(Season6PlugIn)));
    }

    /// <summary>
    /// Tests if plugins of the correct language are selected.
    /// </summary>
    [Test]
    public void SelectPlugInOfCorrectLanguage()
    {
        var manager = new PlugInManager(null, new NullLoggerFactory(), null, null);
        manager.RegisterPlugIn<ISomeViewPlugIn, Season6PlugInOfSomeOtherLanguage>();
        manager.RegisterPlugIn<ISomeViewPlugIn, Season6PlugIn>();
        var containerForSeason6English = new ViewPlugInContainer(this.CreatePlayer(manager), Season6E3English, manager);
        Assert.That(containerForSeason6English.GetPlugIn<ISomeViewPlugIn>()!.GetType(), Is.EqualTo(typeof(Season6PlugIn)));
    }

    /// <summary>
    /// Tests if plugins of invariant language and version are selected.
    /// </summary>
    [Test]
    public void SelectInvariantPlugIn()
    {
        var manager = new PlugInManager(null, new NullLoggerFactory(), null, null);
        manager.RegisterPlugIn<ISomeViewPlugIn, InvariantSeasonPlugIn>();
        var containerForSeason6English = new ViewPlugInContainer(this.CreatePlayer(manager), Season6E3English, manager);
        Assert.That(containerForSeason6English.GetPlugIn<ISomeViewPlugIn>()!.GetType(), Is.EqualTo(typeof(InvariantSeasonPlugIn)));
    }

    /// <summary>
    /// Tests if another plugin is getting 'effective' when the currently effective plugin gets deactivated.
    /// </summary>
    [Test]
    public void SelectPlugInAfterDeactivation()
    {
        var manager = new PlugInManager(null, new NullLoggerFactory(), null, null);
        manager.RegisterPlugIn<ISomeViewPlugIn, InvariantSeasonPlugIn>();
        manager.RegisterPlugIn<ISomeViewPlugIn, Season1PlugIn>();
        manager.RegisterPlugIn<ISomeViewPlugIn, Season6PlugIn>();
        var containerForSeason9 = new ViewPlugInContainer(this.CreatePlayer(manager), Season9E2English, manager);

        manager.DeactivatePlugIn<Season6PlugIn>();
        Assert.That(containerForSeason9.GetPlugIn<ISomeViewPlugIn>()!.GetType(), Is.EqualTo(typeof(Season1PlugIn)));
    }

    /// <summary>
    /// Tests if the language specific plugin has priority over the invariant one.
    /// </summary>
    [Test]
    public void SelectLanguageSpecificOverInvariant()
    {
        var manager = new PlugInManager(null, new NullLoggerFactory(), null, null);
        manager.RegisterPlugIn<ISomeViewPlugIn, Season6PlugIn>();
        manager.RegisterPlugIn<ISomeViewPlugIn, Season6PlugInInvariant>();
        var containerForSeason9 = new ViewPlugInContainer(this.CreatePlayer(manager), Season9E2English, manager);

        Assert.That(containerForSeason9.GetPlugIn<ISomeViewPlugIn>()!.GetType(), Is.EqualTo(typeof(Season6PlugIn)));
    }

    private RemotePlayer CreatePlayer(PlugInManager plugInManager)
    {
        var gameContext = new Mock<IGameServerContext>();
        gameContext.Setup(c => c.PersistenceContextProvider).Returns(new Mock<IPersistenceContextProvider>().Object);
        gameContext.Setup(c => c.Configuration).Returns(new GameConfiguration());
        gameContext.Setup(c => c.PlugInManager).Returns(plugInManager);
        gameContext.Setup(c => c.LoggerFactory).Returns(new NullLoggerFactory());
        return new RemotePlayer(gameContext.Object, new Mock<IConnection>().Object, default);
    }

    /// <summary>
    /// A plugin which is version/language invariant.
    /// </summary>
    [PlugIn("Season Invariant Test PlugIn", "")]
    [Guid("96A3FED8-0112-4CFC-A717-70EEEEBE859A")]
    public class InvariantSeasonPlugIn : ISomeViewPlugIn
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvariantSeasonPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public InvariantSeasonPlugIn(RemotePlayer player)
        {
        }
    }

    /// <summary>
    /// A test plugin for season 1.
    /// </summary>
    [PlugIn("Season 1 Test PlugIn", "")]
    [Guid("8CA21647-85D5-43BB-A8F9-3543D0E02176")]
    [MinimumClient(1, 0, ClientLanguage.English)]
    public class Season1PlugIn : ISomeViewPlugIn
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Season1PlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public Season1PlugIn(RemotePlayer player)
        {
        }
    }

    /// <summary>
    /// A test plugin for season 6.
    /// </summary>
    [PlugIn("Season 6 Test PlugIn", "")]
    [Guid("7C029691-BB22-4B5D-BE96-924537E43EB2")]
    [MinimumClient(6, 3, ClientLanguage.English)]
    public class Season6PlugIn : ISomeViewPlugIn
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Season6PlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public Season6PlugIn(RemotePlayer player)
        {
        }
    }

    /// <summary>
    /// A test plugin for season 6, with invariant language.
    /// </summary>
    [PlugIn("Season 6 Test PlugIn, Invariant language", "")]
    [Guid("D58A6AC6-A804-4321-9422-0911EDC82867")]
    [MinimumClient(6, 3, ClientLanguage.Invariant)]
    public class Season6PlugInInvariant : ISomeViewPlugIn
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Season6PlugInInvariant"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public Season6PlugInInvariant(RemotePlayer player)
        {
        }
    }

    /// <summary>
    /// A test plugin for season 6, but for another client language.
    /// </summary>
    [PlugIn("Season 6 Test PlugIn", "")]
    [Guid("05C47D9E-F0A0-48B3-9FFF-22CF43B20494")]
    [MinimumClient(6, 3, (ClientLanguage)42)]
    public class Season6PlugInOfSomeOtherLanguage : ISomeViewPlugIn
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Season6PlugInOfSomeOtherLanguage"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public Season6PlugInOfSomeOtherLanguage(RemotePlayer player)
        {
        }
    }

    /// <summary>
    /// Test plugin for season 9.
    /// </summary>
    [PlugIn("Season 9 Test PlugIn", "")]
    [Guid("82AC1C9A-F3D0-4196-A3CD-6CB36AA2D914")]
    [MinimumClient(9, 2, ClientLanguage.English)]
    public class Season9PlugIn : ISomeViewPlugIn
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Season9PlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public Season9PlugIn(RemotePlayer player)
        {
        }
    }
}
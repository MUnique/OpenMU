// <copyright file="PlugInTypeExtensionTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.PlugIns.Tests
{
    using MUnique.OpenMU.GameServer;
    using MUnique.OpenMU.Network.PlugIns;
    using NUnit.Framework;

    /// <summary>
    /// Tests for the <see cref="PlugInTypeExtensions"/>.
    /// </summary>
    [TestFixture]
    public class PlugInTypeExtensionTest
    {
        /// <summary>
        /// Tests the maximum client version requirement.
        /// </summary>
        [Test]
        public void ConsiderMaximumClientVersion()
        {
            Assert.That(new ClientVersion(6, 3, ClientLanguage.English).IsPlugInSuitable(typeof(PlugInTypeUntilSeason1)), Is.False);
            Assert.That(new ClientVersion(1, 1, ClientLanguage.English).IsPlugInSuitable(typeof(PlugInTypeUntilSeason1)), Is.False);
            Assert.That(new ClientVersion(1, 0, ClientLanguage.English).IsPlugInSuitable(typeof(PlugInTypeUntilSeason1)), Is.True);
            Assert.That(new ClientVersion(0, 99, ClientLanguage.English).IsPlugInSuitable(typeof(PlugInTypeUntilSeason1)), Is.True);
        }

        /// <summary>
        /// Tests the minimum client version requirement.
        /// </summary>
        [Test]
        public void ConsiderMinimumClientVersion()
        {
            Assert.That(new ClientVersion(6, 3, ClientLanguage.English).IsPlugInSuitable(typeof(PlugInTypeAfterSeason2)), Is.True);
            Assert.That(new ClientVersion(2, 0, ClientLanguage.English).IsPlugInSuitable(typeof(PlugInTypeAfterSeason2)), Is.True);
            Assert.That(new ClientVersion(1, 255, ClientLanguage.English).IsPlugInSuitable(typeof(PlugInTypeAfterSeason2)), Is.False);
        }

        /// <summary>
        /// Tests the minimum and maximum client version requirements in combination.
        /// </summary>
        [Test]
        public void ConsiderMinimumAndMaximumClientVersion()
        {
            Assert.That(new ClientVersion(6, 3, ClientLanguage.English).IsPlugInSuitable(typeof(PlugInTypeBetweenSeason1And2)), Is.False);
            Assert.That(new ClientVersion(2, 0, ClientLanguage.English).IsPlugInSuitable(typeof(PlugInTypeBetweenSeason1And2)), Is.True);
            Assert.That(new ClientVersion(1, 0, ClientLanguage.English).IsPlugInSuitable(typeof(PlugInTypeBetweenSeason1And2)), Is.True);
            Assert.That(new ClientVersion(0, 255, ClientLanguage.English).IsPlugInSuitable(typeof(PlugInTypeBetweenSeason1And2)), Is.False);

            Assert.That(new ClientVersion(1, 1, ClientLanguage.English).IsPlugInSuitable(typeof(PlugInTypeAtExactlySeason1)), Is.False);
            Assert.That(new ClientVersion(1, 0, ClientLanguage.English).IsPlugInSuitable(typeof(PlugInTypeAtExactlySeason1)), Is.True);
            Assert.That(new ClientVersion(0, 255, ClientLanguage.English).IsPlugInSuitable(typeof(PlugInTypeAtExactlySeason1)), Is.False);
        }

        /// <summary>
        /// Tests if minimum and maximum client version requirements are not inherited from base classes.
        /// </summary>
        [Test]
        public void DontConsiderInheritedAttributes()
        {
            Assert.That(new ClientVersion(1, 1, ClientLanguage.English).IsPlugInSuitable(typeof(PlugInWithInheritedAttribute)), Is.True);
            Assert.That(new ClientVersion(0, 255, ClientLanguage.English).IsPlugInSuitable(typeof(PlugInWithInheritedAttribute)), Is.True);
        }

        [MaximumClient(1, 0, ClientLanguage.Invariant)]
        private class PlugInTypeUntilSeason1
        {
        }

        [MinimumClient(2, 0, ClientLanguage.Invariant)]
        private class PlugInTypeAfterSeason2
        {
        }

        [MinimumClient(1, 0, ClientLanguage.Invariant)]
        [MaximumClient(2, 0, ClientLanguage.Invariant)]
        private class PlugInTypeBetweenSeason1And2
        {
        }

        [MaximumClient(1, 0, ClientLanguage.Invariant)]
        [MinimumClient(1, 0, ClientLanguage.Invariant)]
        private class PlugInTypeAtExactlySeason1
        {
        }

        private class PlugInWithInheritedAttribute : PlugInTypeAtExactlySeason1
        {
        }
    }
}

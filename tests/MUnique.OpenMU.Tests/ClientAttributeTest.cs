// <copyright file="ClientAttributeTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests
{
    using System;
    using MUnique.OpenMU.GameServer;
    using MUnique.OpenMU.Network.PlugIns;
    using NUnit.Framework;

    /// <summary>
    /// Tests for <see cref="MinimumClientAttribute"/>.
    /// </summary>
    [TestFixture]
    public class ClientAttributeTest
    {
        private static readonly MinimumClientAttribute Season6E3English = new (6, 3, ClientLanguage.English);
        private static readonly MinimumClientAttribute Season6E3Japanese = new (6, 3, ClientLanguage.Japanese);

        private static readonly MinimumClientAttribute Season9E2English = new (9, 2, ClientLanguage.English);
        private static readonly MinimumClientAttribute Season9E2EnglishOtherInstance = new (9, 2, ClientLanguage.English);

        /// <summary>
        /// Tests less than using <see cref="IComparable"/>.
        /// </summary>
        [Test]
        public void LessThan()
        {
            Assert.That(Season6E3English, Is.LessThan(Season9E2English));
        }

        /// <summary>
        /// Tests greater than using <see cref="IComparable"/>.
        /// </summary>
        [Test]
        public void GreaterThan()
        {
            Assert.That(Season9E2English, Is.GreaterThan(Season6E3English));
        }

        /// <summary>
        /// Tests equality.
        /// </summary>
        [Test]
        public void Equal()
        {
            Assert.That(Season9E2English, Is.EqualTo(Season9E2English));
        }

        /// <summary>
        /// Tests non equality when version differs.
        /// </summary>
        [Test]
        public void NotEqualWhenVersionDiffers()
        {
            Assert.That(Season6E3English, Is.Not.EqualTo(Season9E2English));
        }

        /// <summary>
        /// Tests non equality when language differs.
        /// </summary>
        [Test]
        public void NotEqualWhenLanguageDiffers()
        {
            Assert.That(Season6E3English, Is.Not.EqualTo(Season6E3Japanese));
        }

        /// <summary>
        /// Tests less than using the overloaded operator.
        /// </summary>
        [Test]
        public void OperatorLessThan()
        {
            Assert.That(Season6E3English < Season9E2English, Is.True);
        }

        /// <summary>
        /// Tests greater than using the overloaded operator.
        /// </summary>
        [Test]
        public void OperatorGreaterThan()
        {
            Assert.That(Season9E2English > Season6E3English, Is.True);
        }

        /// <summary>
        /// Tests less than using the overloaded operator.
        /// </summary>
        [Test]
        public void OperatorLessOrEqualThan()
        {
            Assert.That(Season6E3English <= Season9E2English, Is.True);
        }

        /// <summary>
        /// Tests greater than using the overloaded operator.
        /// </summary>
        [Test]
        public void OperatorGreaterOrEqualThan()
        {
            Assert.That(Season9E2English >= Season6E3English, Is.True);
        }

        /// <summary>
        /// Tests less than using the overloaded operator.
        /// </summary>
        [Test]
        public void OperatorLessOrEqualThanWhenEqual()
        {
            Assert.That(Season9E2EnglishOtherInstance <= Season9E2English, Is.True);
        }

        /// <summary>
        /// Tests greater than using the overloaded operator.
        /// </summary>
        [Test]
        public void OperatorGreaterOrEqualThanWhenEqual()
        {
            Assert.That(Season9E2English >= Season9E2EnglishOtherInstance, Is.True);
        }

        /// <summary>
        /// Tests equality using the overloaded operator.
        /// </summary>
        [Test]
        public void OperatorEqual()
        {
            Assert.That(Season9E2English == Season9E2EnglishOtherInstance, Is.True);
        }

        /// <summary>
        /// Tests non-equality using the overloaded operator when version differs.
        /// </summary>
        [Test]
        public void OperatorNotEqualWhenVersionDiffers()
        {
            Assert.That(Season6E3English != Season9E2English, Is.True);
        }

        /// <summary>
        /// Tests non-equality using the overloaded operator when language differs.
        /// </summary>
        [Test]
        public void OperatorNotEqualWhenLanguageDiffers()
        {
            Assert.That(Season6E3English != Season6E3Japanese, Is.True);
        }
    }
}
// <copyright file="AttributeDefinitionTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AttributeSystem.Tests
{
    using System;
    using NUnit.Framework;

    /// <summary>
    /// Tests for the <see cref="AttributeDefinition"/>.
    /// </summary>
    [TestFixture]
    public class AttributeDefinitionTests
    {
        /// <summary>
        /// Tests if different instances with the same id are treated as being equal.
        /// </summary>
        [Test]
        public void Equality()
        {
            var attributeId = new Guid("86A31E67-8696-43C5-A9FE-CA85E1E07017");
            var definition1 = new AttributeDefinition(attributeId, "foo", "bar");
            var definition2 = new AttributeDefinition(attributeId, "test", "123");
            Assert.That(definition1 == definition2, Is.True);
            Assert.That(definition1, Is.EqualTo(definition2));
            Assert.That(definition1.GetHashCode(), Is.EqualTo(definition2.GetHashCode()));
        }
    }
}
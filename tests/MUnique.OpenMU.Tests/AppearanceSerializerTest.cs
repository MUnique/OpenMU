// <copyright file="AppearanceSerializerTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests
{
    using System.Collections.Generic;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Configuration.Items;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameServer.RemoteView;
    using NUnit.Framework;
    using Rhino.Mocks;

    /// <summary>
    /// Tests the <see cref="AppearanceSerializer"/>.
    /// </summary>
    [TestFixture]
    public class AppearanceSerializerTest
    {
        /// <summary>
        /// Tests how a new (naked) dark knight with small axe would be serialized.
        /// </summary>
        [Test]
        public void NewDarkKnightWithSmallAxe()
        {
            // character list with dk and small axe: C1 2A F3 00 00 00 01 00 00 74 65 73 74 30 00 00 00 00 00 01 0E 00 00 20 00 FF FF FF FF 00 00 00 F8 00 00 20 FF FF FF 00 00 00
            // (this packet is not 100 % correct - it wears some strange thing as wing)
            var serializer = new AppearanceSerializer();
            var appeareanceData = MockRepository.GenerateStub<IAppearanceData>();
            appeareanceData.Stub(a => a.CharacterClass).Return(new CharacterClass() { Number = 0x20 >> 3 }); // Dark Knight;
            appeareanceData.Stub(a => a.EquippedItems).Return(this.GetSmallAxeEquipped());
            var data = serializer.GetAppearanceData(appeareanceData);
            var expected = new byte[] { 0x20, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0xF8, 0x00, 0x00, 0x20, 0xFF, 0xFF, 0xFF, 0x00, 0x00 };
            Assert.That(data, Is.EquivalentTo(expected));
            //// Expected: equivalent to< 32, 0, 255, 255, 255, 255, 0, 0, 0, 248, 0, 0, 32, 255, 255, 255, 0, 0 >
            ////              But was:  < 32, 0, 255, 255, 255, 243, 0, 0, 0, 248, 0, 0, 16, 255, 255, 255, 0, 0 >
        }

        private IEnumerable<ItemAppearance> GetSmallAxeEquipped()
        {
            yield return new ItemAppearance { Definition = new ItemDefinition { Group = 1 } };
        }
    }
}

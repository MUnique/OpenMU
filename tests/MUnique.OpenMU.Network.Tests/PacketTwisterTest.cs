// <copyright file="PacketTwisterTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Tests
{
    using System;
    using System.Linq;
    using MUnique.OpenMU.Network.PacketTwister;
    using NUnit.Framework;

    /// <summary>
    /// Tests for the  <see cref="PacketTwistRunner"/>.
    /// </summary>
    [TestFixture]
    public class PacketTwisterTest
    {
        /// <summary>
        /// Tests encryption and decryption using the <see cref="PacketTwistRunner"/>.
        /// </summary>
        [Test]
        public void EncryptDecryptWithPacketTwister()
        {
            var decrypted = Convert.FromBase64String("w7gAudHEjjSP53H6Rkp3oXj7B9z+rVDR2f0Is4bvsIsUL3RM/aTDB2FX9YG3Hkboy1Z1JThot558MeDTvNuunzfl5RbWK6TTOP97prjPGbq3IOcweopTq3fVz8vD8EuFqVVJ0jgvEZ+xoe047RHmrRgmG5zzfSWtkTmeAVzZD0i09f1jhUeBiA5HfticGr5m7iGzndSvkSwvm0D/kRBD15GlhPgTgyfQpJONrP5NEHd7NxI6JnJzBQ==");
            var packetTwister = new PacketTwister.PacketTwistRunner();
            for (byte packetType = 0; packetType < byte.MaxValue; packetType++)
            {
                decrypted[2] = packetType;
                var result = decrypted.ToArray();
                packetTwister.Encrypt(result);
                packetTwister.Decrypt(result);
                CompareArrays(decrypted, result);
            }
        }

        private static void CompareArrays(byte[] expected, byte[] actual)
        {
            Assert.That(actual.Length, Is.EqualTo(expected.Length));
            for (int i = 0; i < actual.Length; i++)
            {
                Assert.That(actual[i], Is.EqualTo(expected[i]), "index {0}, packet type {1}", i, expected[expected.GetPacketHeaderSize()]);
            }
        }
    }
}

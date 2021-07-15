// <copyright file="Version075NetworkEncryptionFactoryPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PlugIns
{
    using System.IO.Pipelines;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.SimpleModulus;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// A plugin which provides network encryptors and decryptors for game clients of version 0.75 (server version 0.65).
    /// </summary>
    /// <remarks>
    /// This early and very old version doesn't seem to have xor 32 encryption yet.
    /// However, it uses simple modulus with some other keys.
    /// </remarks>
    [PlugIn("Network Encryption - 0.75", "A plugin which provides network encryptors and decryptors for game clients of version 0.75 (server version 0.65)")]
    [Guid("E83F05AC-18D4-4714-AEFB-2F4F3B37951C")]
    public class Version075NetworkEncryptionFactoryPlugIn : INetworkEncryptionFactoryPlugIn
    {
        /// <summary>
        /// The simple modulus keys which are used to transfer data from client to server.
        /// </summary>
        /// <remarks>
        /// Enc1.dat (client file):
        /// Modulus: 102510, 92475, 128027, 81329, 70183, 106846, 85665, 114515, 212247, 82188, 154403, 71042, 122076, 157628, 74267, 110930
        /// Crypt: 24347, 21878, 31058, 35010, 15375, 24555, 15976, 18344, 26249, 24973, 10565, 12143, 6481, 21283, 21147, 30327
        /// XOR: 2573, 25253, 28177, 14604, 17198, 36180, 26560, 26251, 31434, 5415, 30023, 60392, 23094, 3763, 24151, 8640
        /// Calculated Crypt Key: 33683, 33092, 21827, 3905, 1269, 2019, 27706, 35839, 10730, 34033, 22258, 32587, 29629, 25211, 2353, 6873.
        /// </remarks>
        private static readonly SimpleModulusKeys ClientToServerKeys = new (
            new uint[] { 102510, 92475, 128027, 81329, 70183, 106846, 85665, 114515, 212247, 82188, 154403, 71042, 122076, 157628, 74267, 110930 },
            new uint[] { 2573, 25253, 28177, 14604, 17198, 36180, 26560, 26251, 31434, 5415, 30023, 60392, 23094, 3763, 24151, 8640 },
            new uint[] { 24347, 21878, 31058, 35010, 15375, 24555, 15976, 18344, 26249, 24973, 10565, 12143, 6481, 21283, 21147, 30327 },
            new uint[] { 33683, 33092, 21827, 3905, 1269, 2019, 27706, 35839, 10730, 34033, 22258, 32587, 29629, 25211, 2353, 6873 });

        /// <summary>
        /// The simple modulus keys which are used to transfer data from server to client.
        /// </summary>
        /// <remarks>
        /// Dec2.dat (client file):
        /// Modulus: 85408, 122071, 147588, 136442, 78598, 187476, 81823, 118486, 71788, 107340, 115009, 103863, 82682, 71536, 97053, 170379
        /// Crypt: 32265, 27110, 8593, 9807, 46581, 19561, 30064, 13133, 20169, 24647, 7162, 28304, 40819, 44985, 19577, 1178
        /// XOR: 31800, 64655, 7620, 59986, 36846, 57613, 50524, 22089, 23951, 62961, 18047, 1584, 53418, 60300, 54445, 2206
        /// Calculated Crypt Key: 23289, 24896, 28597, 12911, 11889, 6661, 27698, 28699, 35789, 4403, 11032, 24164, 17661, 30793, 21347, 5930.
        /// </remarks>
        private static readonly SimpleModulusKeys ServerToClientKeys = new (
            new uint[] { 85408, 122071, 147588, 136442, 78598, 187476, 81823, 118486, 71788, 107340, 115009, 103863, 82682, 71536, 97053, 170379 },
            new uint[] { 31800, 64655, 7620, 59986, 36846, 57613, 50524, 22089, 23951, 62961, 18047, 1584, 53418, 60300, 54445, 2206 },
            new uint[] { 23289, 24896, 28597, 12911, 11889, 6661, 27698, 28699, 35789, 4403, 11032, 24164, 17661, 30793, 21347, 5930 },
            new uint[] { 32265, 27110, 8593, 9807, 46581, 19561, 30064, 13133, 20169, 24647, 7162, 28304, 40819, 44985, 19577, 1178 });

        /// <inheritdoc />
        public ClientVersion Key { get; } = new (0, 75, ClientLanguage.Invariant);

        /// <inheritdoc />
        public IPipelinedEncryptor CreateEncryptor(PipeWriter target, DataDirection direction) => new PipelinedSimpleModulusEncryptor(target, direction == DataDirection.ClientToServer ? ClientToServerKeys : ServerToClientKeys);

        /// <inheritdoc />
        public IPipelinedDecryptor CreateDecryptor(PipeReader source, DataDirection direction) => new PipelinedSimpleModulusDecryptor(source, direction == DataDirection.ClientToServer ? ClientToServerKeys : ServerToClientKeys);
    }
}
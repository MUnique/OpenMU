// <copyright file="Program.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.SimpleModulusKeyGenerator
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using MUnique.OpenMU.Network.SimpleModulus;

    /// <summary>
    /// The main entry point class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Main entry point of the program.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The async task.</returns>
        public static async Task Main(string[] args)
        {
            if (args.Length == 1)
            {
                FindOtherKey(args[0]);
            }
            else
            {
                await GenerateNewKeyPair().ConfigureAwait(false);
            }
        }

        private static void FindOtherKey(string keyFilePath)
        {
            var serializer = new SimpleModulusKeySerializer();

            if (serializer.TryDeserialize(keyFilePath, out var modulusKey, out var cryptKey, out var xorKey))
            {
                Console.WriteLine($"Loaded Key:");
                Console.WriteLine($"   Modulus: {string.Join(", ", modulusKey.Select(k => k.ToString()))}");
                Console.WriteLine($"     Crypt: {string.Join(", ", cryptKey.Select(k => k.ToString()))}");
                Console.WriteLine($"       XOR: {string.Join(", ", xorKey.Select(k => k.ToString()))}");
                var generator = new SimpleModulusKeyGenerator();
                var otherCryptKey = generator.FindOtherKey(modulusKey, cryptKey);
                var file = new FileInfo(keyFilePath);
                var otherFileName = GetOtherFileName(file.Name);
                var otherFilePath = file.DirectoryName is null ? otherFileName : Path.Combine(file.DirectoryName, otherFileName);

                Console.WriteLine($"Calculated Crypt Key: {string.Join(", ", otherCryptKey.Select(k => k.ToString()))}");
                Console.WriteLine($"To save the calculated key file '{otherFilePath}' press any key");
                Console.ReadKey(true);
                serializer.Serialize(otherFilePath, modulusKey, otherCryptKey, xorKey);
                Console.WriteLine("Key saved");
            }
            else
            {
                throw new ArgumentException("Keys could not be read, file too small?", nameof(keyFilePath));
            }
        }

        private static string GetOtherFileName(string fileName)
        {
            if (fileName.StartsWith("Enc", StringComparison.InvariantCultureIgnoreCase))
            {
                return fileName.Replace("Enc", "Dec", StringComparison.InvariantCultureIgnoreCase);
            }

            if (fileName.StartsWith("Dec", StringComparison.InvariantCultureIgnoreCase))
            {
                return fileName.Replace("Dec", "Enc", StringComparison.InvariantCultureIgnoreCase);
            }

            throw new ArgumentException("File name does not begin with 'Enc' or 'Dec'.", nameof(fileName));
        }

        private static async Task GenerateNewKeyPair()
        {
            var generator = new SimpleModulusKeyGenerator();
            var result = await generator.GenerateKeys().ConfigureAwait(false);

            Console.WriteLine("Generated key pair:");
            Console.WriteLine(result);
            Console.WriteLine(string.Empty);
            Console.WriteLine("If you want to save them, enter a number and hit enter. They will be saved as 'EncX.dat' and 'DecX.dat'.");
            if (int.TryParse(Console.ReadLine(), out int number))
            {
                var serializer = new SimpleModulusKeySerializer();
                serializer.Serialize($"Enc{number}.dat", result.ModulusKey, result.EncryptKey, result.XorKey);
                serializer.Serialize($"Dec{number}.dat", result.ModulusKey, result.DecryptKey, result.XorKey);
                Console.WriteLine("Keys saved, press any key to exit");
                Console.ReadKey(true);
            }
            else
            {
                Console.WriteLine("Not a valid number.");
            }
        }
    }
}

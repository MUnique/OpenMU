// <copyright file="Program.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.SimpleModulusKeyGenerator
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using MUnique.OpenMU.Network.SimpleModulus;

    /// <summary>
    /// The main entry point class.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main entry point of the program.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static async Task Main(string[] args)
        {
            if (args.Length == 1)
            {
                FindOtherKey(args[0]);
            }
            else
            {
                await GenerateNewKeyPair();
            }
        }

        private static void FindOtherKey(string keyFilePath)
        {
            var serializer = new SimpleModulusKeySerializer();

            serializer.Deserialize(keyFilePath, out uint[] modkey, out uint[] cryptKey, out uint[] xorKey);

            var generator = new SimpleModulusKeyGenerator();
            var otherCryptKey = generator.FindOtherKey(modkey, cryptKey);
            var file = new FileInfo(keyFilePath);
            var otherFileName = GetOtherFileName(file.Name);
            var otherFilePath = Path.Combine(file.DirectoryName, otherFileName);
            Console.WriteLine($"Calculation successful. To save the calculated key to file '{otherFilePath}' press any key");
            Console.ReadKey(true);
            serializer.Serialize(otherFilePath, modkey, otherCryptKey, xorKey);
            Console.WriteLine("Key saved");
        }

        private static string GetOtherFileName(string fileName)
        {
            if (fileName.StartsWith("Enc", StringComparison.InvariantCultureIgnoreCase))
            {
                return fileName.Replace("Enc", "Dec");
            }

            if (fileName.StartsWith("Dec", StringComparison.InvariantCultureIgnoreCase))
            {
                return fileName.Replace("Dec", "Enc");
            }

            throw new Exception("File name does not begin with 'Enc' or 'Dec'.");
        }

        private static async Task GenerateNewKeyPair()
        {
            var generator = new SimpleModulusKeyGenerator();
            var result = await generator.GenerateKeys();

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

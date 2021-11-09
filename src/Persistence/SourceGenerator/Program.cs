// <copyright file="Program.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.SourceGenerator
{
    using System;
    using System.IO;

    /// <summary>
    /// Main entry point for the generator, if it's used as an executable.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point function.
        /// It expects two arguments:
        ///  - The target assembly name
        ///  - The target folder path for the generated code.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static int Main(params string[] args)
        {
            Console.WriteLine("Started generator with these parameters:");
            foreach (var arg in args)
            {
                Console.WriteLine(arg);
            }

            if (args.Length < 2)
            {
                Console.WriteLine("Can't generate code. Please add the project name and the target folder path as starting parameter.");
                return 1;
            }

            IUnboundSourceGenerator generator = args[0] switch
            {
                EfCoreModelGenerator.TargetAssemblyName => new EfCoreModelGenerator(),
                BasicModelGenerator.TargetAssemblyName => new BasicModelGenerator(),
                _ => null
            };

            if (generator is null)
            {
                Console.WriteLine($"No generator found for target assembly '{args[0]}'.");
                return 2;
            }

            var targetFolder = args[1];
            var previouslyGeneratedFile = Directory.EnumerateFiles(targetFolder, ".Generated.cs");
            foreach (var file in previouslyGeneratedFile)
            {
                File.Delete(file);
            }

            foreach (var (name, source) in generator.GenerateSources())
            {
                var filePath = Path.Combine(targetFolder, name + ".Generated.cs");
                Console.WriteLine($"Writing {filePath}");
                File.WriteAllText(filePath, source);
            }

            return 0;
        }
    }
}

// <copyright file="SyntaxTreeExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.PlugIns;

using System.Collections.Immutable;
using System.IO;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

/// <summary>
/// Extensions for <see cref="SyntaxTree"/>s.
/// </summary>
public static class SyntaxTreeExtensions
{
    private static IList<PortableExecutableReference>? _assemblyReferences;

    private static IList<PortableExecutableReference> AssemblyReferences
    {
        get
        {
            if (_assemblyReferences is { })
            {
                return _assemblyReferences;
            }

            var nitoAssemblies = Directory.EnumerateFiles(new FileInfo(typeof(SyntaxTreeExtensions).Assembly.Location).DirectoryName!, "Nito.*.dll")
                .Select(path => Assembly.LoadFrom(path))
                .ToList();
            
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic)
                .Select(a => a.Location)
                .ToImmutableHashSet();
            var separator = (Environment.OSVersion.Platform == PlatformID.MacOSX ||
                             Environment.OSVersion.Platform == PlatformID.Unix)
                ? ':'
                : ';';

            _assemblyReferences = (AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") as string)
                ?.Split(separator)
                .Select(path => MetadataReference.CreateFromFile(path))
                .Where(metaData => metaData.FilePath is not null && loadedAssemblies.Contains(metaData.FilePath))
                .ToList() ?? new List<PortableExecutableReference>();
            return _assemblyReferences;
        }
    }

    /// <summary>
    /// Compiles the <see cref="SyntaxTree"/> and load its assembly into memory.
    /// </summary>
    /// <param name="syntaxTree">The syntax tree.</param>
    /// <param name="assemblyName">Name of the assembly.</param>
    /// <returns>The compiled assembly.</returns>
    public static Assembly CompileAndLoad(this SyntaxTree syntaxTree, string assemblyName)
    {
        var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            .WithOverflowChecks(false)
            .WithOptimizationLevel(OptimizationLevel.Release)
            .WithUsings("System", "System.Collections.Generic", "System.Threading", "Nito.AsyncEx");
        var compilation = CSharpCompilation.Create(assemblyName, new[] { syntaxTree }, AssemblyReferences, options);
        using var stream = new MemoryStream();
        var result = compilation.Emit(stream);
        if (!result.Success)
        {
            var stringBuilder = new StringBuilder();
            result.Diagnostics
                .Where(m => m.Severity == DiagnosticSeverity.Error)
                .Select(d => $"{d.GetMessage()} @ {d.Location}")
                .ToList()
                .ForEach(message => stringBuilder.AppendLine(message));
            throw new ArgumentException(stringBuilder.ToString());
        }

        return Assembly.Load(stream.ToArray());
    }
}
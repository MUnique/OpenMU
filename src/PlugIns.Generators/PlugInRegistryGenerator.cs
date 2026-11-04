// <copyright file="PlugInRegistryGenerator.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.PlugIns.Generators;

using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

/// <summary>
/// Generates the registry of the plugins of an assembly, which are the classes marked with the <c>PlugInAttribute</c>.
/// For each plugin, the registry contains an action which registers it at each of its plugin interfaces,
/// by calling <c>PlugInManager.RegisterPlugIn&lt;TPlugInInterface, TPlugInClass&gt;()</c>.
/// </summary>
/// <remarks>
/// Without a generated registry, the <c>PlugInManager</c> searches all types of the assembly for plugins and calls
/// <c>RegisterPlugIn</c> by reflection. The registry contains the same plugins, in the same order as
/// <c>Assembly.DefinedTypes</c>, because the order of the plugins in a plugin point depends on it.
/// If a plugin can't be referenced by the generated code, no registry is generated for the assembly,
/// so that the behavior stays the same for it.
/// </remarks>
[Generator]
public class PlugInRegistryGenerator : IIncrementalGenerator
{
    private const string PlugInAttributeFullName = "MUnique.OpenMU.PlugIns.PlugInAttribute";

    private const string PlugInPointAttributeFullName = "MUnique.OpenMU.PlugIns.PlugInPointAttribute";

    private const string CustomPlugInContainerAttributeFullName = "MUnique.OpenMU.PlugIns.CustomPlugInContainerAttribute";

    private const string PlugInRegistryAttributeFullName = "MUnique.OpenMU.PlugIns.PlugInRegistryAttribute";

    private static readonly SymbolDisplayFormat TypeFormat = SymbolDisplayFormat.FullyQualifiedFormat;

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var registry = context.CompilationProvider.Select(static (compilation, cancellationToken) => CreateRegistryModel(compilation, cancellationToken));

        context.RegisterSourceOutput(registry, static (spc, model) =>
        {
            if (model is not null)
            {
                spc.AddSource("GeneratedPlugInRegistry.g.cs", SourceText.From(GenerateSource(model), Encoding.UTF8));
            }
        });
    }

    private static RegistryModel? CreateRegistryModel(Compilation compilation, CancellationToken cancellationToken)
    {
        if (compilation.GetTypeByMetadataName(PlugInAttributeFullName) is not { } plugInAttribute
            || compilation.GetTypeByMetadataName(PlugInPointAttributeFullName) is not { } plugInPointAttribute
            || compilation.GetTypeByMetadataName(CustomPlugInContainerAttributeFullName) is not { } customPlugInContainerAttribute
            || compilation.GetTypeByMetadataName(PlugInRegistryAttributeFullName) is null)
        {
            return null;
        }

        var plugIns = ImmutableArray.CreateBuilder<PlugInModel>();
        foreach (var type in GetTypesInDefinitionOrder(compilation.Assembly))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (type.TypeKind != TypeKind.Class || !HasPlugInAttribute(type, plugInAttribute))
            {
                continue;
            }

            if (type.IsGenericType
                || GetContainingTypes(type).Any(t => t.IsGenericType)
                || !compilation.IsSymbolAccessibleWithin(type, compilation.Assembly))
            {
                // The generated code can't reference this plugin, so the PlugInManager has to find the plugins by reflection.
                return null;
            }

            var interfaces = ImmutableArray.CreateBuilder<string>();
            foreach (var plugInInterface in type.AllInterfaces)
            {
                if (!plugInInterface.GetAttributes().Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, plugInPointAttribute)
                                                            || SymbolEqualityComparer.Default.Equals(a.AttributeClass, customPlugInContainerAttribute)))
                {
                    continue;
                }

                if (!compilation.IsSymbolAccessibleWithin(plugInInterface, compilation.Assembly))
                {
                    return null;
                }

                interfaces.Add(plugInInterface.ToDisplayString(TypeFormat));
            }

            plugIns.Add(new PlugInModel(type.ToDisplayString(TypeFormat), new EquatableArray<string>(interfaces.ToImmutable())));
        }

        if (plugIns.Count == 0)
        {
            return null;
        }

        var assemblyName = compilation.AssemblyName;
        var ns = assemblyName is not null && assemblyName.Split('.').All(SyntaxFacts.IsValidIdentifier) ? assemblyName : null;
        return new RegistryModel(ns, new EquatableArray<PlugInModel>(plugIns.ToImmutable()));
    }

    /// <summary>
    /// Gets the types of the assembly in the order in which the compiler emits their definitions,
    /// which is the order of <c>Assembly.DefinedTypes</c>: first the top level types, namespace by namespace,
    /// then the nested types, level by level.
    /// </summary>
    private static IEnumerable<INamedTypeSymbol> GetTypesInDefinitionOrder(IAssemblySymbol assembly)
    {
        var nestedTypes = new Queue<INamedTypeSymbol>();
        var namespaces = new Stack<INamespaceSymbol>();
        namespaces.Push(assembly.GlobalNamespace);
        while (namespaces.Count > 0)
        {
            foreach (var member in namespaces.Pop().GetMembers())
            {
                if (member is INamespaceSymbol ns)
                {
                    namespaces.Push(ns);
                }
                else if (member is INamedTypeSymbol type)
                {
                    yield return type;
                    EnqueueNestedTypes(type, nestedTypes);
                }
            }
        }

        while (nestedTypes.Count > 0)
        {
            var type = nestedTypes.Dequeue();
            yield return type;
            EnqueueNestedTypes(type, nestedTypes);
        }
    }

    private static void EnqueueNestedTypes(INamedTypeSymbol type, Queue<INamedTypeSymbol> nestedTypes)
    {
        foreach (var nestedType in type.GetTypeMembers())
        {
            nestedTypes.Enqueue(nestedType);
        }
    }

    /// <summary>
    /// Determines if the type or one of its base types is marked with the <c>PlugInAttribute</c>,
    /// because the attribute is inherited.
    /// </summary>
    private static bool HasPlugInAttribute(INamedTypeSymbol type, INamedTypeSymbol plugInAttribute)
    {
        for (var current = type; current is not null; current = current.BaseType)
        {
            if (current.GetAttributes().Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, plugInAttribute)))
            {
                return true;
            }
        }

        return false;
    }

    private static IEnumerable<INamedTypeSymbol> GetContainingTypes(INamedTypeSymbol symbol)
    {
        for (var containingType = symbol.ContainingType; containingType is not null; containingType = containingType.ContainingType)
        {
            yield return containingType;
        }
    }

    private static string GenerateSource(RegistryModel model)
    {
        var registryName = model.Namespace is null ? "global::GeneratedPlugInRegistry" : "global::" + model.Namespace + ".GeneratedPlugInRegistry";
        var builder = new StringBuilder();
        builder.AppendLine("// <auto-generated />");
        builder.AppendLine("#nullable enable");
        builder.AppendLine();
        builder.Append("[assembly: global::MUnique.OpenMU.PlugIns.PlugInRegistry(typeof(").Append(registryName).AppendLine("))]");
        builder.AppendLine();
        if (model.Namespace is not null)
        {
            builder.Append("namespace ").Append(model.Namespace).AppendLine(";");
            builder.AppendLine();
        }

        builder.AppendLine("/// <summary>The registry of the plugins of this assembly.</summary>");
        builder.AppendLine("[global::System.CodeDom.Compiler.GeneratedCode(\"MUnique.OpenMU.PlugIns.Generators\", \"1.0\")]");
        builder.AppendLine("internal sealed class GeneratedPlugInRegistry : global::MUnique.OpenMU.PlugIns.IPlugInRegistry");
        builder.AppendLine("{");
        builder.AppendLine("    private static readonly global::MUnique.OpenMU.PlugIns.PlugInRegistration[] Registrations =");
        builder.AppendLine("    [");
        foreach (var plugIn in model.PlugIns)
        {
            builder.Append("        new(typeof(").Append(plugIn.TypeName).AppendLine("), static manager =>");
            builder.AppendLine("        {");
            foreach (var plugInInterface in plugIn.Interfaces)
            {
                builder.Append("            manager.RegisterPlugIn<").Append(plugInInterface).Append(", ").Append(plugIn.TypeName).AppendLine(">();");
            }

            builder.AppendLine("        }),");
        }

        builder.AppendLine("    ];");
        builder.AppendLine();
        builder.AppendLine("    /// <inheritdoc />");
        builder.AppendLine("    public global::System.Collections.Generic.IReadOnlyList<global::MUnique.OpenMU.PlugIns.PlugInRegistration> PlugIns => Registrations;");
        builder.AppendLine("}");
        return builder.ToString();
    }

    /// <summary>
    /// The model of the registry.
    /// </summary>
    /// <param name="Namespace">The namespace of the registry class.</param>
    /// <param name="PlugIns">The plugins, in the order of their definition.</param>
    private sealed record RegistryModel(string? Namespace, EquatableArray<PlugInModel> PlugIns);

    /// <summary>
    /// The model of a plugin.
    /// </summary>
    /// <param name="TypeName">The fully qualified name of the plugin type.</param>
    /// <param name="Interfaces">The fully qualified names of the plugin interfaces, which the plugin implements.</param>
    private sealed record PlugInModel(string TypeName, EquatableArray<string> Interfaces);
}

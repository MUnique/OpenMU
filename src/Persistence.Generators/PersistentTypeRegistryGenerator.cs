// <copyright file="PersistentTypeRegistryGenerator.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Generators;

using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

/// <summary>
/// Generates the registry of the persistent types of an assembly, which is marked with the
/// <c>GeneratePersistentTypeRegistryAttribute</c>. The persistent types are the classes of the namespace which is
/// specified by the attribute.
/// </summary>
/// <remarks>
/// Without a registry, the <c>TypeHelper</c> searches all types of the assembly to find the persistent type of a
/// base type, and creates instances and generic repositories by reflection. The registry lists the types in the
/// order of <c>Assembly.GetTypes()</c>, because the <c>TypeHelper</c> takes the first type with a matching base type.
/// </remarks>
[Generator]
public class PersistentTypeRegistryGenerator : IIncrementalGenerator
{
    private const string GenerateAttributeFullName = "MUnique.OpenMU.Persistence.GeneratePersistentTypeRegistryAttribute";

    private static readonly SymbolDisplayFormat TypeFormat = SymbolDisplayFormat.FullyQualifiedFormat;

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var registry = context.CompilationProvider.Select(static (compilation, cancellationToken) => CreateRegistryModel(compilation, cancellationToken));

        context.RegisterSourceOutput(registry, static (spc, model) =>
        {
            if (model is not null)
            {
                spc.AddSource("GeneratedPersistentTypeRegistry.g.cs", SourceText.From(GenerateSource(model), Encoding.UTF8));
            }
        });
    }

    private static RegistryModel? CreateRegistryModel(Compilation compilation, CancellationToken cancellationToken)
    {
        if (compilation.GetTypeByMetadataName(GenerateAttributeFullName) is not { } generateAttribute)
        {
            return null;
        }

        var namespaces = compilation.Assembly.GetAttributes()
            .Where(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, generateAttribute))
            .Select(a => a.ConstructorArguments.FirstOrDefault().Value as string)
            .Where(ns => ns is not null)
            .ToImmutableHashSet();
        if (namespaces.IsEmpty)
        {
            return null;
        }

        var types = ImmutableArray.CreateBuilder<TypeModel>();
        foreach (var type in GetTypesInDefinitionOrder(compilation.Assembly))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (type is not { TypeKind: TypeKind.Class, IsStatic: false, IsGenericType: false, ContainingType: null }
                || !namespaces.Contains(type.ContainingNamespace.ToDisplayString())
                || !compilation.IsSymbolAccessibleWithin(type, compilation.Assembly))
            {
                continue;
            }

            var hasFactory = !type.IsAbstract
                             && type.InstanceConstructors.Any(c => c.Parameters.IsEmpty && c.DeclaredAccessibility == Accessibility.Public);
            types.Add(new TypeModel(type.ToDisplayString(TypeFormat), hasFactory));
        }

        var assemblyName = compilation.AssemblyName;
        var ns = assemblyName is not null && assemblyName.Split('.').All(SyntaxFacts.IsValidIdentifier) ? assemblyName : null;
        return new RegistryModel(ns, new EquatableArray<TypeModel>(types.ToImmutable()));
    }

    /// <summary>
    /// Gets the types of the assembly in the order in which the compiler emits their definitions,
    /// which is the order of <c>Assembly.GetTypes()</c>: first the top level types, namespace by namespace,
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

    private static string GenerateSource(RegistryModel model)
    {
        var registryName = model.Namespace is null ? "global::GeneratedPersistentTypeRegistry" : "global::" + model.Namespace + ".GeneratedPersistentTypeRegistry";
        var builder = new StringBuilder();
        builder.AppendLine("// <auto-generated />");
        builder.AppendLine("#nullable enable");
        builder.AppendLine();
        builder.Append("[assembly: global::MUnique.OpenMU.Persistence.PersistentTypeRegistry(typeof(").Append(registryName).AppendLine("))]");
        builder.AppendLine();
        if (model.Namespace is not null)
        {
            builder.Append("namespace ").Append(model.Namespace).AppendLine(";");
            builder.AppendLine();
        }

        builder.AppendLine("/// <summary>The registry of the persistent types of this assembly.</summary>");
        builder.AppendLine("[global::System.CodeDom.Compiler.GeneratedCode(\"MUnique.OpenMU.Persistence.Generators\", \"1.0\")]");
        builder.AppendLine("internal sealed class GeneratedPersistentTypeRegistry : global::MUnique.OpenMU.Persistence.IPersistentTypeRegistry");
        builder.AppendLine("{");
        builder.AppendLine("    private static readonly global::MUnique.OpenMU.Persistence.PersistentType[] PersistentTypes =");
        builder.AppendLine("    [");
        foreach (var type in model.Types)
        {
            builder.Append("        new global::MUnique.OpenMU.Persistence.PersistentType<").Append(type.TypeName).Append(">(")
                .Append(type.HasFactory ? "static () => new " + type.TypeName + "()" : "null").AppendLine("),");
        }

        builder.AppendLine("    ];");
        builder.AppendLine();
        builder.AppendLine("    /// <inheritdoc />");
        builder.AppendLine("    public global::System.Collections.Generic.IReadOnlyList<global::MUnique.OpenMU.Persistence.PersistentType> Types => PersistentTypes;");
        builder.AppendLine("}");
        return builder.ToString();
    }

    /// <summary>
    /// The model of the registry.
    /// </summary>
    /// <param name="Namespace">The namespace of the registry class.</param>
    /// <param name="Types">The persistent types, in the order of their definition.</param>
    private sealed record RegistryModel(string? Namespace, EquatableArray<TypeModel> Types);

    /// <summary>
    /// The model of a persistent type.
    /// </summary>
    /// <param name="TypeName">The fully qualified name of the type.</param>
    /// <param name="HasFactory">If set to <c>true</c>, the type has a public parameterless constructor.</param>
    private sealed record TypeModel(string TypeName, bool HasFactory);
}

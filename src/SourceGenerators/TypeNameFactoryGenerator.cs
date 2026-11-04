// <copyright file="TypeNameFactoryGenerator.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.SourceGenerators;

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

/// <summary>
/// Implements the static partial methods which are marked with the <c>TypeNameFactoryAttribute</c>.
/// Such a method creates an object of a type which is specified by its full name, like <c>Type.GetType(typeName)</c>
/// and <c>Activator.CreateInstance</c> would do, but without reflection.
/// </summary>
/// <remarks>
/// For each class of the assembly which is assignable to the return type of the method, the generated method calls
/// the public constructor with the most parameters which can be supplied by the other parameters of the method,
/// matched by their types. Classes without such a constructor, and generic or inaccessible classes, are not created.
/// </remarks>
[Generator]
public class TypeNameFactoryGenerator : IIncrementalGenerator
{
    private const string TypeNameFactoryAttributeFullName = "MUnique.OpenMU.Annotations.TypeNameFactoryAttribute";

    private static readonly SymbolDisplayFormat TypeFormat = SymbolDisplayFormat.FullyQualifiedFormat
        .AddMiscellaneousOptions(SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var factories = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                TypeNameFactoryAttributeFullName,
                predicate: static (node, _) => node is MethodDeclarationSyntax,
                transform: static (ctx, _) => CreateModel((IMethodSymbol)ctx.TargetSymbol, ctx.SemanticModel.Compilation))
            .Where(static model => model is not null);

        context.RegisterSourceOutput(factories, static (spc, model) => spc.AddSource(model!.HintName, SourceText.From(GenerateSource(model), Encoding.UTF8)));
    }

    private static FactoryModel? CreateModel(IMethodSymbol method, Compilation compilation)
    {
        if (!method.IsStatic
            || !method.IsPartialDefinition
            || method.Parameters.Length == 0
            || method.Parameters[0].Type.SpecialType != SpecialType.System_String
            || method.ContainingType is not { } containingType
            || containingType.ContainingType is not null)
        {
            return null;
        }

        var argumentParameters = method.Parameters.Skip(1).ToList();
        var cases = ImmutableArray.CreateBuilder<CaseModel>();
        foreach (var type in GetAllTypes(compilation.Assembly.GlobalNamespace))
        {
            if (type.TypeKind != TypeKind.Class
                || type.IsAbstract
                || type.IsGenericType
                || GetContainingTypes(type).Any(t => t.IsGenericType)
                || !compilation.IsSymbolAccessibleWithin(type, compilation.Assembly)
                || !compilation.HasImplicitConversion(type, method.ReturnType))
            {
                continue;
            }

            var constructor = type.InstanceConstructors
                .Where(c => c.DeclaredAccessibility == Accessibility.Public)
                .Select(c => (Constructor: c, Arguments: GetArguments(c, argumentParameters)))
                .Where(c => c.Arguments is not null)
                .OrderByDescending(c => c.Arguments!.Count)
                .FirstOrDefault();
            if (constructor.Arguments is null)
            {
                continue;
            }

            cases.Add(new CaseModel(GetFullMetadataName(type), type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat), string.Join(", ", constructor.Arguments)));
        }

        var parameters = string.Join(", ", method.Parameters.Select(p => p.Type.ToDisplayString(TypeFormat) + " @" + p.Name));
        return new FactoryModel(
            containingType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).Replace("global::", string.Empty) + "." + method.Name + ".g.cs",
            containingType.ContainingNamespace.IsGlobalNamespace ? null : containingType.ContainingNamespace.ToDisplayString(),
            containingType.Name,
            containingType.IsStatic,
            SyntaxFacts(method.DeclaredAccessibility),
            method.ReturnType.ToDisplayString(TypeFormat),
            method.Name,
            parameters,
            "@" + method.Parameters[0].Name,
            new EquatableArray<CaseModel>(cases.OrderBy(c => c.TypeName, StringComparer.Ordinal).ToImmutableArray()));
    }

    /// <summary>
    /// Gets the arguments for the constructor, which are the names of the matching method parameters.
    /// </summary>
    /// <returns>The arguments; <c>null</c>, if a constructor parameter can't be supplied.</returns>
    private static List<string>? GetArguments(IMethodSymbol constructor, List<IParameterSymbol> methodParameters)
    {
        var arguments = new List<string>();
        foreach (var parameter in constructor.Parameters)
        {
            var match = methodParameters.FirstOrDefault(p => SymbolEqualityComparer.Default.Equals(
                p.Type.WithNullableAnnotation(NullableAnnotation.None),
                parameter.Type.WithNullableAnnotation(NullableAnnotation.None)));
            if (match is null)
            {
                return null;
            }

            // A null value is passed through, like Activator.CreateInstance would do.
            var suppressNull = match.NullableAnnotation == NullableAnnotation.Annotated
                               && parameter.NullableAnnotation != NullableAnnotation.Annotated
                               && !parameter.Type.IsValueType;
            arguments.Add("@" + match.Name + (suppressNull ? "!" : string.Empty));
        }

        return arguments;
    }

    private static IEnumerable<INamedTypeSymbol> GetAllTypes(INamespaceSymbol ns)
    {
        foreach (var member in ns.GetMembers())
        {
            if (member is INamespaceSymbol childNamespace)
            {
                foreach (var type in GetAllTypes(childNamespace))
                {
                    yield return type;
                }
            }
            else if (member is INamedTypeSymbol type)
            {
                foreach (var nestedType in GetTypeAndNestedTypes(type))
                {
                    yield return nestedType;
                }
            }
        }
    }

    private static IEnumerable<INamedTypeSymbol> GetTypeAndNestedTypes(INamedTypeSymbol type)
    {
        yield return type;
        foreach (var nested in type.GetTypeMembers().SelectMany(GetTypeAndNestedTypes))
        {
            yield return nested;
        }
    }

    private static IEnumerable<INamedTypeSymbol> GetContainingTypes(INamedTypeSymbol symbol)
    {
        for (var containingType = symbol.ContainingType; containingType is not null; containingType = containingType.ContainingType)
        {
            yield return containingType;
        }
    }

    /// <summary>
    /// Gets the full name of the type, like <c>Type.FullName</c>, e.g. <c>Namespace.Outer+Inner</c>.
    /// </summary>
    private static string GetFullMetadataName(INamedTypeSymbol type)
    {
        var name = type.MetadataName;
        for (var containingType = type.ContainingType; containingType is not null; containingType = containingType.ContainingType)
        {
            name = containingType.MetadataName + "+" + name;
        }

        return type.ContainingNamespace.IsGlobalNamespace ? name : type.ContainingNamespace.ToDisplayString() + "." + name;
    }

    private static string SyntaxFacts(Accessibility accessibility) => accessibility switch
    {
        Accessibility.Public => "public ",
        Accessibility.Internal => "internal ",
        Accessibility.Private => "private ",
        Accessibility.Protected => "protected ",
        Accessibility.ProtectedOrInternal => "protected internal ",
        Accessibility.ProtectedAndInternal => "private protected ",
        _ => string.Empty,
    };

    private static string GenerateSource(FactoryModel model)
    {
        var builder = new StringBuilder();
        builder.AppendLine("// <auto-generated />");
        builder.AppendLine("#nullable enable");
        builder.AppendLine();
        if (model.Namespace is not null)
        {
            builder.Append("namespace ").Append(model.Namespace).AppendLine(";");
            builder.AppendLine();
        }

        builder.Append(model.IsStaticClass ? "static " : string.Empty).Append("partial class ").AppendLine(model.ClassName);
        builder.AppendLine("{");
        builder.Append("    ").Append(model.Accessibility).Append("static partial ").Append(model.ReturnType).Append(' ').Append(model.MethodName)
            .Append('(').Append(model.Parameters).AppendLine(")");
        builder.AppendLine("    {");
        builder.Append("        return ").Append(model.TypeNameParameter).AppendLine(" switch");
        builder.AppendLine("        {");
        foreach (var factoryCase in model.Cases)
        {
            builder.Append("            \"").Append(factoryCase.TypeName).Append("\" => new ").Append(factoryCase.TypeDisplayName)
                .Append('(').Append(factoryCase.Arguments).AppendLine("),");
        }

        builder.AppendLine("            _ => null,");
        builder.AppendLine("        };");
        builder.AppendLine("    }");
        builder.AppendLine("}");
        return builder.ToString();
    }

    /// <summary>
    /// The model of a factory method.
    /// </summary>
    /// <param name="HintName">The hint name of the generated source.</param>
    /// <param name="Namespace">The namespace of the class which declares the method.</param>
    /// <param name="ClassName">The name of the class which declares the method.</param>
    /// <param name="IsStaticClass">If set to <c>true</c>, the class is static.</param>
    /// <param name="Accessibility">The accessibility modifier of the method, including a trailing space.</param>
    /// <param name="ReturnType">The return type of the method.</param>
    /// <param name="MethodName">The name of the method.</param>
    /// <param name="Parameters">The parameter list of the method.</param>
    /// <param name="TypeNameParameter">The name of the parameter which holds the type name.</param>
    /// <param name="Cases">The types which can be created.</param>
    private sealed record FactoryModel(
        string HintName,
        string? Namespace,
        string ClassName,
        bool IsStaticClass,
        string Accessibility,
        string ReturnType,
        string MethodName,
        string Parameters,
        string TypeNameParameter,
        EquatableArray<CaseModel> Cases);

    /// <summary>
    /// The model of a type which can be created by a factory method.
    /// </summary>
    /// <param name="TypeName">The full name of the type, like <c>Type.FullName</c>.</param>
    /// <param name="TypeDisplayName">The fully qualified name of the type in C#.</param>
    /// <param name="Arguments">The arguments of the constructor call.</param>
    private sealed record CaseModel(string TypeName, string TypeDisplayName, string Arguments);
}

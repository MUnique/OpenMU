// <copyright file="ChatCommandArgumentsGenerator.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.SourceGenerators;

using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

/// <summary>
/// Generates the registration of the writable properties of the arguments classes of the chat commands,
/// which are derived from <c>ArgumentsBase</c>. Without it, <c>ChatCommandArguments</c> determines them by reflection.
/// </summary>
/// <remarks>
/// The properties are listed in the order of <c>Type.GetProperties()</c>, because the arguments of a chat command
/// are assigned to the properties in this order, when they are passed without their short names.
/// Classes with properties which can't be accessed by the generated code are skipped.
/// </remarks>
[Generator]
public class ChatCommandArgumentsGenerator : IIncrementalGenerator
{
    private const string ArgumentsBaseFullName = "MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.ArgumentsBase";

    private const string ArgumentAttributeFullName = "MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.ArgumentAttribute";

    private const string ValidValuesAttributeFullName = "MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.ValidValuesAttribute";

    private const string RangeAttributeFullName = "System.ComponentModel.DataAnnotations.RangeAttribute";

    private const string ValueReferenceAttributeFullName = "MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.ValueReferenceAttribute";

    private static readonly SymbolDisplayFormat TypeFormat = SymbolDisplayFormat.FullyQualifiedFormat;

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var registrations = context.CompilationProvider.Select(static (compilation, cancellationToken) => CreateModel(compilation, cancellationToken));

        context.RegisterSourceOutput(registrations, static (spc, model) =>
        {
            if (model is not null)
            {
                spc.AddSource("GeneratedChatCommandArguments.g.cs", SourceText.From(GenerateSource(model), Encoding.UTF8));
            }
        });
    }

    private static RegistrationModel? CreateModel(Compilation compilation, CancellationToken cancellationToken)
    {
        if (compilation.GetTypeByMetadataName(ArgumentsBaseFullName) is not { } argumentsBase
            || compilation.GetTypeByMetadataName(ArgumentAttributeFullName) is not { } argumentAttribute
            || compilation.GetTypeByMetadataName(ValidValuesAttributeFullName) is not { } validValuesAttribute
            || compilation.GetTypeByMetadataName(RangeAttributeFullName) is not { } rangeAttribute
            || compilation.GetTypeByMetadataName(ValueReferenceAttributeFullName) is not { } valueReferenceAttribute)
        {
            return null;
        }

        var classes = ImmutableArray.CreateBuilder<ClassModel>();
        foreach (var type in GetAllTypes(compilation.Assembly.GlobalNamespace))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (type.TypeKind != TypeKind.Class
                || type.IsGenericType
                || !compilation.IsSymbolAccessibleWithin(type, compilation.Assembly)
                || !InheritsFrom(type, argumentsBase))
            {
                continue;
            }

            var properties = ImmutableArray.CreateBuilder<PropertyModel>();
            var isSupported = true;
            foreach (var property in GetPublicProperties(type))
            {
                if (property.SetMethod is not { } setMethod)
                {
                    continue;
                }

                if (property.IsStatic
                    || !compilation.IsSymbolAccessibleWithin(setMethod, compilation.Assembly)
                    || !compilation.IsSymbolAccessibleWithin(property.Type, compilation.Assembly))
                {
                    isSupported = false;
                    break;
                }

                properties.Add(new PropertyModel(
                    property.Name,
                    property.Type.ToDisplayString(TypeFormat),
                    GetAttributeCreation(property, argumentAttribute),
                    GetAttributeCreation(property, validValuesAttribute),
                    GetAttributeCreation(property, rangeAttribute),
                    GetAttributeCreation(property, valueReferenceAttribute)));
            }

            if (isSupported)
            {
                classes.Add(new ClassModel(type.ToDisplayString(TypeFormat), new EquatableArray<PropertyModel>(properties.ToImmutable())));
            }
        }

        if (classes.Count == 0)
        {
            return null;
        }

        var assemblyName = compilation.AssemblyName;
        var ns = assemblyName is not null && assemblyName.Split('.').All(SyntaxFacts.IsValidIdentifier) ? assemblyName : null;
        return new RegistrationModel(ns, new EquatableArray<ClassModel>(classes.OrderBy(c => c.TypeName, StringComparer.Ordinal).ToImmutableArray()));
    }

    private static bool InheritsFrom(INamedTypeSymbol type, INamedTypeSymbol baseType)
    {
        for (var current = type; current is not null; current = current.BaseType)
        {
            if (SymbolEqualityComparer.Default.Equals(current, baseType))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Gets the public properties of the type, including the inherited ones, in the order of <c>Type.GetProperties()</c>:
    /// the properties which are declared by the type itself first, then the ones of its base types.
    /// An overriding property hides the overridden property, even if it doesn't override all of its accessors.
    /// </summary>
    private static IEnumerable<IPropertySymbol> GetPublicProperties(INamedTypeSymbol type)
    {
        var names = new HashSet<string>();
        for (var current = type; current is not null; current = current.BaseType)
        {
            foreach (var property in current.GetMembers().OfType<IPropertySymbol>())
            {
                if (property.IsIndexer || !names.Add(property.Name))
                {
                    continue;
                }

                if (property.GetMethod?.DeclaredAccessibility == Accessibility.Public
                    || property.SetMethod?.DeclaredAccessibility == Accessibility.Public)
                {
                    yield return property;
                }
            }
        }
    }

    /// <summary>
    /// Gets the C# expression which creates the attribute of the property, or of the properties which it overrides,
    /// like <c>Attribute.GetCustomAttribute</c> would return it.
    /// </summary>
    private static string? GetAttributeCreation(IPropertySymbol property, INamedTypeSymbol attributeType)
    {
        for (var current = property; current is not null; current = current.OverriddenProperty)
        {
            if (current.GetAttributes().FirstOrDefault(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeType)) is { } attribute)
            {
                var arguments = string.Join(", ", attribute.ConstructorArguments.Select(ToCSharp));
                var namedArguments = attribute.NamedArguments.IsEmpty
                    ? string.Empty
                    : " { " + string.Join(", ", attribute.NamedArguments.Select(a => a.Key + " = " + ToCSharp(a.Value))) + " }";
                return "new " + attributeType.ToDisplayString(TypeFormat) + "(" + arguments + ")" + namedArguments;
            }
        }

        return null;
    }

    private static string ToCSharp(TypedConstant constant)
    {
        if (constant.Kind == TypedConstantKind.Array)
        {
            return "new " + constant.Type!.ToDisplayString(TypeFormat) + " { " + string.Join(", ", constant.Values.Select(ToCSharp)) + " }";
        }

        return constant.ToCSharpString();
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

    private static string GenerateSource(RegistrationModel model)
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

        builder.AppendLine("/// <summary>Registers the properties of the chat command arguments classes of this assembly.</summary>");
        builder.AppendLine("[global::System.CodeDom.Compiler.GeneratedCode(\"MUnique.OpenMU.SourceGenerators\", \"1.0\")]");
        builder.AppendLine("internal static class GeneratedChatCommandArguments");
        builder.AppendLine("{");
        builder.AppendLine("    /// <summary>Registers the properties.</summary>");
        builder.AppendLine("    [global::System.Runtime.CompilerServices.ModuleInitializer]");
        builder.AppendLine("    internal static void Register()");
        builder.AppendLine("    {");
        foreach (var argumentsClass in model.Classes)
        {
            builder.Append("        global::MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.ChatCommandArguments.Register(typeof(").Append(argumentsClass.TypeName).AppendLine("),");
            builder.AppendLine("        [");
            foreach (var property in argumentsClass.Properties)
            {
                builder.Append("            new(\"").Append(property.Name).Append("\", typeof(").Append(property.TypeName).Append("), ")
                    .Append(property.Argument ?? "null").Append(", ")
                    .Append(property.ValidValues ?? "null").Append(", ")
                    .Append(property.Range ?? "null").Append(", ")
                    .Append(property.ValueReference ?? "null").Append(", ")
                    .Append("static (instance, value) => ((").Append(argumentsClass.TypeName).Append(")instance).").Append(property.Name)
                    .Append(" = (").Append(property.TypeName).AppendLine(")value!),");
            }

            builder.AppendLine("        ]);");
        }

        builder.AppendLine("    }");
        builder.AppendLine("}");
        return builder.ToString();
    }

    /// <summary>
    /// The model of the registration.
    /// </summary>
    /// <param name="Namespace">The namespace of the generated class.</param>
    /// <param name="Classes">The arguments classes.</param>
    private sealed record RegistrationModel(string? Namespace, EquatableArray<ClassModel> Classes);

    /// <summary>
    /// The model of an arguments class.
    /// </summary>
    /// <param name="TypeName">The fully qualified name of the class.</param>
    /// <param name="Properties">The writable properties.</param>
    private sealed record ClassModel(string TypeName, EquatableArray<PropertyModel> Properties);

    /// <summary>
    /// The model of a writable property.
    /// </summary>
    /// <param name="Name">The name of the property.</param>
    /// <param name="TypeName">The fully qualified name of the type of the property.</param>
    /// <param name="Argument">The expression which creates the argument attribute.</param>
    /// <param name="ValidValues">The expression which creates the valid values attribute.</param>
    /// <param name="Range">The expression which creates the range attribute.</param>
    /// <param name="ValueReference">The expression which creates the value reference attribute.</param>
    private sealed record PropertyModel(string Name, string TypeName, string? Argument, string? ValidValues, string? Range, string? ValueReference);
}

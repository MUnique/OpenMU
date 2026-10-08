// <copyright file="ReferenceResolvingConverterGenerator.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Generators;

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

/// <summary>
/// Generates the json converters of the persistent data classes, which implement <c>IIdentifiable</c>.
/// A converter reads the json objects which are created by the json queries of the persistence, and resolves
/// their (circular) references.
/// </summary>
/// <remarks>
/// Without a generated converter, the <c>ReferenceResolvingConverterFactory</c> creates a <c>ReferenceResolvingConverter</c>
/// which determines the properties by reflection, and compiles expressions to access them.
/// This generator determines the same properties at compile time, and generates the same accesses as plain code.
/// Each converter registers itself at the <c>ReferenceResolvingConverterRegistry</c> with a module initializer.
/// Types which are not supported by the <c>ReferenceResolvingConverter</c> are skipped, so that the behavior
/// stays the same for them.
/// </remarks>
[Generator]
public class ReferenceResolvingConverterGenerator : IIncrementalGenerator
{
    private const string IdentifiableFullName = "MUnique.OpenMU.Persistence.IIdentifiable";

    private const string JsonIgnoreAttributeFullName = "System.Text.Json.Serialization.JsonIgnoreAttribute";

    private const string JsonPropertyNameAttributeFullName = "System.Text.Json.Serialization.JsonPropertyNameAttribute";

    private const string ConverterBaseName = "global::MUnique.OpenMU.Persistence.Json.ReferenceResolvingConverterBase";

    private const string RawPrefix = "Raw";

    private const string JoinedPrefix = "Joined";

    private static readonly SymbolDisplayFormat TypeFormat = SymbolDisplayFormat.FullyQualifiedFormat;

    /// <summary>
    /// The kind of a property handler. It's the same as in the <c>ReferenceResolvingConverter</c>.
    /// </summary>
    private enum PropertyKind
    {
        /// <summary>
        /// The value is assigned to the property.
        /// </summary>
        Setter,

        /// <summary>
        /// The items are added to the collection of a "Raw" property, if they're not already contained.
        /// </summary>
        RawAdder,

        /// <summary>
        /// The items are added to the collection which is joined by a "Joined" property, if they're not already contained.
        /// </summary>
        JoinedAdder,

        /// <summary>
        /// The items are added to the collection of a property without setter.
        /// </summary>
        CollectionAdder,
    }

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var converters = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => node is ClassDeclarationSyntax,
                transform: static (ctx, cancellationToken) => ctx.SemanticModel.GetDeclaredSymbol(ctx.Node, cancellationToken) is INamedTypeSymbol type
                    ? CreateConverterModel(type, ctx.SemanticModel.Compilation)
                    : null)
            .Where(static model => model is not null)
            .Collect()
            .Select(static (models, _) => new EquatableArray<ConverterModel>(models
                .Select(model => model!)
                .GroupBy(model => model.TypeName)
                .Select(group => group.First())
                .OrderBy(model => model.TypeName, StringComparer.Ordinal)
                .ToImmutableArray()));

        var assemblyName = context.CompilationProvider.Select(static (compilation, _) => compilation.AssemblyName);

        context.RegisterSourceOutput(converters.Combine(assemblyName), static (spc, input) =>
        {
            var (models, assemblyName) = input;
            if (!models.Any())
            {
                return;
            }

            foreach (var model in models)
            {
                spc.AddSource(model.HintName, SourceText.From(GenerateConverter(model), Encoding.UTF8));
            }

            spc.AddSource("GeneratedReferenceResolvingConverters.g.cs", SourceText.From(GenerateRegistration(models, assemblyName), Encoding.UTF8));
        });
    }

    private static ConverterModel? CreateConverterModel(INamedTypeSymbol type, Compilation compilation)
    {
        if (compilation.GetTypeByMetadataName(IdentifiableFullName) is not { } identifiable
            || compilation.GetTypeByMetadataName(JsonIgnoreAttributeFullName) is not { } jsonIgnore
            || compilation.GetTypeByMetadataName(JsonPropertyNameAttributeFullName) is not { } jsonPropertyName)
        {
            return null;
        }

        if (!IsConvertible(type, identifiable, compilation))
        {
            return null;
        }

        var collectionType = compilation.GetSpecialType(SpecialType.System_Collections_Generic_ICollection_T);
        var properties = GetPublicProperties(type)
            .Where(p => !HasAttribute(p, jsonIgnore))
            .ToList();
        var propertyNames = new HashSet<string>(properties.Select(p => p.Name));

        var propertyModels = ImmutableArray.CreateBuilder<PropertyModel>();
        var valueTypes = new List<ITypeSymbol> { type };
        foreach (var property in properties)
        {
            ITypeSymbol valueType;
            var collectionInterface = DetermineCollectionInterface(property.Type, collectionType);
            var jsonName = GetJsonPropertyName(property, jsonPropertyName) ?? property.Name;
            PropertyModel? model;
            if (collectionInterface is not null && property.Name.StartsWith(RawPrefix, StringComparison.Ordinal))
            {
                valueType = collectionInterface.TypeArguments[0];
                model = CreateAdderModel(PropertyKind.RawAdder, jsonName, valueType, property, collectionInterface, compilation);
            }
            else if (collectionInterface is not null && property.Name.StartsWith(JoinedPrefix, StringComparison.Ordinal))
            {
                var basePropertyName = property.Name.Substring(JoinedPrefix.Length);
                var baseCollectionProperty = properties.FirstOrDefault(p => p.Name == basePropertyName);
                if (baseCollectionProperty?.Type is not INamedTypeSymbol { IsGenericType: true } baseCollectionPropertyType
                    || DetermineCollectionInterface(baseCollectionPropertyType, collectionType) is not { } baseCollectionInterface)
                {
                    // The ReferenceResolvingConverter fails for this type.
                    return null;
                }

                var baseType = baseCollectionPropertyType.TypeArguments[0];
                var joinNavigation = GetPublicProperties(collectionInterface.TypeArguments[0])
                    .FirstOrDefault(p => SymbolEqualityComparer.Default.Equals(p.Type.BaseType, baseType));
                if (joinNavigation is null)
                {
                    // The ReferenceResolvingConverter fails for this type.
                    return null;
                }

                valueType = joinNavigation.Type;
                model = CreateAdderModel(PropertyKind.JoinedAdder, basePropertyName, valueType, baseCollectionProperty, baseCollectionInterface, compilation);
            }
            else if (property.SetMethod is { } setMethod && !propertyNames.Contains(JoinedPrefix + property.Name))
            {
                valueType = property.Type;
                model = CreateSetterModel(jsonName, property, setMethod, compilation);
            }
            else if (collectionInterface is not null
                     && !propertyNames.Contains(RawPrefix + property.Name)
                     && !propertyNames.Contains(JoinedPrefix + property.Name))
            {
                // A collection without setter and without a "Raw" or "Joined" counterpart, which would hold its data.
                valueType = collectionInterface.TypeArguments[0];
                model = CreateAdderModel(PropertyKind.CollectionAdder, jsonName, valueType, property, collectionInterface, compilation);
            }
            else
            {
                // not supported property, ignore...
                continue;
            }

            if (model is null)
            {
                // We can't access a member, so we let the ReferenceResolvingConverter handle the type.
                return null;
            }

            propertyModels.Add(model);
            valueTypes.Add(valueType);
        }

        var containingTypes = string.Concat(GetContainingTypes(type).Select(t => t.Name + "_"));
        var ns = type.ContainingNamespace.IsGlobalNamespace ? null : type.ContainingNamespace.ToDisplayString();
        var typeName = type.ToDisplayString(TypeFormat);
        return new ConverterModel(
            typeName.Replace("global::", string.Empty) + ".ReferenceResolvingConverter.g.cs",
            ns,
            containingTypes + type.Name + "ReferenceResolvingConverter",
            typeName,
            new EquatableArray<PropertyModel>(propertyModels.ToImmutable()),
            new EquatableArray<ValueTypeModel>(CreateValueTypeModels(valueTypes, compilation)));
    }

    /// <summary>
    /// Creates the models of the types which are read by the converter, including the converted type itself.
    /// The underlying types of nullable value types are included, too.
    /// </summary>
    private static ImmutableArray<ValueTypeModel> CreateValueTypeModels(IEnumerable<ITypeSymbol> types, Compilation compilation)
    {
        var result = new Dictionary<string, ValueTypeModel>();
        foreach (var type in types)
        {
            var current = type;
            while (current is not null)
            {
                var typeName = current.ToDisplayString(TypeFormat);
                ITypeSymbol? underlyingType = null;
                if (result.ContainsKey(typeName) || !IsAccessible(current, compilation))
                {
                    break;
                }

                string? builtInConverter;
                if (current is INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T } nullable)
                {
                    underlyingType = nullable.TypeArguments[0];
                    builtInConverter = "global::System.Text.Json.Serialization.Metadata.JsonMetadataServices.GetNullableConverter<" + underlyingType.ToDisplayString(TypeFormat) + ">(options)";
                }
                else if (current.TypeKind == TypeKind.Enum)
                {
                    builtInConverter = "global::System.Text.Json.Serialization.Metadata.JsonMetadataServices.GetEnumConverter<" + typeName + ">(options)";
                }
                else
                {
                    builtInConverter = GetBuiltInConverterName(current) is { } converterName
                        ? "global::System.Text.Json.Serialization.Metadata.JsonMetadataServices." + converterName
                        : null;
                }

                result.Add(typeName, new ValueTypeModel(typeName, builtInConverter));
                current = underlyingType;
            }
        }

        return result.Values.OrderBy(t => t.TypeName, StringComparer.Ordinal).ToImmutableArray();
    }

    /// <summary>
    /// Gets the name of the property of the <c>JsonMetadataServices</c>, which provides the built-in converter of the type.
    /// </summary>
    private static string? GetBuiltInConverterName(ITypeSymbol type)
    {
        if (type is IArrayTypeSymbol { Rank: 1, ElementType.SpecialType: SpecialType.System_Byte })
        {
            return "ByteArrayConverter";
        }

        switch (type.SpecialType)
        {
            case SpecialType.System_Boolean: return "BooleanConverter";
            case SpecialType.System_Byte: return "ByteConverter";
            case SpecialType.System_SByte: return "SByteConverter";
            case SpecialType.System_Char: return "CharConverter";
            case SpecialType.System_Int16: return "Int16Converter";
            case SpecialType.System_UInt16: return "UInt16Converter";
            case SpecialType.System_Int32: return "Int32Converter";
            case SpecialType.System_UInt32: return "UInt32Converter";
            case SpecialType.System_Int64: return "Int64Converter";
            case SpecialType.System_UInt64: return "UInt64Converter";
            case SpecialType.System_Single: return "SingleConverter";
            case SpecialType.System_Double: return "DoubleConverter";
            case SpecialType.System_Decimal: return "DecimalConverter";
            case SpecialType.System_String: return "StringConverter";
            case SpecialType.System_DateTime: return "DateTimeConverter";
        }

        return type.ToDisplayString(TypeFormat) switch
        {
            "global::System.Guid" => "GuidConverter",
            "global::System.DateTimeOffset" => "DateTimeOffsetConverter",
            "global::System.TimeSpan" => "TimeSpanConverter",
            "global::System.DateOnly" => "DateOnlyConverter",
            "global::System.TimeOnly" => "TimeOnlyConverter",
            "global::System.Uri" => "UriConverter",
            "global::System.Version" => "VersionConverter",
            _ => null,
        };
    }

    /// <summary>
    /// Determines if the type is convertible, with the same conditions as the <c>ReferenceResolvingConverterFactory</c> and
    /// the type constraints of the <c>ReferenceResolvingConverter</c>.
    /// </summary>
    private static bool IsConvertible(INamedTypeSymbol type, INamedTypeSymbol identifiable, Compilation compilation)
    {
        return type is { TypeKind: TypeKind.Class, IsAbstract: false, IsStatic: false, IsGenericType: false }
               && GetContainingTypes(type).All(t => !t.IsGenericType)
               && type.ContainingNamespace.ToDisplayString().StartsWith("MUnique.OpenMU.", StringComparison.Ordinal)
               && type.AllInterfaces.Contains(identifiable, SymbolEqualityComparer.Default)
               && type.InstanceConstructors.Any(c => c.Parameters.IsEmpty && c.DeclaredAccessibility == Accessibility.Public)
               && compilation.IsSymbolAccessibleWithin(type, compilation.Assembly);
    }

    /// <summary>
    /// Gets the public instance properties of the type, including the inherited ones, like <c>Type.GetProperties()</c>.
    /// </summary>
    /// <remarks>
    /// An overriding property hides the overridden property, even if it doesn't override all of its accessors.
    /// Indexers are not included, because the <c>ReferenceResolvingConverter</c> can't handle them anyway.
    /// </remarks>
    private static IEnumerable<IPropertySymbol> GetPublicProperties(ITypeSymbol type)
    {
        var names = new HashSet<string>();
        for (var current = type; current is not null; current = current.BaseType)
        {
            foreach (var property in current.GetMembers().OfType<IPropertySymbol>())
            {
                if (property.IsStatic || property.IsIndexer || !names.Add(property.Name))
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

    private static IEnumerable<INamedTypeSymbol> GetContainingTypes(INamedTypeSymbol symbol)
    {
        var containingTypes = new List<INamedTypeSymbol>();
        for (var containingType = symbol.ContainingType; containingType is not null; containingType = containingType.ContainingType)
        {
            containingTypes.Insert(0, containingType);
        }

        return containingTypes;
    }

    /// <summary>
    /// Gets the attribute of the property, or of the properties which it overrides, like <c>Attribute.GetCustomAttribute</c>.
    /// </summary>
    private static AttributeData? GetAttribute(IPropertySymbol property, INamedTypeSymbol attributeType)
    {
        for (var current = property; current is not null; current = current.OverriddenProperty)
        {
            if (current.GetAttributes().FirstOrDefault(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeType)) is { } attribute)
            {
                return attribute;
            }
        }

        return null;
    }

    private static bool HasAttribute(IPropertySymbol property, INamedTypeSymbol attributeType)
    {
        return GetAttribute(property, attributeType) is not null;
    }

    private static string? GetJsonPropertyName(IPropertySymbol property, INamedTypeSymbol jsonPropertyName)
    {
        return GetAttribute(property, jsonPropertyName) is { ConstructorArguments: { Length: 1 } arguments }
            ? arguments[0].Value as string
            : null;
    }

    private static INamedTypeSymbol? DetermineCollectionInterface(ITypeSymbol type, INamedTypeSymbol collectionType)
    {
        if (type is not INamedTypeSymbol { IsGenericType: true } namedType)
        {
            return null;
        }

        return SymbolEqualityComparer.Default.Equals(namedType.OriginalDefinition, collectionType)
            ? namedType
            : namedType.AllInterfaces.FirstOrDefault(i => SymbolEqualityComparer.Default.Equals(i.OriginalDefinition, collectionType));
    }

    private static PropertyModel? CreateSetterModel(string jsonName, IPropertySymbol property, IMethodSymbol setMethod, Compilation compilation)
    {
        if (!IsAccessible(property.Type, compilation))
        {
            return null;
        }

        AccessorModel? accessor = null;
        if (!compilation.IsSymbolAccessibleWithin(setMethod, compilation.Assembly))
        {
            if (!IsAccessible(property.ContainingType, compilation))
            {
                return null;
            }

            accessor = new AccessorModel(setMethod.Name, property.ContainingType.ToDisplayString(TypeFormat));
        }

        return new PropertyModel(
            PropertyKind.Setter,
            jsonName,
            property.Type.ToDisplayString(TypeFormat),
            IsNonNullableValueType(property.Type),
            EscapeIdentifier(property.Name),
            property.Type.ToDisplayString(TypeFormat),
            accessor);
    }

    private static PropertyModel? CreateAdderModel(PropertyKind kind, string jsonName, ITypeSymbol itemType, IPropertySymbol collectionProperty, INamedTypeSymbol collectionInterface, Compilation compilation)
    {
        if (!IsAccessible(itemType, compilation)
            || !IsAccessible(collectionInterface, compilation)
            || !IsAccessible(collectionProperty.Type, compilation)
            || collectionProperty.GetMethod is not { } getMethod)
        {
            return null;
        }

        AccessorModel? accessor = null;
        if (!compilation.IsSymbolAccessibleWithin(getMethod, compilation.Assembly))
        {
            if (!IsAccessible(collectionProperty.ContainingType, compilation))
            {
                return null;
            }

            accessor = new AccessorModel(getMethod.Name, collectionProperty.ContainingType.ToDisplayString(TypeFormat));
        }

        return new PropertyModel(
            kind,
            jsonName,
            itemType.ToDisplayString(TypeFormat),
            IsNonNullableValueType(itemType),
            EscapeIdentifier(collectionProperty.Name),
            collectionInterface.ToDisplayString(TypeFormat),
            accessor)
        {
            CollectionPropertyType = collectionProperty.Type.ToDisplayString(TypeFormat),
        };
    }

    private static bool IsAccessible(ITypeSymbol type, Compilation compilation)
    {
        return compilation.IsSymbolAccessibleWithin(type, compilation.Assembly);
    }

    private static bool IsNonNullableValueType(ITypeSymbol type)
    {
        return type.IsValueType && type.OriginalDefinition.SpecialType != SpecialType.System_Nullable_T;
    }

    private static string EscapeIdentifier(string name)
    {
        return SyntaxFacts.GetKeywordKind(name) != SyntaxKind.None ? "@" + name : name;
    }

    private static string GenerateConverter(ConverterModel model)
    {
        var properties = model.Properties.ToList();
        var builder = new StringBuilder();
        builder.AppendLine("// <auto-generated />");
        builder.AppendLine("#nullable disable");
        builder.AppendLine();
        if (model.Namespace is not null)
        {
            builder.Append("namespace ").Append(model.Namespace).AppendLine(";");
            builder.AppendLine();
        }

        builder.Append("/// <summary>Reads and resolves the references of <see cref=\"").Append(model.TypeName).AppendLine("\"/>.</summary>");
        builder.AppendLine("[global::System.CodeDom.Compiler.GeneratedCode(\"MUnique.OpenMU.Persistence.Generators\", \"1.0\")]");
        builder.Append("internal sealed class ").Append(model.ConverterName).Append(" : ").Append(ConverterBaseName).Append('<').Append(model.TypeName).AppendLine(">");
        builder.AppendLine("{");
        builder.AppendLine("    private static readonly global::MUnique.OpenMU.Persistence.Json.ReferenceResolvingProperty[] PropertyDescriptions =");
        builder.AppendLine("    [");
        foreach (var property in properties)
        {
            builder.Append("        new(").Append(SymbolDisplay.FormatLiteral(property.JsonName, true)).Append(", typeof(").Append(property.ValueType).Append("), ")
                .Append(property.Kind == PropertyKind.Setter ? "false" : "true").AppendLine("),");
        }

        builder.AppendLine("    ];");
        builder.AppendLine();
        builder.AppendLine("    /// <summary>Initializes a new instance of the converter.</summary>");
        builder.AppendLine("    /// <param name=\"ignoredTypes\">The ignored types.</param>");
        builder.Append("    public ").Append(model.ConverterName).AppendLine("(global::System.Type[] ignoredTypes)");
        builder.AppendLine("        : base(PropertyDescriptions, ignoredTypes)");
        builder.AppendLine("    {");
        builder.AppendLine("    }");

        AppendReadMethod(builder, model, properties, "ReadPropertyValue", setters: true);
        AppendReadMethod(builder, model, properties, "ReadCollectionItem", setters: false);

        for (var index = 0; index < properties.Count; index++)
        {
            var property = properties[index];
            if (property.Accessor is not { } accessor)
            {
                continue;
            }

            builder.AppendLine();
            builder.Append("    [global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.Method, Name = ")
                .Append(SymbolDisplay.FormatLiteral(accessor.MethodName, true)).AppendLine(")]");
            if (property.Kind == PropertyKind.Setter)
            {
                builder.Append("    private static extern void ").Append(GetAccessorName(index)).Append('(').Append(accessor.DeclaringType).Append(" target, ")
                    .Append(property.MemberType).AppendLine(" value);");
            }
            else
            {
                builder.Append("    private static extern ").Append(property.CollectionPropertyType).Append(' ').Append(GetAccessorName(index)).Append('(')
                    .Append(accessor.DeclaringType).AppendLine(" target);");
            }
        }

        builder.AppendLine("}");
        return builder.ToString();
    }

    private static void AppendReadMethod(StringBuilder builder, ConverterModel model, List<PropertyModel> properties, string methodName, bool setters)
    {
        builder.AppendLine();
        builder.AppendLine("    /// <inheritdoc />");
        builder.Append("    protected override void ").Append(methodName).Append("(int index, ref global::System.Text.Json.Utf8JsonReader reader, ")
            .Append(model.TypeName).AppendLine(" target, global::System.Text.Json.JsonSerializerOptions options)");
        builder.AppendLine("    {");
        builder.AppendLine("        switch (index)");
        builder.AppendLine("        {");
        for (var index = 0; index < properties.Count; index++)
        {
            var property = properties[index];
            if ((property.Kind == PropertyKind.Setter) != setters)
            {
                continue;
            }

            builder.Append("            case ").Append(index).AppendLine(":");
            builder.AppendLine("            {");
            builder.Append("                var value = ReadValue<").Append(property.ValueType).AppendLine(">(ref reader, options);");
            var indent = "                ";
            if (!property.IsNonNullableValueType)
            {
                builder.AppendLine("                if (value is not null)");
                builder.AppendLine("                {");
                indent = "                    ";
            }

            if (property.Kind == PropertyKind.Setter)
            {
                builder.Append(indent);
                if (property.Accessor is null)
                {
                    builder.Append("target.").Append(property.MemberName).AppendLine(" = value;");
                }
                else
                {
                    builder.Append(GetAccessorName(index)).AppendLine("(target, value);");
                }
            }
            else
            {
                builder.Append(indent).Append(property.MemberType).Append(" collection = ")
                    .Append(property.Accessor is null ? "target." + property.MemberName : GetAccessorName(index) + "(target)").AppendLine(";");
                if (property.Kind == PropertyKind.CollectionAdder)
                {
                    builder.Append(indent).AppendLine("collection.Add(value);");
                }
                else
                {
                    builder.Append(indent).AppendLine("if (!collection.Contains(value))");
                    builder.Append(indent).AppendLine("{");
                    builder.Append(indent).AppendLine("    collection.Add(value);");
                    builder.Append(indent).AppendLine("}");
                }
            }

            if (!property.IsNonNullableValueType)
            {
                builder.AppendLine("                }");
            }

            builder.AppendLine();
            builder.AppendLine("                break;");
            builder.AppendLine("            }");
        }

        builder.AppendLine("            default:");
        builder.AppendLine("                throw new global::System.ArgumentOutOfRangeException(nameof(index), index, null);");
        builder.AppendLine("        }");
        builder.AppendLine("    }");
    }

    private static string GetAccessorName(int index) => "Access" + index;

    private static string GenerateRegistration(EquatableArray<ConverterModel> models, string? assemblyName)
    {
        var builder = new StringBuilder();
        builder.AppendLine("// <auto-generated />");
        builder.AppendLine("#nullable disable");
        builder.AppendLine();
        if (assemblyName is not null && assemblyName.Split('.').All(SyntaxFacts.IsValidIdentifier))
        {
            builder.Append("namespace ").Append(assemblyName).AppendLine(";");
            builder.AppendLine();
        }

        builder.AppendLine("/// <summary>Registers the generated reference resolving json converters of this assembly.</summary>");
        builder.AppendLine("[global::System.CodeDom.Compiler.GeneratedCode(\"MUnique.OpenMU.Persistence.Generators\", \"1.0\")]");
        builder.AppendLine("internal static class GeneratedReferenceResolvingConverters");
        builder.AppendLine("{");
        builder.AppendLine("    /// <summary>Registers the converters.</summary>");
        builder.AppendLine("    [global::System.Runtime.CompilerServices.ModuleInitializer]");
        builder.AppendLine("    internal static void Register()");
        builder.AppendLine("    {");
        foreach (var valueType in models.SelectMany(m => m.ValueTypes).GroupBy(t => t.TypeName).Select(g => g.First()).OrderBy(t => t.TypeName, StringComparer.Ordinal))
        {
            builder.Append("        global::MUnique.OpenMU.Persistence.Json.ReferenceResolvingTypeInfoResolver.Register<").Append(valueType.TypeName).Append(">(")
                .Append(valueType.BuiltInConverter is null ? "null" : "static options => " + valueType.BuiltInConverter).AppendLine(");");
        }

        foreach (var model in models)
        {
            var converterName = model.Namespace is null ? "global::" + model.ConverterName : "global::" + model.Namespace + "." + model.ConverterName;
            builder.Append("        global::MUnique.OpenMU.Persistence.Json.ReferenceResolvingConverterRegistry.Register<").Append(model.TypeName)
                .Append(">(static ignoredTypes => new ").Append(converterName).AppendLine("(ignoredTypes));");
        }

        builder.AppendLine("    }");
        builder.AppendLine("}");
        return builder.ToString();
    }

    /// <summary>
    /// The model of a converter.
    /// </summary>
    /// <param name="HintName">The hint name of the generated source.</param>
    /// <param name="Namespace">The namespace of the converted type.</param>
    /// <param name="ConverterName">The name of the converter class.</param>
    /// <param name="TypeName">The fully qualified name of the converted type.</param>
    /// <param name="Properties">The properties which are read.</param>
    /// <param name="ValueTypes">The types which are read by the converter, including the converted type.</param>
    private sealed record ConverterModel(string HintName, string? Namespace, string ConverterName, string TypeName, EquatableArray<PropertyModel> Properties, EquatableArray<ValueTypeModel> ValueTypes);

    /// <summary>
    /// The model of a type which is read by a converter.
    /// </summary>
    /// <param name="TypeName">The fully qualified name of the type.</param>
    /// <param name="BuiltInConverter">The expression which gets the built-in converter of the type for the <c>options</c>, if there is one.</param>
    private sealed record ValueTypeModel(string TypeName, string? BuiltInConverter);

    /// <summary>
    /// The model of a property handler.
    /// </summary>
    /// <param name="Kind">The kind of the handler.</param>
    /// <param name="JsonName">The name of the json property.</param>
    /// <param name="ValueType">The fully qualified type of the value, or of the items, which are read.</param>
    /// <param name="IsNonNullableValueType">If set to <c>true</c>, the <paramref name="ValueType"/> is a non-nullable value type.</param>
    /// <param name="MemberName">The name of the property which is set, or which holds the collection.</param>
    /// <param name="MemberType">The fully qualified type of the property which is set, or the type of the collection interface.</param>
    /// <param name="Accessor">The accessor, if the property accessor is not accessible from the generated code.</param>
    private sealed record PropertyModel(PropertyKind Kind, string JsonName, string ValueType, bool IsNonNullableValueType, string MemberName, string MemberType, AccessorModel? Accessor)
    {
        /// <summary>
        /// Gets the fully qualified type of the property which holds the collection.
        /// </summary>
        public string? CollectionPropertyType { get; init; }
    }

    /// <summary>
    /// The model of an unsafe accessor for a property accessor which is not accessible from the generated code.
    /// </summary>
    /// <param name="MethodName">The name of the accessor method, e.g. <c>set_Value</c>.</param>
    /// <param name="DeclaringType">The fully qualified type which declares the accessor method.</param>
    private sealed record AccessorModel(string MethodName, string DeclaringType);
}

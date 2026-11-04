// <copyright file="EfCoreModelGenerator.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.SourceGenerator;

using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using MUnique.OpenMU.Annotations;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Composition;

/// <summary>
/// Source Generator which creates classes of the our entities specifically for the entity framework core.
/// </summary>
[Generator]
public class EfCoreModelGenerator : IIncrementalGenerator, IUnboundSourceGenerator
{
    /// <summary>
    /// Holds the Assembly-Name which is the target of this generator.
    /// </summary>
    internal const string TargetAssemblyName = "MUnique.OpenMU.Persistence.EntityFramework";

    private const string GameConfigurationFullName = "MUnique.OpenMU.DataModel.Configuration.GameConfiguration";

    private static readonly Type[] IgnoredTypes = { typeof(SimpleElement) };

    /// <summary>
    /// The standalone types which should not contain additional foreign key, because they were used somewhere in collections (except at GameConfiguration).
    /// For these types, join entity classes will be created and ManyToManyCollectionAdapter{T,TJoin} are used adapt between these types and the join entities.
    /// </summary>
    private static readonly (string TypeName, bool StandaloneForEntityOnly)[] StandaloneTypes =
    {
        ("MUnique.OpenMU.DataModel.Configuration.CharacterClass", false),
        ("MUnique.OpenMU.DataModel.Configuration.DropItemGroup", false),
        ("MUnique.OpenMU.DataModel.Configuration.Items.IncreasableItemOption", false),
        ("MUnique.OpenMU.DataModel.Configuration.Items.ItemDefinition", false),
        ("MUnique.OpenMU.DataModel.Configuration.Items.ItemOption", false),
        ("MUnique.OpenMU.DataModel.Configuration.Items.ItemOptionType", false),
        ("MUnique.OpenMU.DataModel.Configuration.Items.ItemOptionDefinition", false),
        ("MUnique.OpenMU.DataModel.Configuration.Items.ItemSetGroup", false),
        ("MUnique.OpenMU.DataModel.Configuration.Items.ItemOfItemSet", true),
        ("MUnique.OpenMU.DataModel.Configuration.MasterSkillDefinition", false),
        ("MUnique.OpenMU.DataModel.Configuration.Skill", false),
        ("MUnique.OpenMU.DataModel.Configuration.GameMapDefinition", false),
    };

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var assemblyNameProvider = context.CompilationProvider.Select((compilation, _) => compilation.AssemblyName);

        context.RegisterSourceOutput(assemblyNameProvider, (sourceProductionContext, assemblyName) =>
        {
            if (assemblyName != TargetAssemblyName)
            {
                return;
            }

            try
            {
                foreach (var (name, source) in this.GenerateSources())
                {
                    sourceProductionContext.AddSource(name, SourceText.From(source, Encoding.UTF8));
                }
            }
            catch (Exception e)
            {
                sourceProductionContext.ReportDiagnostic(
                    Diagnostic.Create(
                        new DiagnosticDescriptor(
                            "EFCOREGEN001",
                            "Source generation failed",
                            $"{e.GetType()}: {e.Message}",
                            "SourceGeneration",
                            DiagnosticSeverity.Error,
                            true),
                        Location.None));
            }
        });
    }

    /// <summary>
    /// Generates the source files.
    /// </summary>
    /// <returns>The created source files.</returns>
    public IEnumerable<(string Name, string Source)> GenerateSources()
    {
        foreach (var type in ModelGeneratorHelper.CustomTypes)
        {
            var className = type.Name;
            var fullName = type.FullName;
            var standaloneCollectionProperties = this.GetStandaloneCollectionProperties(type).ToList();
            var isCloneable = type.GetCustomAttribute<CloneableAttribute>(true) is not null;

            var classSource = $@"{string.Format(ModelGeneratorHelper.FileHeaderTemplate, className)}

namespace MUnique.OpenMU.Persistence.EntityFramework.Model;

using System.ComponentModel.DataAnnotations.Schema;
using MUnique.OpenMU.Persistence;

/// <summary>
/// The Entity Framework Core implementation of <see cref=""{type.FullName}""/>.
/// </summary>
[Table(nameof({type.Name}), Schema = {(ModelGeneratorHelper.IsConfigurationType(type) ? "SchemaNames.Configuration" : "SchemaNames.AccountData")})]
internal partial class {className} : {fullName}, IIdentifiable
{{
    {this.CreateConstructors(type, standaloneCollectionProperties.Any())}
    {this.CreateIdPropertyIfRequired(type)}
    {this.CreateNavigationProperties(type)}
{(isCloneable ? ModelGeneratorHelper.OverrideClonable(type, className) : null)}
    /// <inheritdoc/>
    public override bool Equals(object obj)
    {{
        var baseObject = obj as IIdentifiable;
        if (baseObject != null)
        {{
            return baseObject.Id == this.Id;
        }}

        return base.Equals(obj);
    }}

    /// <inheritdoc/>
    public override int GetHashCode()
    {{
        return this.Id.GetHashCode();
    }}

    {this.CreateInitJoinCollections(type, standaloneCollectionProperties)}
}}
";
            yield return (className, classSource);
        }

        yield return ("ExtendedTypeContext", this.GenerateDbContext());
        yield return ("BasicModelConverter", GenerateBasicModelConverter());
        foreach (var (name, source) in this.GenerateJoinEntities())
        {
            yield return (name, source);
        }
    }

    /// <summary>
    /// Gets the constructor which should be used to create the converted object, if there is one.
    /// It's only usable when all of its parameters can be taken from a property of the same name.
    /// </summary>
    /// <param name="type">The type of the data model.</param>
    /// <returns>The constructor which should be used; otherwise, <c>null</c>.</returns>
    private static ConstructorInfo GetConstructorToMapWith(Type type)
    {
        return type.GetConstructors()
            .Where(c => c.IsPublic && c.GetParameters().Length > 0)
            .FirstOrDefault(c => c.GetParameters().All(parameter =>
                type.GetProperty(parameter.Name.ToPascalCase()) is { CanRead: true } property
                && parameter.ParameterType.IsAssignableFrom(property.PropertyType)));
    }

    private static bool IsMemberOfAggregate(PropertyInfo propertyInfo)
    {
        if (propertyInfo?.Name.StartsWith("Raw") ?? false)
        {
            propertyInfo = propertyInfo.DeclaringType?.GetProperty(propertyInfo.Name.Substring(3), BindingFlags.Instance | BindingFlags.Public);
        }

        return propertyInfo?.GetCustomAttribute<MemberOfAggregateAttribute>() is { };
    }

    private static bool IsStandaloneType(string typeName, Type referencingType)
    {
        return StandaloneTypes.Any(st =>
        {
            if (st.TypeName != typeName)
            {
                return false;
            }

            return !st.StandaloneForEntityOnly || !ModelGeneratorHelper.IsConfigurationType(referencingType);
        });
    }

    private IEnumerable<(string Name, string Source)> GenerateJoinEntities()
    {
        var standaloneCollectionProperties = ModelGeneratorHelper.CustomTypes.SelectMany(this.GetStandaloneCollectionProperties).ToList();
        foreach (PropertyInfo propertyInfo in standaloneCollectionProperties)
        {
            var elementType = propertyInfo.PropertyType.GenericTypeArguments[0];
            var joinTypeName = propertyInfo.ReflectedType!.Name + elementType.Name;

            var source = $@"{string.Format(ModelGeneratorHelper.FileHeaderTemplate, joinTypeName)}

namespace MUnique.OpenMU.Persistence.EntityFramework.Model;

using System.ComponentModel.DataAnnotations.Schema;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.EntityFramework;

[Table(nameof({joinTypeName}), Schema = {(ModelGeneratorHelper.IsConfigurationType(propertyInfo.ReflectedType) ? "SchemaNames.Configuration" : "SchemaNames.AccountData")})]
internal partial class {joinTypeName}
{{
    public Guid {propertyInfo.ReflectedType.Name}Id {{ get; set; }}
    public {propertyInfo.ReflectedType.Name} {propertyInfo.ReflectedType.Name} {{ get; set; }}

    public Guid {elementType.Name}Id {{ get; set; }}
    public {elementType.Name} {elementType.Name} {{ get; set; }}
}}

internal partial class {propertyInfo.ReflectedType.Name}
{{
    public ICollection<{joinTypeName}> Joined{propertyInfo.Name} {{ get; }} = new EntityFramework.List<{joinTypeName}>();
}}
";
            yield return (joinTypeName, source);
        }
    }

    /// <summary>
    /// Generates the converter, which converts the objects of this model to the objects of the basic model.
    /// </summary>
    /// <remarks>
    /// It copies the whole object graph:
    /// The references are preserved, and the properties whose names start with "Raw" or which are marked
    /// with the <see cref="TransientAttribute"/> are ignored.
    /// </remarks>
    /// <returns>The generated source.</returns>
    private static string GenerateBasicModelConverter()
    {
        var customTypes = ModelGeneratorHelper.CustomTypes.ToList();
        var dispatch = new StringBuilder();
        foreach (var type in customTypes.OrderByDescending(GetInheritanceDepth).ThenBy(t => t.Name, StringComparer.Ordinal))
        {
            dispatch.AppendLine($"            {type.Name} value => this.Convert(value),");
        }

        var methods = new StringBuilder();
        foreach (var type in customTypes.OrderBy(t => t.Name, StringComparer.Ordinal))
        {
            methods.AppendLine();
            methods.AppendLine($"    private BasicModel.{type.Name} Convert({type.Name} source)");
            methods.AppendLine("    {");
            methods.AppendLine("        if (this._converted.TryGetValue(source, out var converted))");
            methods.AppendLine("        {");
            methods.AppendLine($"            return (BasicModel.{type.Name})converted;");
            methods.AppendLine("        }");
            methods.AppendLine();
            if (GetConstructorToMapWith(type) is { } constructor)
            {
                // Properties which can only be set through a constructor (e.g. ConstValueAttribute.Value)
                var arguments = string.Join(", ", constructor.GetParameters().Select(p => $"source.{p.Name.ToPascalCase()}"));
                methods.AppendLine($"        var target = new BasicModel.{type.Name}({arguments});");
            }
            else
            {
                methods.AppendLine($"        var target = new BasicModel.{type.Name}();");
            }

            methods.AppendLine("        this._converted.Add(source, target);");
            if (type.GetProperty("Id") is null)
            {
                methods.AppendLine("        target.Id = source.Id;");
            }

            foreach (var property in GetConvertedProperties(type))
            {
                methods.AppendLine(GetPropertyConversion(property, customTypes));
            }

            methods.AppendLine("        return target;");
            methods.AppendLine("    }");
        }

        return $@"{string.Format(ModelGeneratorHelper.FileHeaderTemplate, "BasicModelConverter")}

#nullable enable

namespace MUnique.OpenMU.Persistence.EntityFramework.Model;

using System.Runtime.CompilerServices;

/// <summary>
/// Converts the objects of this persistence model to the objects of the <see cref=""MUnique.OpenMU.Persistence.BasicModel""/>.
/// The references between the objects are preserved.
/// </summary>
internal sealed class BasicModelConverter
{{
    private readonly Dictionary<object, object> _converted = new(ReferenceEqualityComparer.Instance);

    /// <summary>
    /// Converts the object and the objects which it references to the basic model.
    /// </summary>
    /// <typeparam name=""TBasic"">The type of the basic model.</typeparam>
    /// <param name=""source"">The source object.</param>
    /// <returns>The converted object.</returns>
    public static TBasic Convert<TBasic>(object source)
        where TBasic : class
    {{
        return (TBasic)new BasicModelConverter().ConvertObject(source)!;
    }}

    private object? ConvertObject(object? source)
    {{
        return source switch
        {{
            null => null,
{dispatch}            _ => source,
        }};
    }}
{methods}{GetSetterAccessors(customTypes)}}}
";
    }

    private static int GetInheritanceDepth(Type type)
    {
        var depth = 0;
        for (var current = type.BaseType; current is not null; current = current.BaseType)
        {
            depth++;
        }

        return depth;
    }

    private static IEnumerable<PropertyInfo> GetConvertedProperties(Type type)
    {
        return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.GetIndexParameters().Length == 0)
            .Where(p => !p.Name.StartsWith("Raw", StringComparison.Ordinal))
            .Where(p => !p.GetCustomAttributes(true).OfType<TransientAttribute>().Any())
            .Where(p => p.CanRead);
    }

    private static string GetPropertyConversion(PropertyInfo property, List<Type> customTypes)
    {
        var propertyType = property.PropertyType;
        var isCollection = propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(ICollection<>);
        if (isCollection)
        {
            var elementType = propertyType.GetGenericArguments()[0];
            var item = elementType.IsValueType || elementType == typeof(string)
                ? "item"
                : $"({elementType.FullName!.Replace('+', '.')})this.ConvertObject(item)!";
            return $@"        if (source.{property.Name} is {{ }} {property.Name.ToCamelCase()}Items)
        {{
            foreach (var item in {property.Name.ToCamelCase()}Items)
            {{
                target.{property.Name}.Add({item});
            }}
        }}
";
        }

        if (property.SetMethod is not { } setMethod)
        {
            return $"        // {property.Name} has no setter.";
        }

        var value = propertyType.IsValueType || propertyType == typeof(string) || propertyType.IsArray
            ? $"source.{property.Name}"
            : $"({GetTypeName(propertyType)})this.ConvertObject(source.{property.Name})!";
        if (setMethod.IsPublic)
        {
            return $"        target.{property.Name} = {value};";
        }

        // Properties with non-public setters are set, too.
        return $"        {GetSetterAccessorName(property)}(target, {value});";
    }

    private static string GetSetterAccessorName(PropertyInfo property) => $"Set{property.SetMethod!.DeclaringType!.Name}{property.Name}";

    private static string GetSetterAccessors(IEnumerable<Type> types)
    {
        var accessors = new StringBuilder();
        var properties = types.SelectMany(GetConvertedProperties)
            .Where(p => p.SetMethod is { IsPublic: false } && !(p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>)))
            .GroupBy(GetSetterAccessorName)
            .Select(g => g.First())
            .OrderBy(GetSetterAccessorName, StringComparer.Ordinal);
        foreach (var property in properties)
        {
            accessors.AppendLine();
            accessors.AppendLine($"    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = \"{property.SetMethod!.Name}\")]");
            accessors.AppendLine($"    private static extern void {GetSetterAccessorName(property)}({GetTypeName(property.SetMethod.DeclaringType!)} target, {GetTypeName(property.PropertyType)} value);");
        }

        return accessors.ToString();
    }

    private static string GetTypeName(Type type)
    {
        return type.IsGenericType
            ? $"{type.Namespace}.{type.Name.Substring(0, type.Name.IndexOf('`'))}<{string.Join(", ", type.GetGenericArguments().Select(GetTypeName))}>"
            : type.FullName!.Replace('+', '.');
    }

    private string GenerateDbContext()
    {
        var ignores = new StringBuilder();
        foreach (var type in ModelGeneratorHelper.CustomTypes)
        {
            ignores.AppendLine($"        modelBuilder.Ignore<{type.FullName}>();");
        }

        var joinDefinitions = new StringBuilder();
        var allStandaloneCollectionProperties = ModelGeneratorHelper.CustomTypes
            .Where(t => t.FullName != GameConfigurationFullName)
            .SelectMany(t => t.GetProperties().Where(p =>
                p.PropertyType.IsGenericType &&
                p.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>) &&
                !IsMemberOfAggregate(p) &&
                IsStandaloneType(p.PropertyType.GenericTypeArguments[0].FullName, t))).ToList();

        foreach (PropertyInfo propertyInfo in allStandaloneCollectionProperties)
        {
            var elementType = propertyInfo.PropertyType.GenericTypeArguments[0];
            var joinTypeName = propertyInfo.ReflectedType!.Name + elementType.Name;
            joinDefinitions
                .AppendLine($"        modelBuilder.Entity<{propertyInfo.ReflectedType.Name}>().HasMany(entity => entity.Joined{propertyInfo.Name}).WithOne(join => join.{propertyInfo.ReflectedType.Name});")
                .AppendLine($"        modelBuilder.Entity<{joinTypeName}>().HasKey(join => new {{ join.{propertyInfo.ReflectedType.Name}Id, join.{elementType.Name}Id }});");
        }

        var deleteCascades = new StringBuilder();
        deleteCascades.AppendLine("        // All members which are marked with the MemberOfAggregateAttribute, should be defined with ON DELETE CASCADE.");
        foreach (var type in ModelGeneratorHelper.CustomTypes)
        {
            foreach (var propertyInfo in type.GetProperties()
                         .Where(p => p.GetCustomAttribute<MemberOfAggregateAttribute>() is { })
                         .Where(p => !IgnoredTypes.Contains(p.PropertyType)))
            {
                var propertyType = propertyInfo.PropertyType;
                var isCollection = propertyType.IsGenericType;
                if (isCollection)
                {
                    deleteCascades.AppendLine($"        modelBuilder.Entity<{type.Name}>().HasMany(entity => entity.Raw{propertyInfo.Name}).WithOne().OnDelete(DeleteBehavior.Cascade);");
                }
                else
                {
                    deleteCascades.AppendLine($"        modelBuilder.Entity<{type.Name}>().HasOne(entity => entity.Raw{propertyInfo.Name}).WithOne().OnDelete(DeleteBehavior.Cascade);");
                }
            }
        }

        var source = $@"{string.Format(ModelGeneratorHelper.FileHeaderTemplate, "ExtendedTypeContext")}

namespace MUnique.OpenMU.Persistence.EntityFramework.Model;

using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using MUnique.OpenMU.Persistence;

/// <summary>
/// DbContext which sets all extended base types to ignore.
/// </summary>
public class ExtendedTypeContext : Microsoft.EntityFrameworkCore.DbContext
{{
    /// <inheritdoc/>
    protected override void OnModelCreating(Microsoft.EntityFrameworkCore.ModelBuilder modelBuilder)
    {{
{ignores}
{deleteCascades}
    }}

    /// <summary>
    /// Adds the generated join definitions.
    /// </summary>
    /// <param name=""modelBuilder"">The model builder.</param>
    protected void AddJoinDefinitions(Microsoft.EntityFrameworkCore.ModelBuilder modelBuilder)
    {{
{joinDefinitions}
    }}
}}
";
        return source;
    }

    private string CreateInitJoinCollections(Type type, ICollection<PropertyInfo> standaloneCollectionProperties)
    {
        if (!standaloneCollectionProperties.Any())
        {
            return null;
        }

        var result = new StringBuilder().AppendLine(@"protected void InitJoinCollections()
    {");

        foreach (PropertyInfo propertyInfo in standaloneCollectionProperties)
        {
            var elementType = propertyInfo.PropertyType.GenericTypeArguments[0];
            var joinTypeName = propertyInfo.ReflectedType!.Name + elementType.Name;
            result.AppendLine($@"        this.{propertyInfo.Name} = new ManyToManyCollectionAdapter<{elementType.FullName}, {joinTypeName}>(this.Joined{propertyInfo.Name}, joinEntity => joinEntity.{elementType.Name}, entity => new {joinTypeName} {{ {type.Name} = this, {type.Name}Id = this.Id, {elementType.Name} = ({elementType.Name})entity, {elementType.Name}Id = (({elementType.Name})entity).Id}});");
        }

        result.Append("    }");

        return result.ToString();
    }

    private string CreateNavigationProperties(Type type)
    {
        var result = new StringBuilder();
        var virtualNavigationProperties = type
            .GetProperties()
            .Where(p => p.GetGetMethod() is { IsVirtual: true, IsFinal: false }
                        && !p.PropertyType.IsValueType
                        && !p.PropertyType.IsArray)
            .Where(p => type.FullName == GameConfigurationFullName ||
                        !(p.PropertyType.IsGenericType
                          && p.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>)
                          && !IsMemberOfAggregate(p)
                          && IsStandaloneType(p.PropertyType.GenericTypeArguments[0].FullName, type)))
            .ToList();

        var collectionProperties = virtualNavigationProperties
            .Where(p => p.PropertyType.IsGenericType
                        && (p.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>)
                            || p.PropertyType.GetGenericTypeDefinition() == typeof(IList<>)))
            .ToList();
        var primitiveCollectionProperties = collectionProperties.Where(p => p.PropertyType.GenericTypeArguments[0].IsPrimitive);
        var nonPrimitiveCollectionProperties = collectionProperties.Where(p => !p.PropertyType.GenericTypeArguments[0].IsPrimitive);

        foreach (var property in nonPrimitiveCollectionProperties)
        {
            result.AppendLine(this.BuildCollectionCode(property));
        }

        foreach (var property in primitiveCollectionProperties)
        {
            result.AppendLine(this.BuildPrimitiveCollectionCode(property));
        }

        var navigationProperties = virtualNavigationProperties.Where(p => !p.PropertyType.IsGenericType);
        foreach (var property in navigationProperties)
        {
            result.AppendLine(this.BuildNavigationCode(property));
        }

        return result.ToString();
    }

    private string BuildNavigationCode(PropertyInfo property)
    {
        var propertyTypeName = property.PropertyType.Name.Split('.').Last();
        var propertyType = property.PropertyType;

        return $@"
    /// <summary>
    /// Gets or sets the identifier of <see cref=""{property.Name}""/>.
    /// </summary>
    public Guid? {property.Name}Id {{ get; set; }}

    /// <summary>
    /// Gets the raw object of <see cref=""{property.Name}"" />.
    /// </summary>
    [ForeignKey(nameof({property.Name}Id))]
    public {propertyTypeName} Raw{property.Name}
    {{
        get => base.{property.Name} as {propertyTypeName};
        {(property.GetSetMethod(true) is { } ? $"set => base.{property.Name} = value;" : null)}
    }}

    /// <inheritdoc/>
    [NotMapped]
    public override {propertyType.FullName} {property.Name}
    {{
        get => base.{property.Name};{(property.GetSetMethod(true) is { } ? $@"{(property.GetSetMethod() is null ? "protected " : null)}set
        {{
            base.{property.Name} = value;
            this.{property.Name}Id = this.Raw{property.Name}?.Id;
        }}" : null)}
    }}";
    }

    private string BuildCollectionCode(PropertyInfo property)
    {
        var propertyType = property.PropertyType;
        var persistentClassName = propertyType.GetGenericArguments()[0].Name;
        var originalClassName = propertyType.GetGenericArguments()[0].FullName;

        var originalPropertyTypeName = propertyType.Name.Split('`')[0] + "<" + originalClassName + ">";
        var propertyTypeName = propertyType.Name.Split('`')[0] + "<" + persistentClassName + ">";

        var adapterClass = propertyType.GetGenericTypeDefinition() == typeof(IList<>) ? "ListAdapter" : "CollectionAdapter";

        return $@"
    /// <summary>
    /// Gets the raw collection of <see cref=""{property.Name}"" />.
    /// </summary>
    public {propertyTypeName} Raw{property.Name} {{ get; }} = new EntityFramework.List<{persistentClassName}>();
    
    /// <inheritdoc/>
    [NotMapped]
    public override {originalPropertyTypeName} {property.Name} => base.{property.Name} ??= new {adapterClass}<{originalClassName}, {persistentClassName}>(this.Raw{property.Name});";
    }

    private string BuildPrimitiveCollectionCode(PropertyInfo property)
    {
        var propertyType = property.PropertyType;
        var itemTypeName = propertyType.GetGenericArguments()[0].FullName;

        var originalPropertyTypeName = propertyType.Name.Split('`')[0] + "<" + itemTypeName + ">";

        return $@"
    /// <summary>
    /// Gets the raw string of <see cref=""{property.Name}"" />.
    /// </summary>
    [Column(nameof({property.Name}))]
    [System.Text.Json.Serialization.JsonPropertyName(""{property.Name.ToCamelCase()}"")]
    public string Raw{property.Name} {{ get; set; }}
    
    /// <inheritdoc/>
    [System.Text.Json.Serialization.JsonIgnore]
    [NotMapped]
    public override {originalPropertyTypeName} {property.Name}
    {{
        get => base.{property.Name} ??= new CollectionToStringAdapter<{itemTypeName}>(this.Raw{property.Name}, newString => this.Raw{property.Name} = newString);
        protected set
        {{
            this.{property.Name}.Clear();
            foreach (var item in value)
            {{
                this.{property.Name}.Add(item);
            }}
        }}
    }}";
    }

    private string CreateIdPropertyIfRequired(Type type)
    {
        if (type.GetProperty("Id") is null)
        {
            return @"
    /// <summary>
    /// Gets or sets the identifier of this instance.
    /// </summary>
    public Guid Id { get; set; }";
        }

        return string.Empty;
    }

    private IEnumerable<PropertyInfo> GetStandaloneCollectionProperties(Type type)
    {
        return type.FullName != GameConfigurationFullName ?
            type.GetProperties().Where(p => p.PropertyType.IsGenericType
                                            && p.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>)
                                            && !IsMemberOfAggregate(p)
                                            && IsStandaloneType(p.PropertyType.GenericTypeArguments[0].FullName, type)).ToList() :
            Enumerable.Empty<PropertyInfo>();
    }

    private string CreateConstructors(Type type, bool requiresJoinCollections)
    {
        var stringBuilder = new StringBuilder();
        var className = type.Name;
        if (requiresJoinCollections
            || (type.GetConstructors().Any(c => c.IsPublic && c.GetParameters().Length > 0)
                && type.GetConstructors().Any(c => c.GetParameters().Length == 0)))
        {
            stringBuilder.AppendLine(@$"/// <inheritdoc />
    public {className}()
    {{
{(requiresJoinCollections ? "        this.InitJoinCollections();" : null)}
    }}");
        }

        foreach (var constructor in type.GetConstructors()
                     .Where(c => c.IsPublic && c.GetParameters().Length > 0))
        {
            var parameters = constructor.GetParameters();
            stringBuilder.Append(@$"
    /// <inheritdoc />
    public {className}({ModelGeneratorHelper.GetParameterDefinitions(parameters)})
        : base({ModelGeneratorHelper.GetParameters(parameters)})
    {{
{(requiresJoinCollections ? "        this.InitJoinCollections();" : null)}
    }}
");
        }

        return stringBuilder.ToString();
    }
}
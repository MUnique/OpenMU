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
public class EfCoreModelGenerator : ModelGeneratorBase, IUnboundSourceGenerator
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

    /// <summary>
    /// Generates the source files.
    /// </summary>
    /// <returns>The created source files.</returns>
    public IEnumerable<(string Name, string Source)> GenerateSources()
    {
        foreach (var type in this.CustomTypes)
        {
            var className = type.Name;
            var fullName = type.FullName;
            var standaloneCollectionProperties = this.GetStandaloneCollectionProperties(type).ToList();
            var isCloneable = type.GetCustomAttribute<CloneableAttribute>(true) is not null;

            var classSource = $@"{string.Format(FileHeaderTemplate, className)}

namespace MUnique.OpenMU.Persistence.EntityFramework.Model;

using System.ComponentModel.DataAnnotations.Schema;
using MUnique.OpenMU.Persistence;

/// <summary>
/// The Entity Framework Core implementation of <see cref=""{type.FullName}""/>.
/// </summary>
[Table(nameof({type.Name}), Schema = {(IsConfigurationType(type) ? "SchemaNames.Configuration" : "SchemaNames.AccountData")})]
internal partial class {className} : {fullName}, IIdentifiable
{{
    {this.CreateConstructors(type, standaloneCollectionProperties.Any())}
    {this.CreateIdPropertyIfRequired(type)}
    {this.CreateNavigationProperties(type)}
{(isCloneable ? this.OverrideClonable(type, className) : null)}
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
        yield return ("MapsterConfigurator", this.GenerateMapsterConfigurator());
        foreach (var (name, source) in this.GenerateJoinEntities())
        {
            yield return (name, source);
        }
    }

    /// <inheritdoc />
    protected override void InnerExecute(in GeneratorExecutionContext context)
    {
        if (context.Compilation.AssemblyName != TargetAssemblyName)
        {
            return;
        }

        foreach (var (name, source) in this.GenerateSources())
        {
            context.AddSource(name, SourceText.From(source, Encoding.UTF8));
        }
    }

    private IEnumerable<(string Name, string Source)> GenerateJoinEntities()
    {
        var standaloneCollectionProperties = this.CustomTypes.SelectMany(this.GetStandaloneCollectionProperties).ToList();
        foreach (PropertyInfo propertyInfo in standaloneCollectionProperties)
        {
            var elementType = propertyInfo.PropertyType.GenericTypeArguments[0];
            var joinTypeName = propertyInfo.ReflectedType!.Name + elementType.Name;

            var source = $@"{string.Format(FileHeaderTemplate, joinTypeName)}

namespace MUnique.OpenMU.Persistence.EntityFramework.Model;

using System.ComponentModel.DataAnnotations.Schema;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.EntityFramework;

[Table(nameof({joinTypeName}), Schema = {(IsConfigurationType(propertyInfo.ReflectedType) ? "SchemaNames.Configuration" : "SchemaNames.AccountData")})]
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

    private string GenerateMapsterConfigurator()
    {
        var configs = new StringBuilder();
        foreach (var type in this.CustomTypes)
        {
            configs
                .AppendLine($"        Mapster.TypeAdapterConfig.GlobalSettings.NewConfig<{type.FullName}, {type.FullName}>()")
                .AppendLine($"            .Include<{type.Name}, BasicModel.{type.Name}>();")
                .AppendLine();
        }

        var source = $@"{string.Format(FileHeaderTemplate, "MapsterConfigurator")}

namespace MUnique.OpenMU.Persistence.EntityFramework.Model;

using MUnique.OpenMU.Persistence;
using Mapster;

/// <summary>
/// Configures Mapster to properly map these classes to the Persistence.BasicModel.
/// </summary>
public static class MapsterConfigurator
{{
    private static bool isConfigured;

    /// <summary>
    /// Ensures that Mapster is configured to properly map these EF-Core persistence classes to the Persistence.BasicModel.
    /// </summary>
    public static void EnsureConfigured()
    {{
        if (isConfigured)
        {{
            return;
        }}

        Mapster.TypeAdapterConfig.GlobalSettings.Default.PreserveReference(true);
        Mapster.TypeAdapterConfig.GlobalSettings.Default.IgnoreMember((member, side) => member.Name.StartsWith(""Raw""));

{configs}
        isConfigured = true;
    }}
}}
";
        return source;
    }

    private static bool IsMemberOfAggregate(PropertyInfo propertyInfo)
    {
        if (propertyInfo?.Name.StartsWith("Raw") ?? false)
        {
            propertyInfo = propertyInfo.DeclaringType?.GetProperty(propertyInfo.Name.Substring(3), BindingFlags.Instance | BindingFlags.Public);
        }

        return propertyInfo?.GetCustomAttribute<MemberOfAggregateAttribute>() is { };
    }

    private string GenerateDbContext()
    {
        var ignores = new StringBuilder();
        foreach (var type in this.CustomTypes)
        {
            ignores.AppendLine($"        modelBuilder.Ignore<{type.FullName}>();");
        }

        var joinDefinitions = new StringBuilder();
        var allStandaloneCollectionProperties = this.CustomTypes
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
        foreach (var type in this.CustomTypes)
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

        var source = $@"{string.Format(FileHeaderTemplate, "ExtendedTypeContext")}

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
        get => base.{property.Name};{
            (property.GetSetMethod(true) is { } ? $@"{(property.GetSetMethod() is null ? "protected " : null)}set
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

    private static bool IsStandaloneType(string typeName, Type referencingType)
    {
        return StandaloneTypes.Any(st =>
        {

            if (st.TypeName != typeName)
            {
                return false;
            }

            return !st.StandaloneForEntityOnly || !IsConfigurationType(referencingType);
        });
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
    public {className}({GetParameterDefinitions(parameters)})
        : base({GetParameters(parameters)})
    {{
{(requiresJoinCollections ? "        this.InitJoinCollections();" : null)}
    }}
");
        }

        return stringBuilder.ToString();
    }
}
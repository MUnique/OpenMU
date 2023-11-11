// <copyright file="BasicModelGenerator.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using MUnique.OpenMU.Annotations;
using MUnique.OpenMU.DataModel;

namespace MUnique.OpenMU.Persistence.SourceGenerator;

using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

/// <summary>
/// A generator for the plain and simple objects for the persistence project.
/// </summary>
[Generator]
public class BasicModelGenerator : ModelGeneratorBase, IUnboundSourceGenerator
{
    /// <summary>
    /// Holds the Assembly-Name which is the target of this generator.
    /// </summary>
    internal const string TargetAssemblyName = "MUnique.OpenMU.Persistence";

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
            var isCloneable = type.GetCustomAttribute<CloneableAttribute>(true) is not null;

            var classSource = $@"{string.Format(FileHeaderTemplate, className)}

namespace MUnique.OpenMU.Persistence.BasicModel;

using MUnique.OpenMU.Persistence.Json;

/// <summary>
/// A plain implementation of <see cref=""{className}""/>.
/// </summary>
public partial class {className} : {fullName}, IIdentifiable, IConvertibleTo<{className}>
{{
    {this.CreateConstructors(type)}
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

    /// <inheritdoc/>
    public {className} Convert() => this;
}}
";
            yield return (className, classSource);
        }
    }

    /// <inheritdoc />
    protected override void InnerExecute(in GeneratorExecutionContext context)
    {
        if (!(context.Compilation.AssemblyName?.EndsWith("Persistence") ?? false))
        {
            return;
        }

        foreach (var (name, source) in this.GenerateSources())
        {
            context.AddSource(name, SourceText.From(source, Encoding.UTF8));
        }
    }

    /// <summary>
    /// Builds the wrapper code for the navigation properties.
    /// </summary>
    /// <param name="type">The type whose properties should be handled.</param>
    /// <returns>The generated code of the properties.</returns>
    private string CreateNavigationProperties(Type type)
    {
        var result = new StringBuilder();
        var virtualNavigationProperties = type.GetProperties()
            .Where(p => p.GetGetMethod() is { IsVirtual: true, IsFinal: false }
                        && !p.PropertyType.IsValueType
                        && !p.PropertyType.IsArray).ToList();

        var collectionProperties = virtualNavigationProperties
            .Where(p => p.PropertyType.IsGenericType
                        && (p.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>) || p.PropertyType.GetGenericTypeDefinition() == typeof(IList<>))
                        && !p.PropertyType.GenericTypeArguments[0].IsPrimitive);

        foreach (var property in collectionProperties)
        {
            result.AppendLine(this.BuildCollectionCode(property));
        }

        var navigationProperties = virtualNavigationProperties.Where(p => !p.PropertyType.IsGenericType);
        foreach (var property in navigationProperties)
        {
            result.AppendLine(this.BuildNavigationCode(property));
        }

        return result.ToString();
    }

    /// <summary>
    /// Builds the wrapper code for a simple navigation property.
    /// </summary>
    /// <param name="property">The handled original property.</param>
    /// <returns>The created code.</returns>
    private string BuildNavigationCode(PropertyInfo property)
    {
        var propertyTypeName = property.PropertyType.Name.Split('.').Last();
        var propertyType = property.PropertyType;

        return $@"
    /// <summary>
    /// Gets the raw object of <see cref=""{property.Name}"" />.
    /// </summary>
    [System.Text.Json.Serialization.JsonPropertyName(""{property.Name.ToCamelCase()}"")]
    public {propertyTypeName} Raw{property.Name}
    {{
        get => base.{property.Name} as {propertyTypeName};
        {(property.GetSetMethod(true) is { } ? $"set => base.{property.Name} = value;" : null)}
    }}

    /// <inheritdoc/>
    [System.Text.Json.Serialization.JsonIgnore]
    public override {propertyType.FullName} {property.Name}
    {{
        get => base.{property.Name};
        {(property.GetSetMethod(true) is { } ? $"{(property.GetSetMethod() is null ? "protected " : null)}set => base.{property.Name} = value;" : null)}
    }}";
    }

    /// <summary>
    /// Builds the wrapper code for a collection navigation property.
    /// </summary>
    /// <param name="property">The handled original property.</param>
    /// <returns>The created code.</returns>
    private string BuildCollectionCode(PropertyInfo property)
    {
        var propertyType = property.PropertyType;
        var persistentClassName = propertyType.GetGenericArguments()[0].Name;
        var originalClassName = propertyType.GetGenericArguments()[0].FullName;

        var originalPropertyTypeName = propertyType.GetCSharpFullName();
        var propertyTypeName = propertyType.GetCSharpName();

        var adapterClass = propertyType.GetGenericTypeDefinition() == typeof(IList<>) ? "ListAdapter" : "CollectionAdapter";

        return $@"
    /// <summary>
    /// Gets the raw collection of <see cref=""{property.Name}"" />.
    /// </summary>
    [System.Text.Json.Serialization.JsonPropertyName(""{property.Name.ToCamelCase()}"")]
    public {propertyTypeName} Raw{property.Name} {{ get; }} = new List<{persistentClassName}>();
    
    /// <inheritdoc/>
    [System.Text.Json.Serialization.JsonIgnore]
    public override {originalPropertyTypeName} {property.Name}
    {{
        get => base.{property.Name} ??= new {adapterClass}<{originalClassName}, {persistentClassName}>(this.Raw{property.Name});
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

    /// <summary>
    /// Builds the code for an Id-Property, if the type has none yet.
    /// </summary>
    /// <param name="type">The handled type.</param>
    /// <returns>The created code.</returns>
    private string CreateIdPropertyIfRequired(Type type)
    {
        if (type.GetProperty("Id") is null)
        {
            return @"/// <summary>
    /// Gets or sets the identifier of this instance.
    /// </summary>
    public Guid Id { get; set; }";
        }

        return string.Empty;
    }

    /// <summary>
    /// Creates the constructors for the new type, if required.
    /// </summary>
    /// <param name="type">The inherited type.</param>
    /// <returns>The constructors.</returns>
    private string CreateConstructors(Type type)
    {
        var stringBuilder = new StringBuilder();
        var className = type.Name;
        if (type.GetConstructors().Any(c => c.IsPublic && c.GetParameters().Length > 0)
            && type.GetConstructors().Any(c => c.GetParameters().Length == 0))
        {
            stringBuilder.AppendLine(@$"/// <inheritdoc />
    public {className}()
    {{
    }}");
        }

        foreach (var constructor in type.GetConstructors()
                     .Where(c => c.IsPublic && c.GetParameters().Length > 0))
        {
            var parameters = constructor.GetParameters();
            stringBuilder.AppendLine(@$"
    /// <inheritdoc />
    public {className}({GetParameterDefinitions(parameters)})
        : base({GetParameters(parameters)})
    {{
    }}");
        }

        return stringBuilder.ToString();
    }
}
// <copyright file="GameConfigurationLoaderGenerator.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.DesignTime;

using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Generates the code of the <c>GameConfigurationLoader</c>, which loads the game configuration
/// with one select statement per table, instead of the json query of the <c>GameConfigurationJsonObjectLoader</c>.
/// </summary>
/// <remarks>
/// The generated loader creates the same object graph as the json query and the <c>JsonObjectDeserializer</c>:
/// It loads the same entity types, which are reached from the game configuration through the navigations which are
/// members of the aggregate, and sets the same properties, which are found by the same rules as the
/// <c>ReferenceResolvingConverter</c> does it for the json properties of the query.
/// The tables of the configuration schema are loaded completely; the tables of other schemas
/// (e.g. the items of the merchant stores) are filtered by the objects which reference them.
/// </remarks>
public sealed class GameConfigurationLoaderGenerator
{
    private const string ConfigurationSchema = "config";

    /// <summary>
    /// The maximum depth of the json query. Its aliases go from "a" to "h".
    /// </summary>
    private const int MaximumDepth = 7;

    private static readonly Type[] IgnoredTypes = [typeof(ConstantElement)];

    private readonly IModel _model;
    private readonly IEntityType _rootType;
    private readonly System.Collections.Generic.List<LoadedType> _loadedTypes = [];
    private readonly Dictionary<IEntityType, LoadedType> _loadedTypesByEntityType = [];
    private readonly System.Collections.Generic.List<OneToManyEdge> _oneToManyEdges = [];
    private readonly System.Collections.Generic.List<ManyToManyEdge> _manyToManyEdges = [];
    private readonly System.Collections.Generic.List<ReferenceEdge> _referenceEdges = [];
    private readonly System.Collections.Generic.List<string> _unsafeAccessors = [];

    private GameConfigurationLoaderGenerator(IModel model)
    {
        this._model = model;
        this._rootType = model.FindEntityType(typeof(Model.GameConfiguration)) ?? throw new ArgumentException("The model doesn't contain the game configuration.", nameof(model));
    }

    /// <summary>
    /// The kind of a json property handler.
    /// </summary>
    private enum HandlerKind
    {
        Setter,
        RawAdder,
        JoinedAdder,
        CollectionAdder,
    }

    /// <summary>
    /// Generates the code of the loader for the specified model.
    /// </summary>
    /// <param name="model">The complete model of the <see cref="EntityDataContext"/>.</param>
    /// <returns>The generated code.</returns>
    public static string Generate(IModel model)
    {
        var generator = new GameConfigurationLoaderGenerator(model);
        generator.Visit(generator._rootType, 0, null);
        return generator.GenerateCode();
    }

    private static string GetCSharpName(Type type)
    {
        if (Nullable.GetUnderlyingType(type) is { } underlyingType)
        {
            return GetCSharpName(underlyingType) + "?";
        }

        if (type.IsArray)
        {
            return GetCSharpName(type.GetElementType()!) + "[]";
        }

        if (type.IsGenericType)
        {
            var name = type.GetGenericTypeDefinition().FullName!;
            name = name.Substring(0, name.IndexOf('`', StringComparison.Ordinal));
            return "global::" + name + "<" + string.Join(", ", type.GetGenericArguments().Select(GetCSharpName)) + ">";
        }

        return "global::" + type.FullName!.Replace('+', '.');
    }

    private static string GetIdentifier(IEntityType entityType) => entityType.ClrType.Name;

    private static string Quote(string identifier) => "\"" + identifier + "\"";

    private static string ToLiteral(string value) => "\"" + value.Replace("\\", "\\\\", StringComparison.Ordinal).Replace("\"", "\\\"", StringComparison.Ordinal) + "\"";

    private static void AppendAdd(StringBuilder code, LoadedType parent, string name, JsonPropertyHandler handler, string target, string item)
    {
        var collectionType = GetCSharpName(handler.CollectionInterface ?? throw new NotSupportedException($"The collection {parent.EntityType.Name}.{name} has a json property handler of kind {handler.Kind}."));
        code.Append("                var collection = (").Append(collectionType).Append(')').Append(target).Append('.').Append(handler.Property.Name).AppendLine(";");
        code.Append("                var item = ").Append(item).AppendLine(";");
        if (handler.Kind == HandlerKind.CollectionAdder)
        {
            code.AppendLine("                collection.Add(item);");
        }
        else
        {
            code.AppendLine("                if (!collection.Contains(item))");
            code.AppendLine("                {");
            code.AppendLine("                    collection.Add(item);");
            code.AppendLine("                }");
        }
    }

    /// <summary>
    /// Visits the entity type like the <see cref="Json.JsonQueryBuilder"/> does it, when it builds the json query.
    /// </summary>
    private LoadedType Visit(IEntityType entityType, int depth, SourceFilter? filter)
    {
        var isFirstVisit = false;
        if (!this._loadedTypesByEntityType.TryGetValue(entityType, out var loadedType))
        {
            loadedType = new LoadedType(entityType, this._loadedTypes.Count, entityType.GetSchema() == ConfigurationSchema);
            this._loadedTypes.Add(loadedType);
            this._loadedTypesByEntityType.Add(entityType, loadedType);
            isFirstVisit = true;
        }

        if (filter is not null && !loadedType.IsConfiguration)
        {
            loadedType.Filters.Add(filter);
        }

        if (!isFirstVisit)
        {
            // The navigations of the type are already visited. The rows of all occurrences are loaded by one statement.
            return loadedType;
        }

        if (depth >= MaximumDepth)
        {
            throw new NotSupportedException($"{entityType.Name} is reached at the maximum depth of the json query, where its navigations are not loaded. This isn't supported by the generator.");
        }

        foreach (var navigation in entityType.GetNavigations())
        {
            if (navigation.IsCollection)
            {
                this.VisitCollection(loadedType, navigation, depth);
            }
            else
            {
                this.VisitReference(loadedType, navigation, depth);
            }
        }

        return loadedType;
    }

    private void VisitCollection(LoadedType parent, INavigation navigation, int depth)
    {
        var keyProperty = navigation.ForeignKey.Properties[0];
        var navigationType = (IEntityType)keyProperty.DeclaringType;
#pragma warning disable EF1001 // Internal EF Core API usage.
        if (Microsoft.EntityFrameworkCore.Metadata.Internal.EntityTypeExtensions.FindDeclaredPrimaryKey(navigationType) is not { } primaryKey)
#pragma warning restore EF1001 // Internal EF Core API usage.
        {
            return;
        }

        if (primaryKey.Properties.Count > 1)
        {
            // Many-to-many: the json contains references to the other entities, which are added to the base collection.
            var otherForeignKey = navigationType.GetForeignKeys().First(fk => fk.PrincipalEntityType != parent.EntityType);
            var target = otherForeignKey.PrincipalEntityType;
            var join = new JoinTable(navigationType, keyProperty, otherForeignKey.Properties[0], parent.IsConfiguration ? null : parent);
            this._manyToManyEdges.Add(new ManyToManyEdge(parent, navigation.Name.Replace("Joined", string.Empty, StringComparison.Ordinal), join, target));
            return;
        }

        if (navigation.IsMemberOfAggregate())
        {
            this.Visit(navigationType, depth + 1, new SourceFilter(parent, keyProperty, IsForeignKeyOfParent: false));
        }

        this._oneToManyEdges.Add(new OneToManyEdge(parent, navigation.Name, navigationType, keyProperty));
    }

    private void VisitReference(LoadedType parent, INavigation navigation, int depth)
    {
        if (navigation.ForeignKey.DeclaringEntityType != navigation.DeclaringEntityType)
        {
            // inverse property, no data required
            return;
        }

        var targetType = navigation.TargetEntityType;
        var foreignKey = navigation.ForeignKey.Properties[0];
        if (foreignKey.IsShadowProperty())
        {
            // The json query assumes that every important foreign key is mapped to a "real" property
            return;
        }

        var isBackReference = navigation.ForeignKey.PrincipalToDependent?.IsCollection ?? false;
        var isReference = !navigation.IsMemberOfAggregate() || isBackReference;
        if (!isReference)
        {
            this.Visit(targetType, depth + 1, new SourceFilter(parent, foreignKey, IsForeignKeyOfParent: true));
        }

        this._referenceEdges.Add(new ReferenceEdge(parent, navigation.Name, targetType, parent.GetForeignKeyIndex(foreignKey), isReference));
    }

    private string GenerateCode()
    {
        foreach (var edge in this._oneToManyEdges.Where(e => !this._loadedTypesByEntityType.ContainsKey(e.DependentType)))
        {
            throw new NotSupportedException($"{edge.DependentType.Name} is referenced by {edge.Parent.EntityType.Name}.{edge.Name}, but not loaded.");
        }

        // The foreign keys which are needed for the fixup are kept for each row.
        var oneToManyForeignKeyIndexes = this._oneToManyEdges.ToDictionary(e => e, e => this._loadedTypesByEntityType[e.DependentType].GetForeignKeyIndex(e.KeyProperty));
        foreach (var edge in this._referenceEdges.Where(e => !this._loadedTypesByEntityType.ContainsKey(e.TargetType)))
        {
            throw new NotSupportedException($"{edge.TargetType.Name} is referenced by {edge.Parent.EntityType.Name}.{edge.Name}, but not loaded.");
        }

        foreach (var edge in this._manyToManyEdges.Where(e => !this._loadedTypesByEntityType.ContainsKey(e.TargetType)))
        {
            throw new NotSupportedException($"{edge.TargetType.Name} is referenced by {edge.Parent.EntityType.Name}.{edge.Name}, but not loaded.");
        }

        var joinTables = this._manyToManyEdges.Select(e => e.Join).Distinct().ToList();
        var statements = this._loadedTypes.Select(this.GetSelectStatement).Concat(joinTables.Select(this.GetJoinStatement)).ToList();

        var code = new StringBuilder();
        code.AppendLine("// <auto-generated>");
        code.AppendLine("//     This code was generated by the GameConfigurationLoaderGenerator of the project MUnique.OpenMU.Persistence.EntityFramework.DesignTime.");
        code.AppendLine("//     See Loading/Readme.md.");
        code.AppendLine("// </auto-generated>");
        code.AppendLine();
        code.AppendLine("#nullable enable");
        code.AppendLine();
        code.AppendLine("namespace MUnique.OpenMU.Persistence.EntityFramework.Loading;");
        code.AppendLine();
        code.AppendLine("/// <content>The generated part of the loader.</content>");
        code.AppendLine("[global::System.CodeDom.Compiler.GeneratedCode(\"MUnique.OpenMU.Persistence.EntityFramework.DesignTime\", \"1.0\")]");
        code.AppendLine("internal sealed partial class GameConfigurationLoader");
        code.AppendLine("{");
        code.AppendLine("    private static readonly string[] Statements =");
        code.AppendLine("    [");
        foreach (var statement in statements)
        {
            code.Append("        ").Append(ToLiteral(statement)).AppendLine(",");
        }

        code.AppendLine("    ];");
        code.AppendLine();

        foreach (var loadedType in this._loadedTypes)
        {
            var typeName = GetCSharpName(loadedType.EntityType.ClrType);
            code.Append("    private readonly global::System.Collections.Generic.Dictionary<global::System.Guid, ").Append(typeName).Append("> _").Append(GetIdentifier(loadedType.EntityType)).AppendLine(" = new();");
            if (loadedType.ForeignKeys.Count == 0)
            {
                code.Append("    private readonly global::System.Collections.Generic.List<").Append(typeName).Append("> _").Append(GetIdentifier(loadedType.EntityType)).AppendLine("Rows = new();");
                continue;
            }

            code.Append("    private readonly global::System.Collections.Generic.List<(").Append(typeName).Append(" Entity");
            for (var i = 0; i < loadedType.ForeignKeys.Count; i++)
            {
                code.Append(", global::System.Guid? Key").Append(i);
            }

            code.Append(")> _").Append(GetIdentifier(loadedType.EntityType)).AppendLine("Rows = new();");
        }

        foreach (var join in joinTables)
        {
            code.Append("    private readonly global::System.Collections.Generic.List<(global::System.Guid Key, global::System.Guid Other)> _").Append(GetIdentifier(join.EntityType)).AppendLine("Rows = new();");
        }

        code.AppendLine();
        code.AppendLine("    private async global::System.Threading.Tasks.ValueTask ReadResultAsync(int statementIndex, global::System.Data.Common.DbDataReader reader, global::System.Threading.CancellationToken cancellationToken)");
        code.AppendLine("    {");
        code.AppendLine("        switch (statementIndex)");
        code.AppendLine("        {");
        var index = 0;
        foreach (var loadedType in this._loadedTypes)
        {
            code.Append("            case ").Append(index++).Append(": await this.Read").Append(GetIdentifier(loadedType.EntityType)).AppendLine("Async(reader, cancellationToken).ConfigureAwait(false); break;");
        }

        foreach (var join in joinTables)
        {
            code.Append("            case ").Append(index++).Append(": await ReadJoinAsync(reader, this._").Append(GetIdentifier(join.EntityType)).AppendLine("Rows, cancellationToken).ConfigureAwait(false); break;");
        }

        code.AppendLine("            default: throw new global::System.ArgumentOutOfRangeException(nameof(statementIndex));");
        code.AppendLine("        }");
        code.AppendLine("    }");

        foreach (var loadedType in this._loadedTypes)
        {
            this.AppendReadMethod(code, loadedType);
        }

        this.AppendFixup(code, oneToManyForeignKeyIndexes);
        this.AppendRootsProperty(code);

        foreach (var loadedType in this._loadedTypes)
        {
            var identifier = GetIdentifier(loadedType.EntityType);
            var typeName = GetCSharpName(loadedType.EntityType.ClrType);
            code.AppendLine();
            code.Append("    private ").Append(typeName).Append(" Get").Append(identifier).AppendLine("(global::System.Guid id)");
            code.AppendLine("    {");
            code.Append("        if (!this._").Append(identifier).AppendLine(".TryGetValue(id, out var entity))");
            code.AppendLine("        {");
            code.AppendLine("            // A reference to an object which isn't loaded; the json deserializer creates a placeholder with the id.");
            code.Append("            entity = new ").Append(typeName).AppendLine(" { Id = id };");
            code.Append("            this._").Append(identifier).AppendLine(".Add(id, entity);");
            code.AppendLine("        }");
            code.AppendLine();
            code.AppendLine("        return entity;");
            code.AppendLine("    }");
        }

        foreach (var accessor in this._unsafeAccessors.Distinct())
        {
            code.AppendLine();
            code.Append(accessor);
        }

        code.AppendLine("}");
        return code.ToString().Replace("\r\n", "\n", StringComparison.Ordinal);
    }

    private string GetSelectStatement(LoadedType loadedType)
    {
        var columns = string.Join(", ", loadedType.Columns.Select(c => Quote(c.GetColumnName())));
        var statement = $"select {columns} from {this.GetTableReference(loadedType.EntityType)}";
        if (!loadedType.IsConfiguration)
        {
            statement += " where " + this.GetFilterCondition(loadedType);
        }

        return statement;
    }

    private string GetJoinStatement(JoinTable join)
    {
        var statement = $"select {Quote(join.KeyProperty.GetColumnName())}, {Quote(join.OtherProperty.GetColumnName())} from {this.GetTableReference(join.EntityType)}";
        if (join.ParentFilter is { } parent)
        {
            statement += $" where {Quote(join.KeyProperty.GetColumnName())} in (select {Quote("Id")} from {this.GetSource(parent)})";
        }

        return statement;
    }

    private string GetTableReference(IEntityType entityType) => $"{entityType.GetSchema()}.{Quote(entityType.GetTableName()!)}";

    private string GetSource(LoadedType loadedType)
    {
        return loadedType.IsConfiguration
            ? this.GetTableReference(loadedType.EntityType)
            : $"(select * from {this.GetTableReference(loadedType.EntityType)} where {this.GetFilterCondition(loadedType)}) {Quote(GetIdentifier(loadedType.EntityType))}";
    }

    private string GetFilterCondition(LoadedType loadedType)
    {
        var conditions = loadedType.Filters.Select(filter => filter.IsForeignKeyOfParent
            ? $"{Quote("Id")} in (select {Quote(filter.KeyProperty.GetColumnName())} from {this.GetSource(filter.Parent)})"
            : $"{Quote(filter.KeyProperty.GetColumnName())} in (select {Quote("Id")} from {this.GetSource(filter.Parent)})");
        return "(" + string.Join(" or ", conditions.Distinct()) + ")";
    }

    private void AppendReadMethod(StringBuilder code, LoadedType loadedType)
    {
        var identifier = GetIdentifier(loadedType.EntityType);
        var typeName = GetCSharpName(loadedType.EntityType.ClrType);
        var handlers = JsonPropertyHandler.GetHandlers(loadedType.EntityType.ClrType);
        code.AppendLine();
        code.Append("    private async global::System.Threading.Tasks.ValueTask Read").Append(identifier).AppendLine("Async(global::System.Data.Common.DbDataReader reader, global::System.Threading.CancellationToken cancellationToken)");
        code.AppendLine("    {");
        code.AppendLine("        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))");
        code.AppendLine("        {");
        code.Append("            var entity = new ").Append(typeName).AppendLine("();");
        for (var ordinal = 0; ordinal < loadedType.Columns.Count; ordinal++)
        {
            var column = loadedType.Columns[ordinal];
            if (!handlers.TryGetValue(column.GetColumnName(), out var handler) || handler.Kind != HandlerKind.Setter || handler.IsIgnored)
            {
                continue;
            }

            this.AppendScalarAssignment(code, loadedType, column, ordinal, handler);
        }

        code.Append("            this._").Append(identifier).AppendLine("[entity.Id] = entity;");
        if (loadedType.ForeignKeys.Count == 0)
        {
            code.Append("            this._").Append(identifier).AppendLine("Rows.Add(entity);");
        }
        else
        {
            code.Append("            this._").Append(identifier).Append("Rows.Add((entity");
            foreach (var foreignKey in loadedType.ForeignKeys)
            {
                var ordinal = loadedType.Columns.IndexOf(foreignKey);
                code.Append(", reader.IsDBNull(").Append(ordinal).Append(") ? null : reader.GetGuid(").Append(ordinal).Append(')');
            }

            code.AppendLine("));");
        }
        code.AppendLine("        }");
        code.AppendLine("    }");
    }

    private void AppendScalarAssignment(StringBuilder code, LoadedType loadedType, IProperty column, int ordinal, JsonPropertyHandler handler)
    {
        var storeType = column.GetColumnType();
        var propertyType = handler.PropertyType;
        var underlyingType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
        var read = (storeType, underlyingType) switch
        {
            ("uuid", var t) when t == typeof(Guid) => $"reader.GetGuid({ordinal})",
            ("text", var t) when t == typeof(string) => $"reader.GetString({ordinal})",
            ("text", var t) when t == typeof(LocalizedString) => $"new global::MUnique.OpenMU.Interfaces.LocalizedString(reader.GetString({ordinal}))",
            ("boolean", var t) when t == typeof(bool) => $"reader.GetBoolean({ordinal})",
            ("real", var t) when t == typeof(float) => $"reader.GetFloat({ordinal})",
            ("double precision", var t) when t == typeof(double) => $"reader.GetDouble({ordinal})",
            ("smallint", var t) when t == typeof(short) => $"reader.GetInt16({ordinal})",
            ("smallint", var t) when t == typeof(byte) || t.IsEnum => $"({GetCSharpName(t)})reader.GetInt16({ordinal})",
            ("integer", var t) when t == typeof(int) => $"reader.GetInt32({ordinal})",
            ("integer", var t) when t.IsEnum => $"({GetCSharpName(t)})reader.GetInt32({ordinal})",
            ("bigint", var t) when t == typeof(long) => $"reader.GetInt64({ordinal})",
            ("interval", var t) when t == typeof(TimeSpan) => $"reader.GetFieldValue<global::System.TimeSpan>({ordinal})",
            ("bytea", var t) when t == typeof(byte[]) => $"reader.GetFieldValue<byte[]>({ordinal})",
            _ => throw new NotSupportedException($"The column {loadedType.EntityType.Name}.{column.Name} of type {storeType} can't be read into {propertyType}."),
        };

        var assignment = this.GetAssignment(handler.Property, "entity", read);
        if (underlyingType == typeof(LocalizedString))
        {
            // The json converter of the localized string returns the default value for null.
            code.Append("            ").AppendLine(this.GetAssignment(handler.Property, "entity", $"reader.IsDBNull({ordinal}) ? default : {read}"));
        }
        else if (column.IsNullable)
        {
            // The json deserializer doesn't assign null values.
            code.Append("            if (!reader.IsDBNull(").Append(ordinal).Append(")) { ").Append(assignment).AppendLine(" }");
        }
        else
        {
            code.Append("            ").AppendLine(assignment);
        }
    }

    private string GetAssignment(PropertyInfo property, string target, string value)
    {
        var setter = property.SetMethod!;
        if (setter.IsPublic)
        {
            return $"{target}.{property.Name} = {value};";
        }

        var accessorName = $"Set{property.DeclaringType!.Name}{property.Name}";
        this._unsafeAccessors.Add(
            $"    [global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.Method, Name = \"{setter.Name}\")]\n" +
            $"    private static extern void {accessorName}({GetCSharpName(property.DeclaringType)} target, {GetCSharpName(property.PropertyType)} value);\n");
        return $"{accessorName}({target}, {value});";
    }

    private void AppendFixup(StringBuilder code, Dictionary<OneToManyEdge, int> oneToManyForeignKeyIndexes)
    {
        code.AppendLine();
        code.AppendLine("    private void Fixup()");
        code.AppendLine("    {");
        foreach (var edge in this._referenceEdges)
        {
            var handlers = JsonPropertyHandler.GetHandlers(edge.Parent.EntityType.ClrType);
            if (!handlers.TryGetValue(edge.Name, out var handler) || handler.IsIgnored)
            {
                continue;
            }

            if (handler.Kind != HandlerKind.Setter)
            {
                throw new NotSupportedException($"The reference {edge.Parent.EntityType.Name}.{edge.Name} has a json property handler of kind {handler.Kind}.");
            }

            var parent = GetIdentifier(edge.Parent.EntityType);
            var target = GetIdentifier(edge.TargetType);
            code.Append("        foreach (var row in this._").Append(parent).AppendLine("Rows)");
            code.AppendLine("        {");
            if (edge.IsReference)
            {
                code.Append("            if (row.Key").Append(edge.ForeignKeyIndex).AppendLine(" is { } id)");
                code.AppendLine("            {");
                code.Append("                ").AppendLine(this.GetAssignment(handler.Property, "row.Entity", $"this.Get{target}(id)"));
            }
            else
            {
                code.Append("            if (row.Key").Append(edge.ForeignKeyIndex).Append(" is { } id && this._").Append(target).AppendLine(".TryGetValue(id, out var target))");
                code.AppendLine("            {");
                code.Append("                ").AppendLine(this.GetAssignment(handler.Property, "row.Entity", "target"));
            }

            code.AppendLine("            }");
            code.AppendLine("        }");
            code.AppendLine();
        }

        foreach (var edge in this._oneToManyEdges)
        {
            var handlers = JsonPropertyHandler.GetHandlers(edge.Parent.EntityType.ClrType);
            if (!handlers.TryGetValue(edge.Name, out var handler) || handler.IsIgnored)
            {
                continue;
            }

            var parent = GetIdentifier(edge.Parent.EntityType);
            var dependent = GetIdentifier(edge.DependentType);
            code.Append("        foreach (var row in this._").Append(dependent).AppendLine("Rows)");
            code.AppendLine("        {");
            code.Append("            if (row.Key").Append(oneToManyForeignKeyIndexes[edge]).Append(" is { } id && this._").Append(parent).AppendLine(".TryGetValue(id, out var parent))");
            code.AppendLine("            {");
            AppendAdd(code, edge.Parent, edge.Name, handler, "parent", "row.Entity");
            code.AppendLine("            }");
            code.AppendLine("        }");
            code.AppendLine();
        }

        foreach (var edge in this._manyToManyEdges)
        {
            var handlers = JsonPropertyHandler.GetHandlers(edge.Parent.EntityType.ClrType);
            if (!handlers.TryGetValue(edge.Name, out var handler) || handler.IsIgnored)
            {
                continue;
            }

            var parent = GetIdentifier(edge.Parent.EntityType);
            code.Append("        foreach (var (key, other) in this._").Append(GetIdentifier(edge.Join.EntityType)).AppendLine("Rows)");
            code.AppendLine("        {");
            code.Append("            if (this._").Append(parent).AppendLine(".TryGetValue(key, out var parent))");
            code.AppendLine("            {");
            AppendAdd(code, edge.Parent, edge.Name, handler, "parent", $"this.Get{GetIdentifier(edge.TargetType)}(other)");
            code.AppendLine("            }");
            code.AppendLine("        }");
            code.AppendLine();
        }

        code.AppendLine("    }");
    }

    private void AppendRootsProperty(StringBuilder code)
    {
        code.AppendLine();
        code.Append("    private global::System.Collections.Generic.IEnumerable<global::MUnique.OpenMU.Persistence.EntityFramework.Model.GameConfiguration> LoadedGameConfigurations => ")
            .AppendLine(this._loadedTypesByEntityType[this._rootType].ForeignKeys.Count == 0
                ? "this._GameConfigurationRows;"
                : "global::System.Linq.Enumerable.Select(this._GameConfigurationRows, row => row.Entity);");
    }

    /// <summary>
    /// A loaded entity type.
    /// </summary>
    private sealed class LoadedType
    {
        public LoadedType(IEntityType entityType, int index, bool isConfiguration)
        {
            this.EntityType = entityType;
            this.Index = index;
            this.IsConfiguration = isConfiguration;
            this.Columns = entityType.GetProperties().ToList();
        }

        public IEntityType EntityType { get; }

        public int Index { get; }

        public bool IsConfiguration { get; }

        public System.Collections.Generic.List<IProperty> Columns { get; }

        public System.Collections.Generic.List<IProperty> ForeignKeys { get; } = [];

        public System.Collections.Generic.List<SourceFilter> Filters { get; } = [];

        public int GetForeignKeyIndex(IReadOnlyProperty property)
        {
            var column = this.Columns.First(c => c.Name == property.Name);
            if (column.ClrType != typeof(Guid) && column.ClrType != typeof(Guid?))
            {
                throw new NotSupportedException($"The foreign key {this.EntityType.Name}.{property.Name} is not a Guid.");
            }

            var index = this.ForeignKeys.IndexOf(column);
            if (index < 0)
            {
                index = this.ForeignKeys.Count;
                this.ForeignKeys.Add(column);
            }

            return index;
        }
    }

    /// <summary>
    /// A filter for an entity type of another schema than the configuration.
    /// </summary>
    /// <param name="Parent">The parent type, which references the entity type.</param>
    /// <param name="KeyProperty">The key property.</param>
    /// <param name="IsForeignKeyOfParent">If set to <c>true</c>, the key property is a foreign key of the parent; otherwise, it's the foreign key of the entity type to the parent.</param>
    private sealed record SourceFilter(LoadedType Parent, IReadOnlyProperty KeyProperty, bool IsForeignKeyOfParent);

    /// <summary>
    /// A join table of a many-to-many relationship.
    /// </summary>
    /// <param name="EntityType">The join entity type.</param>
    /// <param name="KeyProperty">The property which references the parent.</param>
    /// <param name="OtherProperty">The property which references the other entity.</param>
    /// <param name="ParentFilter">The parent, if the join table needs to be filtered by it.</param>
    private sealed record JoinTable(IEntityType EntityType, IReadOnlyProperty KeyProperty, IReadOnlyProperty OtherProperty, LoadedType? ParentFilter);

    /// <summary>
    /// A one-to-many relationship, whose dependents are added to a collection of the parent.
    /// </summary>
    private sealed record OneToManyEdge(LoadedType Parent, string Name, IEntityType DependentType, IReadOnlyProperty KeyProperty);

    /// <summary>
    /// A many-to-many relationship, whose other entities are added to a collection of the parent.
    /// </summary>
    private sealed record ManyToManyEdge(LoadedType Parent, string Name, JoinTable Join, IEntityType TargetType);

    /// <summary>
    /// A reference of the parent to another entity.
    /// </summary>
    private sealed record ReferenceEdge(LoadedType Parent, string Name, IEntityType TargetType, int ForeignKeyIndex, bool IsReference);


    /// <summary>
    /// The handler of a json property, determined with the same rules as the <c>ReferenceResolvingConverter</c>.
    /// </summary>
    private sealed record JsonPropertyHandler(HandlerKind Kind, PropertyInfo Property, Type PropertyType, Type? CollectionInterface)
    {
        public bool IsIgnored => IgnoredTypes.Contains(this.PropertyType) || IgnoredTypes.Contains(this.PropertyType.BaseType);

        public static Dictionary<string, JsonPropertyHandler> GetHandlers(Type type)
        {
            var properties = type.GetProperties()
                .Where(p => p.GetCustomAttribute<JsonIgnoreAttribute>() is null)
                .ToList();
            var result = new Dictionary<string, JsonPropertyHandler>(StringComparer.InvariantCultureIgnoreCase);
            foreach (var property in properties)
            {
                var collectionInterface = property.PropertyType.IsGenericType ? DetermineCollectionInterface(property.PropertyType) : null;
                var jsonPropertyName = property.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? property.Name;
                JsonPropertyHandler? handler = null;
                if (collectionInterface is not null && property.Name.StartsWith("Raw", StringComparison.Ordinal))
                {
                    handler = new JsonPropertyHandler(HandlerKind.RawAdder, property, collectionInterface.GetGenericArguments()[0], collectionInterface);
                }
                else if (collectionInterface is not null && property.Name.StartsWith("Joined", StringComparison.Ordinal))
                {
                    var basePropertyName = property.Name.Substring("Joined".Length);
                    var baseCollectionProperty = properties.First(p => p.Name == basePropertyName);
                    var baseType = baseCollectionProperty.PropertyType.GetGenericArguments()[0];
                    var itemType = collectionInterface.GetGenericArguments()[0].GetProperties().First(p => p.PropertyType.BaseType == baseType).PropertyType;
                    jsonPropertyName = basePropertyName;
                    handler = new JsonPropertyHandler(HandlerKind.JoinedAdder, baseCollectionProperty, itemType, DetermineCollectionInterface(baseCollectionProperty.PropertyType));
                }
                else if (property.CanWrite && properties.All(p => p.Name != "Joined" + property.Name))
                {
                    handler = new JsonPropertyHandler(HandlerKind.Setter, property, property.PropertyType, null);
                }
                else if (collectionInterface is not null && properties.All(p => p.Name != "Raw" + property.Name && p.Name != "Joined" + property.Name))
                {
                    handler = new JsonPropertyHandler(HandlerKind.CollectionAdder, property, collectionInterface.GetGenericArguments()[0], collectionInterface);
                }

                if (handler is not null)
                {
                    result.Add(jsonPropertyName, handler);
                }
            }

            return result;
        }

        private static Type? DetermineCollectionInterface(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ICollection<>)
                ? type
                : type.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICollection<>));
        }
    }
}

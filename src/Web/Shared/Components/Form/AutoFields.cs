// <copyright file="AutoFields.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.Form;

using System.Collections.Concurrent;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Composition;
using MUnique.OpenMU.Web.Shared.ComponentBuilders;
using MUnique.OpenMU.Web.Shared.Services;

/// <summary>
/// A razor component which automatically generates input fields for all properties for the type of the enclosing form model.
/// Must be used inside a <see cref="EditForm"/>.
/// </summary>
public class AutoFields : ComponentBase
{
    private static readonly IList<IComponentBuilder> Builders = new List<IComponentBuilder>();
    private static readonly ConcurrentDictionary<Type, IReadOnlyList<PropertyMetadata>> CachedProperties = new();

    /// <summary>
    /// Initializes static members of the <see cref="AutoFields"/> class.
    /// </summary>
    static AutoFields()
    {
        Builders.Add(new PasswordHashFieldBuilder());
        Builders.Add(new TextFieldBuilder());
        Builders.Add(new LocalizedStringFieldBuilder());
        Builders.Add(new NumberFieldBuilder<long>());
        Builders.Add(new NumberFieldBuilder<int>());
        Builders.Add(new NumberFieldBuilder<decimal>());
        Builders.Add(new NumberFieldBuilder<double>());
        Builders.Add(new NumberFieldBuilder<float>());
        Builders.Add(new ByteFieldBuilder());
        Builders.Add(new ShortFieldBuilder());
        Builders.Add(new BooleanFieldBuilder());
        Builders.Add(new DateTimeFieldBuilder());
        Builders.Add(new DateOnlyFieldBuilder());
        Builders.Add(new TimeOnlyFieldBuilder());
        Builders.Add(new TimeSpanFieldBuilder());
        Builders.Add(new EnumFieldBuilder());
        Builders.Add(new FlagsEnumFieldBuilder());
        Builders.Add(new ExitGateFieldBuilder());
        Builders.Add(new ItemStorageFieldBuilder());
        Builders.Add(new LookupFieldBuilder());
        Builders.Add(new EmbeddedFormFieldBuilder());
        Builders.Add(new ObjectCollectionFieldBuilder());
        Builders.Add(new IntCollectionFieldBuilder());
        Builders.Add(new ByteArrayFieldBuilder());
        Builders.Add(new ValueListFieldBuilder());
    }

    /// <summary>
    /// Gets or sets the context of the <see cref="EditForm"/>.
    /// </summary>
    /// <value>
    /// The context.
    /// </value>
    [CascadingParameter]
    public EditContext Context { get; set; } = null!;

    /// <summary>
    /// Gets or sets a value indicating whether to hide collections or not.
    /// </summary>
    [Parameter]
    public bool HideCollections { get; set; }

    /// <summary>
    /// Gets or sets the search term to filter properties by their name or display name.
    /// </summary>
    [Parameter]
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Gets the properties which should be shown in this component.
    /// </summary>
    /// <returns>The properties which should be shown in this component.</returns>
    protected virtual IEnumerable<PropertyInfo> Properties
    {
        get
        {
            if (this.Context?.Model is null)
            {
                this.Logger.LogError(this.Context is null ? "Context is null" : "Model is null");
                return Enumerable.Empty<PropertyInfo>();
            }

            try
            {
                var modelType = this.Context.Model.GetType();
                var properties = CachedProperties.GetOrAdd(modelType, CreatePropertyMetadata);

                return properties
                    .Where(p => !this.HideCollections || !p.IsGenericType)
                    .Where(this.IsMatch)
                    .OrderBy(p => p.DisplayAttribute?.GetOrder())
                    .ThenByDescending(p => p.IsString)
                    .ThenByDescending(p => p.IsValueType)
                    .ThenByDescending(p => !p.IsGenericType)
                    .Select(p => p.Property)
                    .ToList();
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "Error during determining properties of type {ModelType}: {Message}", this.Context.Model.GetType(), ex.Message);
            }

            return Enumerable.Empty<PropertyInfo>();
        }
    }

    /// <summary>
    /// Gets or sets the notification service.
    /// </summary>
    /// <value>
    /// The notification service.
    /// </value>
    [Inject]
    private IChangeNotificationService NotificationService { get; set; } = null!;

    [Inject]
    private ILogger<AutoFields> Logger { get; set; } = null!;

    /// <inheritdoc />
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        int i = 0;
        foreach (var propertyInfo in this.Properties)
        {
            IComponentBuilder? componentBuilder = null;
            try
            {
                componentBuilder = Builders.FirstOrDefault(b => b.CanBuildComponent(propertyInfo));
                if (componentBuilder != null)
                {
                    // TODO: Build something around groups (same DisplayAttribute.GroupName)
                    i = componentBuilder.BuildComponent(this.Context.Model, propertyInfo, builder, i, this.NotificationService);
                }
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, $"Error building component for property {this.Context.Model.GetType().Name}.{propertyInfo.Name} with component builder {componentBuilder}: {ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }
    }

    private static IReadOnlyList<PropertyMetadata> CreatePropertyMetadata(Type type)
    {
        return type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy)
            .Where(p => p.GetCustomAttribute<TransientAttribute>() is null)
            .Where(p => p.GetCustomAttribute<BrowsableAttribute>()?.Browsable ?? true)
            .Where(p => !p.Name.StartsWith("Raw", StringComparison.Ordinal))
            .Where(p => !p.Name.StartsWith("Joined", StringComparison.Ordinal))
            .Where(p => !p.GetIndexParameters().Any())
            .Select(p => new PropertyMetadata(
                p,
                p.GetCustomAttribute<DisplayAttribute>(),
                p.PropertyType.IsGenericType,
                p.PropertyType == typeof(string),
                p.PropertyType.IsValueType))
            .ToList();
    }

    private bool IsMatch(PropertyMetadata metadata)
    {
        if (string.IsNullOrWhiteSpace(this.SearchTerm))
        {
            return true;
        }

        return metadata.Property.Name.Contains(this.SearchTerm, StringComparison.OrdinalIgnoreCase)
               || (metadata.DisplayAttribute?.GetName()?.Contains(this.SearchTerm, StringComparison.OrdinalIgnoreCase) ?? false);
    }

    private sealed record PropertyMetadata(
        PropertyInfo Property,
        DisplayAttribute? DisplayAttribute,
        bool IsGenericType,
        bool IsString,
        bool IsValueType);
}

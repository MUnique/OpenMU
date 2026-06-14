// <copyright file="ModelBuilderExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.Extensions;

/// <summary>
/// Extensions for <see cref="Microsoft.EntityFrameworkCore.ModelBuilder"/>.
/// </summary>
internal static class ModelBuilderExtensions
{
    /// <summary>
    /// Configures the model builder to use UUID V7 as primary keys.
    /// </summary>
    /// <param name="modelBuilder">The model builder to configure.</param>
    /// <returns>The configured model builder with UUID V7 key generation applied.</returns>
    public static Microsoft.EntityFrameworkCore.ModelBuilder UseGuidV7Ids(this Microsoft.EntityFrameworkCore.ModelBuilder modelBuilder)
    {
        var types = modelBuilder.Model.GetEntityTypes();
        foreach (var t in types)
        {
            var entity = modelBuilder.Entity(t.ClrType);
            var key = entity.Metadata.FindProperty("Id");
            if (key != null)
            {
                key.ValueGenerated = Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.OnAdd;
                key.SetValueGeneratorFactory((_, _) => new GuidV7ValueGenerator());
            }
        }

        return modelBuilder;
    }
}
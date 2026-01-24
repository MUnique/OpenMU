// <copyright file="ModelBuilderExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.PlugIns;

namespace MUnique.OpenMU.Persistence.EntityFramework.Extensions;

/// <summary>
/// Extensions for <see cref="Microsoft.EntityFrameworkCore.ModelBuilder"/>.
/// </summary>
internal static class ModelBuilderExtensions
{
    /// <summary>
    /// Configures the model builder to use UUID V7 as primary keys.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    /// <returns>The model builder.</returns>
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

    ///// <summary>
    ///// Configures the model builder to convert <see cref="LocalizedString"/> to string and vice-versa.
    ///// </summary>
    ///// <param name="modelBuilder">The model builder.</param>
    ///// <returns>The model builder.</returns>
    //public static Microsoft.EntityFrameworkCore.ModelBuilder UseLocalizedStringConverter(this Microsoft.EntityFrameworkCore.ModelBuilder modelBuilder)
    //{
    //    var converter = new LocalizedStringConverter();
    //    var types = modelBuilder.Model.GetEntityTypes();
    //    foreach (var t in types)
    //    {
    //        var entity = modelBuilder.Entity(t.ClrType);
    //        var localizableStrings = entity.Metadata.GetProperties().Where(p => p.ClrType.GetTypeOfNullable() == typeof(LocalizedString));
    //        foreach (var property in localizableStrings)
    //        {
    //            entity.Property(property.Name).HasConversion(converter);
    //        }
    //    }

    //    return modelBuilder;
    //}



}
namespace MUnique.OpenMU.Persistence.EntityFramework.Extensions.ModelBuilder;

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MUnique.OpenMU.Persistence.EntityFramework.Model;

internal static class ItemExtensions
{
    /// <summary>
    /// Extension for ItemStorage entity.
    /// centralize logic for apply configuration on database with code first.
    /// </summary>
    public static void Apply(this EntityTypeBuilder<ItemStorage> builder)
    {
        builder.HasMany(storage => storage.RawItems).WithOne(item => item.RawItemStorage!);
    }

    /// <summary>
    /// Extension for ItemSetGroup entity.
    /// centralize logic for apply configuration on database with code first.
    /// </summary>
    public static void Apply(this EntityTypeBuilder<ItemSetGroup> builder)
    {
        builder.HasMany(isg => isg.RawItems).WithOne(item => item.RawItemSetGroup!);
    }
}
namespace MUnique.OpenMU.Persistence.EntityFramework.Extensions.ModelBuilder;

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MUnique.OpenMU.Persistence.EntityFramework.Model;

internal static class ItemExtensions
{
    public static void Apply(this EntityTypeBuilder<ItemStorage> builder)
    {
        builder.HasMany(storage => storage.RawItems).WithOne(item => item.RawItemStorage!);
    }

    public static void Apply(this EntityTypeBuilder<ItemSetGroup> builder)
    {
        builder.HasMany(isg => isg.RawItems).WithOne(item => item.RawItemSetGroup!);
    }
}
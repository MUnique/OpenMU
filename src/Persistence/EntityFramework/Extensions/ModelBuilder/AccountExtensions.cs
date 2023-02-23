namespace MUnique.OpenMU.Persistence.EntityFramework.Extensions.ModelBuilder;

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MUnique.OpenMU.Persistence.EntityFramework.Model;

internal static class AccountExtensions
{
    /// <summary>
    /// Extension for Account entity.
    /// centralize logic for apply configuration on database with code first.
    /// </summary>
    public static void Apply(this EntityTypeBuilder<Account> builder)
    {
        builder.Property(account => account.LoginName).HasMaxLength(10).IsRequired();
        builder.HasIndex(account => account.LoginName).IsUnique();
    }
}
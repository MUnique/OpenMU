namespace MUnique.OpenMU.Persistence.EntityFramework.Extensions.ModelBuilder;

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MUnique.OpenMU.Persistence.EntityFramework.Model;

internal static class AccountExtensions
{
    public static void Apply(this EntityTypeBuilder<Account> builder)
    {
        builder.Property(account => account.LoginName).HasMaxLength(10).IsRequired();
        builder.HasIndex(account => account.LoginName).IsUnique();
    }
}
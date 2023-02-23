namespace MUnique.OpenMU.Persistence.EntityFramework.Extensions.ModelBuilder;

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MUnique.OpenMU.Persistence.EntityFramework.Model;
using Microsoft.EntityFrameworkCore;

internal static class CharacterExtensions
{
    public static void Apply(this EntityTypeBuilder<Character> builder)
    {
        builder.Property(character => character.Name).HasMaxLength(10).IsRequired();
        builder.HasIndex(character => character.Name).IsUnique();
        builder.Metadata.FindNavigation(nameof(Character.RawCharacterClass))!.ForeignKey.IsRequired = true;
        builder.Property(character => character.CharacterSlot).IsRequired();
        builder.HasMany(character => character.RawLetters).WithOne(letter => letter.Receiver!).OnDelete(DeleteBehavior.Cascade);
    }

    public static void Apply(this EntityTypeBuilder<CharacterClass> builder)
    {
        builder.HasMany(c => c.RawBaseAttributeValues)
            .WithOne(c => c.CharacterClass!);
    }
}
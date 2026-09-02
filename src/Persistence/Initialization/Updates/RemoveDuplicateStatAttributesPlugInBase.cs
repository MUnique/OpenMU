// <copyright file="RemoveDuplicateStatAttributesPlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// This update removes stat attributes which are defined more than once for a character class.
/// </summary>
/// <remarks>
/// The <see cref="RegenerationsRefactorPlugInBase"/> added the "Is resting" and "Nearby party member count"
/// stat attributes to every character class without checking if the class already had them. For a
/// configuration which was initialized after these attributes were introduced, that resulted in classes
/// holding the same stat attribute twice - and every character of such a class got the attribute twice as
/// well, which made it impossible to enter the game with it.
/// </remarks>
public abstract class RemoveDuplicateStatAttributesPlugInBase : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Remove duplicate stat attributes";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update removes stat attributes which are defined more than once for a character class.";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 9, 1, 12, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        foreach (var characterClass in gameConfiguration.CharacterClasses)
        {
            var duplicates = characterClass.StatAttributes
                .GroupBy(statAttribute => statAttribute.Attribute)
                .Where(group => group.Count() > 1)
                .SelectMany(group => group.Skip(1))
                .ToList();

            duplicates.ForEach(duplicate => characterClass.StatAttributes.Remove(duplicate));
        }

        return ValueTask.CompletedTask;
    }
}

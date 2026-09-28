// <copyright file="SpanishHelp.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared;

/// <summary>
/// Help texts in Spanish for the fields of the admin panel: what a field is and, where it matters, what changing it affects.
/// The labels stay in English; these texts are shown in the "info" popup of each label, above the original description.
/// </summary>
/// <remarks>
/// The entries live in partial files by area (items, monsters, ...), so adding help never conflicts with the upstream code.
/// Keys are "Type.Property", where the type is the simple name of the model class (or view model) which declares the property.
/// </remarks>
public static partial class SpanishHelp
{
    private static readonly Lazy<Dictionary<string, Entry>> LazyEntries = new(Build);

    /// <summary>
    /// The help of one field.
    /// </summary>
    /// <param name="Info">What the field is.</param>
    /// <param name="Impact">What changing it affects. Optional.</param>
    public sealed record Entry(string Info, string? Impact = null);

    /// <summary>
    /// Tries to find the help of a property. Base classes are searched too, because the panel edits derived (persistence) types.
    /// </summary>
    /// <param name="modelType">The type of the edited object.</param>
    /// <param name="propertyName">The name of the property.</param>
    /// <param name="entry">The help, if there is one.</param>
    /// <returns><c>True</c>, if there is help for the property.</returns>
    public static bool TryGet(Type modelType, string propertyName, out Entry entry)
    {
        for (var type = modelType; type is not null; type = type.BaseType)
        {
            if (LazyEntries.Value.TryGetValue($"{type.Name}.{propertyName}", out var found))
            {
                entry = found;
                return true;
            }
        }

        entry = null!;
        return false;
    }

    private static Dictionary<string, Entry> Build()
    {
        var entries = new Dictionary<string, Entry>(StringComparer.Ordinal);
        AddItems(entries);
        AddMonsters(entries);
        AddGameConfiguration(entries);
        AddMaps(entries);
        return entries;
    }

    private static void Add(Dictionary<string, Entry> entries, string key, string info, string? impact = null)
    {
        entries[key] = new Entry(info, impact);
    }
}

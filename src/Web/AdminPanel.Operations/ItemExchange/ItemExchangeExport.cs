// <copyright file="ItemExchangeExport.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations.ItemExchange;

using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Persistence;

/// <summary>
/// Writes the items of a game configuration as an item exchange file of the MuMain client,
/// which its editor (MuEditor) imports with a diff the user checks by hand.
/// </summary>
/// <remarks>
/// The file uses the client's keys and meanings, see the client's <c>docs/item-data.md</c>.
/// Its <c>fields</c> list names the item fields the file carries; the client compares and
/// applies only these and keeps its own values of every other field. A file with this list
/// has the format version 2, so that a client which doesn't know the list refuses the file
/// instead of reading the fields it lacks as zero.
/// </remarks>
public static class ItemExchangeExport
{
    /// <summary>
    /// The format version of a file with a <c>fields</c> list.
    /// </summary>
    public const int FormatVersion = 2;

    /// <summary>
    /// The source which the file names.
    /// </summary>
    public const string Source = "OpenMU";

    /// <summary>
    /// The game version of the client's data, the key of the Season 6 data initialization.
    /// The client refuses a file of another version.
    /// </summary>
    public const string ClientGameVersion = "season6";

    /// <summary>
    /// The highest item group the client knows.
    /// </summary>
    public const int MaximumGroup = 15;

    /// <summary>
    /// The highest item number within a group the client knows.
    /// </summary>
    public const int MaximumNumber = 511;

    /// <summary>
    /// Gets the item fields the file carries, in the order the client writes them.
    /// </summary>
    internal static IReadOnlyList<ItemExchangeField> Fields { get; } = [];

    /// <summary>
    /// Writes the exchange file of the items of the configuration.
    /// </summary>
    /// <param name="configuration">The game configuration.</param>
    /// <param name="gameVersion">
    /// The game version of the configuration, the key of the data initialization which created it.
    /// The client only reads a file of its own version, <see cref="ClientGameVersion"/>.
    /// </param>
    /// <returns>The file as UTF-8 text with LF line endings, without a byte order mark.</returns>
    /// <remarks>
    /// Items are sorted by group and number. An item outside the client's groups or numbers
    /// is left out, because the client refuses the whole file because of it.
    /// </remarks>
    public static byte[] Write(GameConfiguration configuration, string gameVersion)
    {
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream, new JsonWriterOptions
        {
            Indented = true,
            NewLine = "\n",

            // The names stay readable, as in the client's files.
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        }))
        {
            writer.WriteStartObject();
            writer.WriteNumber("formatVersion", FormatVersion);
            writer.WriteString("source", Source);
            writer.WriteString("gameVersion", gameVersion);
            writer.WriteStartArray("fields");
            foreach (var field in Fields)
            {
                writer.WriteStringValue(field.Key);
            }

            writer.WriteEndArray();
            writer.WriteStartArray("items");
            foreach (var item in GetExportedItems(configuration))
            {
                writer.WriteStartObject();
                writer.WriteNumber("group", item.Group);
                writer.WriteNumber("number", item.Number);
                foreach (var field in Fields)
                {
                    field.Write(writer, item);
                }

                writer.WriteEndObject();
            }

            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        stream.WriteByte((byte)'\n');
        return stream.ToArray();
    }

    /// <summary>
    /// Gets the items which the file carries: the ones within the client's groups and numbers,
    /// sorted by group and number.
    /// </summary>
    /// <param name="configuration">The game configuration.</param>
    /// <returns>The exported items.</returns>
    /// <remarks>
    /// OpenMU doesn't keep the group and number of an item unique. Items which share them are
    /// sorted by their id, so that the same configuration always gives the same file; the order
    /// in which the configuration is loaded can differ.
    /// </remarks>
    internal static IEnumerable<ItemDefinition> GetExportedItems(GameConfiguration configuration)
    {
        return configuration.Items
            .Where(item => item.Group <= MaximumGroup && item.Number is >= 0 and <= MaximumNumber)
            .OrderBy(item => item.Group)
            .ThenBy(item => item.Number)
            .ThenBy(item => item.GetId());
    }
}

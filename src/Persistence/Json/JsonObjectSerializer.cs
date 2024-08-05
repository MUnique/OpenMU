// <copyright file="JsonObjectSerializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Json;

using System.IO;
using System.Text.Json;
using System.Threading;

/// <summary>
/// Class to serialize an object to a json string or stream.
/// </summary>
public class JsonObjectSerializer
{
    /// <summary>
    /// Serializes the specified object into a stream.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="obj">The object.</param>
    /// <param name="stream">The stream.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async ValueTask SerializeAsync<T>(T obj, Stream stream, CancellationToken cancellationToken)
    {
        var options = new JsonSerializerOptions
        {
            ReferenceHandler = new IdReferenceHandler(),
            WriteIndented = true,
            Converters =
            {
                new OnlyWriteBelowRootConverter<BasicModel.ItemDefinition>(),
                new OnlyWriteBelowRootConverter<BasicModel.DropItemGroup>(),
                new OnlyWriteBelowRootConverter<BasicModel.Skill>(),
                new OnlyWriteBelowRootConverter<BasicModel.CharacterClass>(),
                new OnlyWriteBelowRootConverter<BasicModel.ItemLevelBonusTable>(),
                new OnlyWriteBelowRootConverter<BasicModel.ItemSlotType>(),
                new OnlyWriteBelowRootConverter<BasicModel.ItemOptionDefinition>(),
                new OnlyWriteBelowRootConverter<BasicModel.ItemOptionType>(),
                new OnlyWriteBelowRootConverter<BasicModel.ItemSetGroup>(),
                new OnlyWriteBelowRootConverter<BasicModel.ItemOptionCombinationBonus>(),
                new OnlyWriteBelowRootConverter<BasicModel.GameMapDefinition>(),
                new OnlyWriteBelowRootConverter<BasicModel.MonsterDefinition>(),
                new OnlyWriteBelowRootConverter<BasicModel.AttributeDefinition>(),
                new OnlyWriteBelowRootConverter<BasicModel.MagicEffectDefinition>(),
            },
        };

        await JsonSerializer.SerializeAsync(stream, obj, options, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Serializes the specified object into a string.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="obj">The object.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The serialized object as string.</returns>
    public async ValueTask<string> SerializeAsync<T>(T obj, CancellationToken cancellationToken)
    {
        using var stream = new MemoryStream();
        await this.SerializeAsync(obj, stream, cancellationToken).ConfigureAwait(false);

        return Encoding.UTF8.GetString(stream.GetBuffer(), 0, (int)stream.Length);
    }
}
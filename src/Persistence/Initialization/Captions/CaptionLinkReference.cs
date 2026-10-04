// <copyright file="CaptionLinkReference.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Captions;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// The linked captions of a reference configuration (usually a freshly initialized one),
/// which are used to link the captions of another configuration to their sources,
/// see <see cref="ConfigurationCaptions.LinkSourceKeys(GameConfiguration, CaptionLinkReference)"/>.
/// It keeps only the identifying keys and the values of the captions, but not the configuration itself.
/// </summary>
public sealed class CaptionLinkReference
{
    private CaptionLinkReference(IReadOnlyList<Entry> entries)
    {
        this.Entries = entries;
    }

    /// <summary>
    /// Gets the linked captions of the reference configuration.
    /// </summary>
    internal IReadOnlyList<Entry> Entries { get; }

    /// <summary>
    /// Creates the reference of the linked captions of a configuration.
    /// </summary>
    /// <param name="referenceConfiguration">The reference configuration.</param>
    /// <returns>The created reference.</returns>
    public static CaptionLinkReference Create(GameConfiguration referenceConfiguration)
    {
        var entries = LocalizedCaption.FindAll(referenceConfiguration)
            .Where(caption => caption.Value.SourceKey is not null)
            .Select(caption => new Entry(caption.Key, ConfigurationCaptions.GetNumberKey(caption), caption.Value))
            .ToList();
        return new CaptionLinkReference(entries.AsReadOnly());
    }

    /// <summary>
    /// A linked caption of the reference configuration.
    /// </summary>
    /// <param name="Key">The key of the caption, see <see cref="LocalizedCaption.Key"/>.</param>
    /// <param name="NumberKey">The key of the caption by the type and number of its owner, if the owner has a number.</param>
    /// <param name="Value">The value of the caption.</param>
    internal sealed record Entry((Guid OwnerId, string PropertyName) Key, (string OwnerType, long Number, string PropertyName)? NumberKey, LocalizedString Value);
}

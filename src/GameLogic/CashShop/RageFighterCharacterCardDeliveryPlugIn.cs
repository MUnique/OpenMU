// <copyright file="RageFighterCharacterCardDeliveryPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CashShop;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Delivers the Rage Fighter Character Card of the cash shop by unlocking the Rage Fighter class.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.RageFighterCharacterCardDeliveryPlugIn_Name), Description = nameof(PlugInResources.RageFighterCharacterCardDeliveryPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("AAFBDBF5-82EE-48B3-8A73-882926519409")]
public class RageFighterCharacterCardDeliveryPlugIn : CharacterCardDeliveryPlugInBase
{
    /// <summary>
    /// The item of the Rage Fighter Character Card.
    /// </summary>
    public static readonly ItemIdentifier RageFighterCharacterCard = new(169, 14);

    private static readonly byte RageFighterNumber = 24;

    /// <summary>
    /// Initializes a new instance of the <see cref="RageFighterCharacterCardDeliveryPlugIn"/> class.
    /// </summary>
    public RageFighterCharacterCardDeliveryPlugIn()
        : base(RageFighterCharacterCard, RageFighterNumber)
    {
    }
}

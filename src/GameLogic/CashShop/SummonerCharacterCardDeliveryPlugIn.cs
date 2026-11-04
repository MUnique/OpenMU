// <copyright file="SummonerCharacterCardDeliveryPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CashShop;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Delivers the Summoner Character Card of the cash shop by unlocking the Summoner class.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.SummonerCharacterCardDeliveryPlugIn_Name), Description = nameof(PlugInResources.SummonerCharacterCardDeliveryPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("17CF1AEA-92FE-468E-ABBA-A08A4710BC99")]
public class SummonerCharacterCardDeliveryPlugIn : CharacterCardDeliveryPlugInBase
{
    /// <summary>
    /// The item of the Summoner Character Card.
    /// </summary>
    public static readonly ItemIdentifier SummonerCharacterCard = new(91, 14);

    private static readonly byte SummonerNumber = 20;

    /// <summary>
    /// Initializes a new instance of the <see cref="SummonerCharacterCardDeliveryPlugIn"/> class.
    /// </summary>
    public SummonerCharacterCardDeliveryPlugIn()
        : base(SummonerCharacterCard, SummonerNumber)
    {
    }
}

// <copyright file="EnterBarracksOfBalgassAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions;

/// <summary>
/// Action to enter the barracks of balgass through an npc.
/// </summary>
public class EnterBarracksOfBalgassAction : EnterQuestMapAction
{
    private const byte InfiltrationOfBarracksOfBalgassQuestNumber = 5;
    private const byte IntoTheDarknessZoneQuestNumber = 6;
    private const byte LegacyQuestGroup = 0;
    private const short WerewolfNpcNumber = 407;
    private const short BarracksOfBalgassMapNumber = 41;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnterBarracksOfBalgassAction"/> class.
    /// </summary>
    public EnterBarracksOfBalgassAction()
        : base(
            WerewolfNpcNumber,
            3000000,
            BarracksOfBalgassMapNumber,
            LegacyQuestGroup,
            InfiltrationOfBarracksOfBalgassQuestNumber,
            IntoTheDarknessZoneQuestNumber)
    {
    }
}
// <copyright file="EnterBalgassRefugeeAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions;

/// <summary>
/// Action to enter the barracks of balgass through an npc.
/// </summary>
public class EnterBalgassRefugeeAction : EnterQuestMapAction
{
    private const byte IntoTheDarknessZoneQuestNumber = 6;
    private const byte LegacyQuestGroup = 0;
    private const short GatekeepterNpcNumber = 408;
    private const short BalgassRefugeeMapNumber = 42;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnterBalgassRefugeeAction"/> class.
    /// </summary>
    public EnterBalgassRefugeeAction()
        : base(
            GatekeepterNpcNumber,
            0,
            BalgassRefugeeMapNumber,
            LegacyQuestGroup,
            IntoTheDarknessZoneQuestNumber)
    {
    }
}
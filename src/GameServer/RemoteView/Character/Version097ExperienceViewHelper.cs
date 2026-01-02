// <copyright file="Version097ExperienceViewHelper.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character;

using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;

internal static class Version097ExperienceViewHelper
{
    public static (uint Current, uint Next) GetViewExperience(RemotePlayer player)
    {
        var attributes = player.Attributes;
        var selectedCharacter = player.SelectedCharacter;
        if (attributes is null || selectedCharacter is null)
        {
            return (0, 0);
        }

        var expTable = player.GameServerContext.ExperienceTable;
        if (expTable.Length == 0)
        {
            return (0, 0);
        }

        var level = (int)attributes[Stats.Level];
        if (level < 0)
        {
            return (0, 0);
        }

        var currentIndex = Math.Min(level, expTable.Length - 1);
        var nextIndex = Math.Min(level + 1, expTable.Length - 1);
        var expForCurrentLevel = expTable[currentIndex];
        var expForNextLevel = expTable[nextIndex];

        var currentExp = selectedCharacter.Experience;
        var viewExpLong = currentExp < expForCurrentLevel ? expForCurrentLevel + currentExp : currentExp;
        if (viewExpLong > expForNextLevel)
        {
            viewExpLong = expForNextLevel;
        }

        return (ClampToUInt32(viewExpLong), ClampToUInt32(expForNextLevel));
    }

    private static uint ClampToUInt32(long value)
    {
        if (value <= 0)
        {
            return 0;
        }

        if (value >= uint.MaxValue)
        {
            return uint.MaxValue;
        }

        return (uint)value;
    }
}

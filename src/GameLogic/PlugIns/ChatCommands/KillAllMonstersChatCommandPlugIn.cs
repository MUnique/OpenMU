// <copyright file="KillAllMonstersChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which handles the killall command.
/// It kills all alive monsters on the current map of the game master through the regular
/// damage pipeline, so that event kill counters, drops and experience work as usual.
/// Guards, traps and other players are left alone. It's intended for game masters to test
/// mini game events without killing every monster manually.
/// </summary>
[Guid("EFFFEE34-BA59-49F1-A8A3-174CFACEA13C")]
[PlugIn]
[Display(Name = nameof(PlugInResources.KillAllMonstersChatCommandPlugIn_Name), Description = nameof(PlugInResources.KillAllMonstersChatCommandPlugIn_Description), ResourceType = typeof(PlugInResources))]
[ChatCommandHelp(Command, CharacterStatus.GameMaster)]
public class KillAllMonstersChatCommandPlugIn : IChatCommandPlugIn
{
    private const string Command = "/killall";

    /// <inheritdoc />
    public string Key => Command;

    /// <inheritdoc/>
    public CharacterStatus MinCharacterStatusRequirement => CharacterStatus.GameMaster;

    /// <inheritdoc />
    public async ValueTask HandleCommandAsync(Player player, string command)
    {
        if (player.CurrentMap is null)
        {
            return;
        }

        // The range covers the whole map from the current position.
        var monsters = player.CurrentMap.GetAttackablesInRange(player.Position, byte.MaxValue)
            .OfType<Monster>()
            .Where(m => m.IsAlive
                && m.Definition.ObjectKind is not NpcObjectKind.Guard and not NpcObjectKind.Trap)
            .ToList();

        // Best effort: one failing death must not spare the rest, no matter how many there are.
        var killed = 0;
        foreach (var monster in monsters)
        {
            try
            {
                if (!monster.IsAlive)
                {
                    continue;
                }

                await monster.ReflectDamageAsync(player, uint.MaxValue).ConfigureAwait(false);
                killed++;
            }
            catch (Exception ex)
            {
                player.Logger.LogWarning(ex, "Unexpected error killing monster {monster} with /killall.", monster);
            }
        }

        await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.KillAllMonstersFormat), killed).ConfigureAwait(false);
    }
}

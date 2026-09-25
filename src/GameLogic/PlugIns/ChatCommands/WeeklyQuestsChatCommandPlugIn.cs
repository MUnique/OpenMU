// <copyright file="WeeklyQuestsChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;
using MUnique.OpenMU.GameLogic.PlugIns.WeeklyQuests;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which shows the progress of the weekly quests.
/// </summary>
[Guid("8A1D4C7E-2B9F-4E83-B6A5-0C3E7F1D9A24")]
[PlugIn]
[Display(Name = nameof(PlugInResources.WeeklyQuestsChatCommandPlugIn_Name), Description = nameof(PlugInResources.WeeklyQuestsChatCommandPlugIn_Description), ResourceType = typeof(PlugInResources))]
[ChatCommandHelp(Command, typeof(EmptyChatCommandArgs), CharacterStatus.Normal)]
public class WeeklyQuestsChatCommandPlugIn : ChatCommandPlugInBase<EmptyChatCommandArgs>
{
    private const string Command = "/weekly";

    /// <inheritdoc />
    public override string Key => Command;

    /// <inheritdoc />
    public override CharacterStatus MinCharacterStatusRequirement => CharacterStatus.Normal;

    /// <inheritdoc />
    protected override async ValueTask DoHandleCommandAsync(Player player, EmptyChatCommandArgs arguments)
    {
        var plugIn = WeeklyQuestsPlugIn.GetTrackingPlugIn(player);
        if (plugIn is null || !player.GameContext.PlugInManager.IsPlugInActive(typeof(WeeklyQuestsPlugIn).GUID))
        {
            await player.ShowBlueMessageAsync("Las quests semanales no están disponibles.").ConfigureAwait(false);
            return;
        }

        if (await plugIn.GetOverviewAsync(player).ConfigureAwait(false) is not { } overview)
        {
            await player.ShowBlueMessageAsync("No se pudo cargar tu progreso semanal. Probá de nuevo en un rato.").ConfigureAwait(false);
            return;
        }

        if (overview.Entries.Count == 0)
        {
            await player.ShowBlueMessageAsync("No hay quests semanales activas.").ConfigureAwait(false);
            return;
        }

        foreach (var entry in overview.Entries)
        {
            var status = entry switch
            {
                { IsRewarded: true } => "[OK]",
                { IsCompleted: true } => "[Premio pendiente]",
                _ => $"{entry.Count}/{entry.Quest.RequiredCount}",
            };
            await player.ShowBlueMessageAsync($"{entry.Quest.Name}: {status} - {entry.Quest.Description}").ConfigureAwait(false);
        }

        var remaining = overview.NextResetUtc - DateTime.UtcNow;
        await player.ShowBlueMessageAsync($"Reinicio en {(int)remaining.TotalDays}d {remaining.Hours}h {remaining.Minutes}m.").ConfigureAwait(false);
    }
}

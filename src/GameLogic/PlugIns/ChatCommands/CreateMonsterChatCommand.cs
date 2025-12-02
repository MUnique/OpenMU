// <copyright file="CreateMonsterChatCommand.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command to create a new monster which can be remote controlled.
/// </summary>
[Guid("BF4DA282-8CFE-4110-B1C5-A01D3F224FAB")]
[PlugIn("Create monster chat command", "Handles the chat command '/createmonster <number> <intelligent>'. Creates a monster which can be remote controlled by the GM.")]
[ChatCommandHelp(Command, "Creates a monster which can be remote controlled by the game master.", typeof(CreateMonsterChatCommandArgs), CharacterStatus.GameMaster)]
internal class CreateMonsterChatCommand : ChatCommandPlugInBase<CreateMonsterChatCommandArgs>
{
    private const string Command = "/createmonster";

    /// <inheritdoc />
    public override string Key => Command;

    /// <inheritdoc/>
    public override CharacterStatus MinCharacterStatusRequirement => CharacterStatus.GameMaster;

    /// <inheritdoc />
    protected override async ValueTask DoHandleCommandAsync(Player gameMaster, CreateMonsterChatCommandArgs arguments)
    {
        var monsterDef = gameMaster.GameContext.Configuration.Monsters.FirstOrDefault(m => m.Number == arguments.MonsterNumber);
        if (monsterDef is null)
        {
            await this.ShowMessageToAsync(gameMaster, $"Monster with number {arguments.MonsterNumber} not found.").ConfigureAwait(false);
            return;
        }

        var gameMap = gameMaster.CurrentMap;
        var area = new MonsterSpawnArea
        {
            GameMap = gameMap!.Definition,
            MonsterDefinition = monsterDef,
            SpawnTrigger = SpawnTrigger.Automatic,
            Quantity = 1,
            X1 = (byte)Math.Max(gameMaster.Position.X - 3, byte.MinValue),
            X2 = (byte)Math.Min(gameMaster.Position.X + 3, byte.MaxValue),
            Y1 = (byte)Math.Max(gameMaster.Position.Y - 3, byte.MinValue),
            Y2 = (byte)Math.Min(gameMaster.Position.Y + 3, byte.MaxValue),
        };

        INpcIntelligence intelligence = arguments.IsIntelligent ? new BasicMonsterIntelligence() : new NullMonsterIntelligence();
        var monster = new Monster(area, monsterDef, gameMap, NullDropGenerator.Instance, intelligence, gameMaster.GameContext.PlugInManager, gameMaster.GameContext.PathFinderPool);
        intelligence.Npc = monster;

        monster.Initialize();
        await gameMap.AddAsync(monster).ConfigureAwait(false);
        monster.OnSpawn();

        await this.ShowMessageToAsync(gameMaster, $"Monster with number {arguments.MonsterNumber} created, id: {monster.Id}.").ConfigureAwait(false);
    }
}
// <copyright file="RecordingViewPlugInContainer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.TestActors;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The "client" of a <see cref="ScriptedPlayer"/>: instead of sending packets, every view call it
/// implements is written to the actor's <see cref="ActorEventLog"/>.
/// </summary>
/// <remarks>
/// Like the offline player's container it answers <c>null</c> for every view it does not implement -
/// the game logic looks views up null-conditionally throughout - so only the callbacks a scenario
/// asserts on have to be covered. Hits are deliberately NOT recorded here:
/// <see cref="IShowHitPlugIn"/> carries no attacker, so the attribution comes from
/// <see cref="ActorHitRecorderPlugIn"/> instead, and recording both would double every hit.
/// </remarks>
public sealed class RecordingViewPlugInContainer :
    ICustomPlugInContainer<IViewPlugIn>,
    IMapChangePlugIn,
    IRespawnAfterDeathPlugIn,
    IObjectGotKilledPlugIn,
    IUpdateStatsPlugIn,
    IChatViewPlugIn,
    IShowDroppedItemsPlugIn,
    IDroppedItemsDisappearedPlugIn,
    INewPlayersInScopePlugIn,
    INewNpcsInScopePlugIn,
    IObjectsOutOfScopePlugIn,
    IObjectMovedPlugIn,
    IShowMessagePlugIn,
    IShowSkillAnimationPlugIn
{
    /// <summary>
    /// The attributes whose changes are recorded as <c>stat</c> events. The engine pushes an update
    /// for EVERY attribute it recalculates - an idle actor produced ~600 events per minute of shield
    /// recovery bookkeeping alone, which wrapped the event ring within minutes and buried the changes
    /// a scenario actually asserts on. Only the combat-relevant stats are kept.
    /// </summary>
    private static readonly HashSet<AttributeDefinition> RecordedStats =
    [
        Stats.CurrentHealth,
        Stats.MaximumHealth,
        Stats.CurrentShield,
        Stats.MaximumShield,
        Stats.CurrentMana,
        Stats.MaximumMana,
        Stats.CurrentAbility,
        Stats.MaximumAbility,
        Stats.Level,
        Stats.MasterLevel,
    ];

    private readonly ScriptedPlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordingViewPlugInContainer"/> class.
    /// </summary>
    /// <param name="player">The actor whose views are recorded.</param>
    public RecordingViewPlugInContainer(ScriptedPlayer player)
    {
        this._player = player;
    }

    private ActorEventLog Log => this._player.EventLog;

    /// <inheritdoc />
    public T? GetPlugIn<T>()
        where T : class, IViewPlugIn
    {
        // Everything this container implements is served by itself; every other view stays null,
        // exactly as the offline player's container does it.
        return this as T;
    }

    /// <inheritdoc />
    public async ValueTask MapChangeAsync()
    {
        // What OfflineMapChangePlugIn does: without it the character never finishes entering the map.
        await this._player.ClientReadyAfterMapChangeAsync().ConfigureAwait(false);
        this.Log.Append(
            "map",
            new ActorEventField("map", this._player.CurrentMap?.Definition.Name.ToString() ?? string.Empty),
            new ActorEventField("map_number", this._player.CurrentMap?.Definition.Number ?? -1),
            new ActorEventField("x", this._player.Position.X),
            new ActorEventField("y", this._player.Position.Y));
    }

    /// <inheritdoc />
    public ValueTask MapChangeFailedAsync()
    {
        this.Log.Append("error", new ActorEventField("code", "map_change_failed"));
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public ValueTask RespawnAsync()
    {
        // The engine respawns server-side; the actor just notes where it woke up and keeps running.
        this.Log.Append(
            "respawn",
            new ActorEventField("map", this._player.CurrentMap?.Definition.Name.ToString() ?? string.Empty),
            new ActorEventField("x", this._player.Position.X),
            new ActorEventField("y", this._player.Position.Y));
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public ValueTask ObjectGotKilledAsync(IAttackable killedObject, IAttacker? killerObject)
    {
        this.Log.Append(
            "killed",
            new ActorEventField("victim_id", ActorObjects.GetId(killedObject)),
            new ActorEventField("victim", ActorObjects.GetName(killedObject)),
            new ActorEventField("victim_kind", ActorObjects.GetKind(killedObject)),
            new ActorEventField("killer_id", ActorObjects.GetId(killerObject)),
            new ActorEventField("killer", ActorObjects.GetName(killerObject)),
            new ActorEventField("killer_kind", ActorObjects.GetKind(killerObject)));
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public ValueTask UpdateStatsAsync(AttributeDefinition attribute, float value)
    {
        if (!RecordedStats.Contains(attribute))
        {
            return ValueTask.CompletedTask;
        }

        this.Log.Append(
            "stat",
            new ActorEventField("attribute", attribute.Designation ?? string.Empty),
            new ActorEventField("value", value));
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public ValueTask ChatMessageAsync(string message, string sender, ChatMessageType type)
    {
        this.Log.Append(
            "chat",
            new ActorEventField("sender", sender),
            new ActorEventField("message", message),
            new ActorEventField("chat_type", type.ToString()));
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public ValueTask ShowDroppedItemsAsync(IEnumerable<DroppedItem> droppedItems, bool freshDrops)
    {
        foreach (var droppedItem in droppedItems)
        {
            this.Log.Append(
                "drop",
                new ActorEventField("id", droppedItem.Id),
                new ActorEventField("name", ActorObjects.GetName(droppedItem)),
                new ActorEventField("x", droppedItem.Position.X),
                new ActorEventField("y", droppedItem.Position.Y),
                new ActorEventField("fresh", freshDrops));
        }

        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public ValueTask DroppedItemsDisappearedAsync(IEnumerable<ushort> disappearedItemIds)
    {
        foreach (var itemId in disappearedItemIds)
        {
            this.Log.Append("drop_gone", new ActorEventField("id", itemId));
        }

        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public ValueTask NewPlayersInScopeAsync(IEnumerable<Player> newObjects, bool isSpawned = true)
    {
        foreach (var newObject in newObjects)
        {
            this.AppendInView(newObject, isSpawned);
        }

        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public ValueTask NewNpcsInScopeAsync(IEnumerable<NonPlayerCharacter> newObjects, bool isSpawned = true)
    {
        foreach (var newObject in newObjects)
        {
            this.AppendInView(newObject, isSpawned);
        }

        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public ValueTask ObjectsOutOfScopeAsync(IEnumerable<IIdentifiable> objects)
    {
        foreach (var goneObject in objects)
        {
            this.Log.Append("out_of_view", new ActorEventField("id", goneObject.Id));
        }

        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public ValueTask ObjectMovedAsync(ILocateable movedObject, MoveType moveType)
    {
        this.Log.Append(
            "moved",
            new ActorEventField("id", ActorObjects.GetId(movedObject)),
            new ActorEventField("name", ActorObjects.GetName(movedObject)),
            new ActorEventField("kind", ActorObjects.GetKind(movedObject)),
            new ActorEventField("x", movedObject.Position.X),
            new ActorEventField("y", movedObject.Position.Y),
            new ActorEventField("move_type", moveType.ToString()));
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public ValueTask ShowMessageAsync(string message, MessageType messageType)
    {
        this.Log.Append(
            "message",
            new ActorEventField("message", message),
            new ActorEventField("message_type", messageType.ToString()));
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public ValueTask ShowSkillAnimationAsync(IAttacker attacker, IAttackable? target, Skill skill, bool effectApplied)
    {
        return this.AppendSkillAsync(attacker, target, (short)skill.Number, effectApplied);
    }

    /// <inheritdoc />
    public ValueTask ShowSkillAnimationAsync(IAttacker attacker, IAttackable? target, short skillNumber, bool effectApplied)
    {
        return this.AppendSkillAsync(attacker, target, skillNumber, effectApplied);
    }

    /// <inheritdoc />
    public ValueTask ShowComboAnimationAsync(IAttacker attacker, IAttackable? target)
    {
        this.Log.Append(
            "combo",
            new ActorEventField("attacker", ActorObjects.GetName(attacker)),
            new ActorEventField("target", ActorObjects.GetName(target)));
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public ValueTask ShowNovaStartAsync(IAttacker attacker)
    {
        this.Log.Append("nova_start", new ActorEventField("attacker", ActorObjects.GetName(attacker)));
        return ValueTask.CompletedTask;
    }

    private ValueTask AppendSkillAsync(IAttacker attacker, IAttackable? target, short skillNumber, bool effectApplied)
    {
        this.Log.Append(
            "skill",
            new ActorEventField("skill", skillNumber),
            new ActorEventField("attacker_id", ActorObjects.GetId(attacker)),
            new ActorEventField("attacker", ActorObjects.GetName(attacker)),
            new ActorEventField("target_id", ActorObjects.GetId(target)),
            new ActorEventField("target", ActorObjects.GetName(target)),
            new ActorEventField("applied", effectApplied));
        return ValueTask.CompletedTask;
    }

    private void AppendInView(ILocateable inViewObject, bool isSpawned)
    {
        this.Log.Append(
            "in_view",
            new ActorEventField("id", ActorObjects.GetId(inViewObject)),
            new ActorEventField("name", ActorObjects.GetName(inViewObject)),
            new ActorEventField("kind", ActorObjects.GetKind(inViewObject)),
            new ActorEventField("x", inViewObject.Position.X),
            new ActorEventField("y", inViewObject.Position.Y),
            new ActorEventField("alive", inViewObject is IAttackable attackable ? attackable.IsAlive : (object?)null),
            new ActorEventField("spawned", isSpawned));
    }
}

// <copyright file="TargetedSkillHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Skills;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Reprersents the targeted skill packet handler.
/// </summary>
internal partial class TargetedSkillHandlerPlugIn
{
    private readonly Dictionary<int, ITargetedSkillAction> _handlers = [];
    private readonly ITargetedSkillAction _defaultHandler = new TargetedSkillActionDefault();

    /// <summary>
    /// Initializes a new instance of the <see cref="TargetedSkillHandlerPlugIn"/> class.
    /// </summary>
    public TargetedSkillHandlerPlugIn()
    {
        // Load all plugins in the current assembly
        var pluginType = typeof(ITargetedSkillAction);
        var pluginInstances = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => pluginType.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
            .Select(t => (ITargetedSkillAction)Activator.CreateInstance(t)!)
            .ToList();

        foreach (var plugin in pluginInstances)
        {
            this._handlers[plugin.Key] = plugin;
        }
    }
}

/// <summary>
/// Implements the targeted skill packet handler.
/// </summary>
[PlugIn("TargetedSkillHandlerPlugIn", "Handler for targeted skill packets.")]
[Guid("5b07d03c-509c-4aec-972c-a99db77561f2")]
[MinimumClient(3, 0, ClientLanguage.Invariant)]
internal partial class TargetedSkillHandlerPlugIn : IPacketHandlerPlugIn
{
    /// <inheritdoc/>
    public virtual bool IsEncryptionExpected => true;

    /// <inheritdoc/>
    public byte Key => TargetedSkill.Code;

    /// <inheritdoc/>
    public virtual async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        TargetedSkill message = packet;

        await this.HandleAsync(player, message.SkillId, message.TargetId).ConfigureAwait(false);
    }

    /// <summary>
    /// Handles the skill request of the specified player.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="skillId">The skill identifier.</param>
    /// <param name="targetId">The target identifier.</param>
    protected async ValueTask HandleAsync(Player player, ushort skillId, ushort targetId)
    {
        if (player.SkillList is null || !player.SkillList.ContainsSkill(skillId))
        {
            return;
        }

        this._handlers.TryGetValue(skillId, out var plugin);

        if (plugin == null)
        {
            plugin = this._defaultHandler;
        }

        // Note: The target can be the own player too, for example when using buff skills.
        if (player.GetObject(targetId) is IAttackable target)
        {
            await plugin.PerformSkillAsync(player, target, skillId).ConfigureAwait(false);
        }
    }
}
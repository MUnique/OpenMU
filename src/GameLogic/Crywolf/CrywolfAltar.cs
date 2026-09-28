// <copyright file="CrywolfAltar.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Crywolf;

using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;

/// <summary>
/// An altar of the crywolf event, which an elf contracts to protect the statue of the holy wolf.
/// </summary>
/// <remarks>
/// An elf has to stand at the altar and stay there until the contract is valid. When the elf moves,
/// dies or leaves, the contract is cancelled. Each altar can be contracted only a few times per event,
/// and the attempts count as well, like in the original game.
/// </remarks>
public sealed class CrywolfAltar
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CrywolfAltar"/> class.
    /// </summary>
    /// <param name="index">The index of the altar, from 0 to 4.</param>
    /// <param name="npc">The NPC of the altar.</param>
    public CrywolfAltar(int index, NonPlayerCharacter npc)
    {
        this.Index = index;
        this.Npc = npc;
        this.Display = new CrywolfEffectDisplay(npc);
    }

    /// <summary>
    /// Gets the index of the altar, from 0 to 4.
    /// </summary>
    public int Index { get; }

    /// <summary>
    /// Gets the NPC of the altar.
    /// </summary>
    public NonPlayerCharacter Npc { get; }

    /// <summary>
    /// Gets the state of the altar.
    /// </summary>
    public CrywolfAltarState State { get; private set; }

    /// <summary>
    /// Gets the number of contracts of this event, including the attempts.
    /// </summary>
    public int ContractCount { get; private set; }

    /// <summary>
    /// Gets the elf which contracted the altar or tries to contract it.
    /// </summary>
    public Player? Contractor { get; private set; }

    /// <summary>
    /// Gets the display of the effect of the altar.
    /// </summary>
    internal CrywolfEffectDisplay Display { get; }

    private DateTime ValidFrom { get; set; }

    private DateTime LastChange { get; set; } = DateTime.MinValue;

    /// <summary>
    /// Gets the state of the altar as it's expected by the client: the altar state in the high nibble,
    /// and the number of remaining contracts in the low nibble.
    /// </summary>
    /// <param name="contractsPerAltar">The number of contracts per altar.</param>
    /// <returns>The state for the client.</returns>
    public byte GetClientState(int contractsPerAltar)
    {
        var remainingContracts = Math.Clamp(contractsPerAltar - this.ContractCount, 0, 0x0F);
        return (byte)(((byte)this.State << 4) | remainingContracts);
    }

    /// <summary>
    /// Resets the altar for a new event.
    /// </summary>
    public void Reset()
    {
        this.State = CrywolfAltarState.Free;
        this.ContractCount = 0;
        this.Contractor = null;
        this.LastChange = DateTime.MinValue;
        this.UpdateEffect(true);
    }

    /// <summary>
    /// Hides the effect of the altar, e.g. when the event is finished.
    /// </summary>
    public void Hide()
    {
        this.UpdateEffect(false);
    }

    /// <summary>
    /// Tries to start a contract with an elf.
    /// </summary>
    /// <param name="player">The player of the elf.</param>
    /// <param name="definition">The definition of the event.</param>
    /// <param name="now">The current time.</param>
    /// <returns>The result.</returns>
    public CrywolfContractResult TryStartContract(Player player, CrywolfEventDefinition definition, DateTime now)
    {
        if (this.State != CrywolfAltarState.Free)
        {
            return CrywolfContractResult.NotAvailable;
        }

        if (player.SelectedCharacter?.CharacterClass is not { } characterClass
            || !definition.ContractCharacterClassNumbers.Contains(characterClass.Number)
            || (player.Attributes?[Stats.Level] ?? 0) < definition.MinimumContractLevel)
        {
            return CrywolfContractResult.NotQualified;
        }

        if (now - this.LastChange < definition.AltarCooldown)
        {
            return CrywolfContractResult.Cooldown;
        }

        if (player.CurrentMap != this.Npc.CurrentMap || player.Position != this.Npc.Position)
        {
            return CrywolfContractResult.WrongPosition;
        }

        // Like in the original game, the attempt already counts as contract.
        this.ContractCount++;
        this.State = CrywolfAltarState.Attempting;
        this.Contractor = player;
        this.ValidFrom = now + definition.ContractDelay;
        this.LastChange = now;
        this.UpdateEffect(true);
        return CrywolfContractResult.Success;
    }

    /// <summary>
    /// Checks the contractor of the altar: the contract gets valid when the elf stayed at the altar long enough,
    /// and it's cancelled when the elf moved, died or left.
    /// </summary>
    /// <param name="definition">The definition of the event.</param>
    /// <param name="now">The current time.</param>
    /// <returns>The change of the contract.</returns>
    public CrywolfContractChange Update(CrywolfEventDefinition definition, DateTime now)
    {
        if (this.Contractor is not { } contractor)
        {
            return CrywolfContractChange.None;
        }

        if (!IsAtAltar(contractor, this.Npc))
        {
            this.Cancel(definition, now);
            return CrywolfContractChange.Cancelled;
        }

        if (this.State == CrywolfAltarState.Attempting && now >= this.ValidFrom)
        {
            this.State = CrywolfAltarState.Contracted;
            this.UpdateEffect(true);
            return CrywolfContractChange.Validated;
        }

        return CrywolfContractChange.None;
    }

    /// <summary>
    /// Cancels the contract of the altar.
    /// </summary>
    /// <param name="definition">The definition of the event.</param>
    /// <param name="now">The current time.</param>
    public void Cancel(CrywolfEventDefinition definition, DateTime now)
    {
        this.Contractor = null;
        this.LastChange = now;
        this.State = this.ContractCount >= definition.ContractsPerAltar ? CrywolfAltarState.Exhausted : CrywolfAltarState.Free;
        this.UpdateEffect(true);
    }

    private static bool IsAtAltar(Player player, NonPlayerCharacter altar)
    {
        return player.IsAlive
               && player.PlayerState.CurrentState == PlayerState.EnteredWorld
               && player.CurrentMap == altar.CurrentMap
               && player.Position == altar.Position;
    }

    private void UpdateEffect(bool isVisible)
    {
        this.Display.Effect = !isVisible
            ? null
            : this.State switch
            {
                CrywolfAltarState.Free => CrywolfEffect.AltarEnabled,
                CrywolfAltarState.Contracted => CrywolfEffect.AltarContracted,
                CrywolfAltarState.Attempting => CrywolfEffect.AltarAttempt,
                _ => CrywolfEffect.AltarDisabled,
            };
    }
}

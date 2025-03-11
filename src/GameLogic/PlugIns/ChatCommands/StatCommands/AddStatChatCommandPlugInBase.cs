// <copyright file="AddStatChatCommandPlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.PlayerActions.Character;

/// <summary>
/// A chat command plugin which handles the command to add stat points.
/// </summary>
public abstract class AddStatChatCommandPlugInBase : IChatCommandPlugIn
{
    private readonly IncreaseStatsAction _action = new();

    /// <inheritdoc />
    public abstract string Key { get; }

    /// <inheritdoc />
    public abstract CharacterStatus MinCharacterStatusRequirement { get; }

    /// <inheritdoc />
    public async ValueTask HandleCommandAsync(Player player, string command)
    {
        try
        {
            if (player.SelectedCharacter is not { } selectedCharacter)
            {
                return;
            }

            var (stat, amount) = this.GetStatAndAmount(command);
            if (selectedCharacter!.Attributes.All(sa => sa.Definition != stat))
            {
                throw new ArgumentException($"The character has no stat attribute '{stat.Designation}'.");
            }

            if (!selectedCharacter.CanIncreaseStats(amount))
            {
                return;
            }

            if (player.CurrentMiniGame is not null)
            {
                await player.ShowMessageAsync("Adding multiple points is not allowed when playing a mini game.").ConfigureAwait(false);
                return;
            }

            await this._action.IncreaseStatsAsync(player, stat, amount).ConfigureAwait(false);
        }
        catch (ArgumentException e)
        {
            await player.ShowMessageAsync(e.Message).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// A chat command plugin which handles the command to add stat points.
    /// </summary>
    /// <param name="command">The command passed.</param>
    protected abstract (AttributeDefinition Stat, ushort Amount) GetStatAndAmount(string command);

    /// <summary>
    /// A chat command plugin which handles the command to add stat points.
    /// </summary>
    public abstract class AddSingleStatChatCommandPlugInBase : AddStatChatCommandPlugInBase
    {
        /// <summary>
        /// Gets the stat which should be added.
        /// </summary>
        protected abstract AttributeDefinition TheStat { get; }

        /// <inheritdoc />
        protected override (AttributeDefinition Stat, ushort Amount) GetStatAndAmount(string command)
        {
            var arguments = command.ParseArguments<Arguments>();
            return (Stat: this.TheStat, Amount: arguments.Amount);
        }

        /// <summary>
        /// The arguments for the chat command.
        /// </summary>
        protected class Arguments : ArgumentsBase
        {
            /// <summary>
            /// Gets or sets the amount.
            /// </summary>
            public ushort Amount { get; set; }
        }
    }
}

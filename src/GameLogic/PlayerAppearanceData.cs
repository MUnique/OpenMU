// <copyright file="PlayerAppearanceData.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

/// <summary>
/// The appearance data of a <see cref="Player"/>, which is based on the
/// currently selected character and its equipped items.
/// </summary>
internal sealed class PlayerAppearanceData : IAppearanceData
{
    private readonly Player _player;
    private bool? _fullAncientSetEquipped;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerAppearanceData"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public PlayerAppearanceData(Player player)
    {
        this._player = player;
    }

    /// <inheritdoc />
    public event EventHandler? AppearanceChanged;

    /// <inheritdoc />
    public CharacterClass? CharacterClass => this._player.SelectedCharacter?.CharacterClass;

    /// <inheritdoc />
    public CharacterStatus CharacterStatus => this._player.SelectedCharacter?.CharacterStatus ?? default;

    /// <inheritdoc />
    public CharacterPose Pose => this._player.SelectedCharacter?.Pose ?? default;

    /// <inheritdoc />
    public bool FullAncientSetEquipped => (this._fullAncientSetEquipped ??= this._player.SelectedCharacter?.HasFullAncientSetEquipped()) ?? false;

    /// <inheritdoc />
    public IEnumerable<ItemAppearance> EquippedItems
    {
        get
        {
            if (this._player.Inventory != null)
            {
                return this._player.Inventory.EquippedItems.Select(item => item.GetAppearance());
            }

            return Enumerable.Empty<ItemAppearance>();
        }
    }

    /// <summary>
    /// Raises the <see cref="AppearanceChanged"/> event.
    /// </summary>
    public void RaiseAppearanceChanged()
    {
        this._fullAncientSetEquipped = null;
        this.AppearanceChanged?.Invoke(this, EventArgs.Empty);
    }
}

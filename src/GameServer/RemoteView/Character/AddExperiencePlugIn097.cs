// <copyright file="AddExperiencePlugIn097.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character;

using MUnique.OpenMU.GameServer.Compatibility;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Experience update plugin for 0.97 clients.
/// </summary>
[PlugIn(nameof(AddExperiencePlugIn097), "Experience update plugin for 0.97 clients.")]
[Guid("742A2D34-3B1C-4A8C-88AB-9F4F9D5F6B58")]
[MinimumClient(0, 97, ClientLanguage.Invariant)]
[MaximumClient(0, 97, ClientLanguage.Invariant)]
public class AddExperiencePlugIn097 : IAddExperiencePlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="AddExperiencePlugIn097"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public AddExperiencePlugIn097(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask AddExperienceAsync(int exp, IAttackable? obj, ExperienceType experienceType)
    {
        await Version097CompatibilityProfile.SendExperienceAsync(this._player, exp, obj, experienceType).ConfigureAwait(false);
    }
}

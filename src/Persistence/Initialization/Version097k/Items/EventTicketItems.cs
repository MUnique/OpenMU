// <copyright file="EventTicketItems.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version097k.Items;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Initializer for the event ticket items of version 0.97k.
/// </summary>
/// <remarks>
/// Compared to version 0.95d, it adds the items of Blood Castle with its six levels.
/// </remarks>
internal class EventTicketItems : Version095d.Items.EventTicketItems
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EventTicketItems"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public EventTicketItems(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    public override void Initialize()
    {
        base.Initialize();

        // Blood Castle:
        this.CreateEventItem(16, 13, 1, 2, LocalizedString.FromResource(() => ItemNames.ScrollOfArchangel), 6, false, 2, 32, 45, 57, 68, 76);
        this.CreateEventItem(17, 13, 1, 2, LocalizedString.FromResource(() => ItemNames.BloodBone), 6, false, 2, 32, 45, 57, 68, 76);
        this.CreateEventItem(18, 13, 2, 2, LocalizedString.FromResource(() => ItemNames.InvisibilityCloak), 6, false);
        this.CreateEventItem(19, 13, 1, 2, LocalizedString.FromResource(() => ItemNames.WeaponOfArchangel), 0, false);
    }
}

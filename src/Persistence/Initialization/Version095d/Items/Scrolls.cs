// <copyright file="Scrolls.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version095d.Items;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Initializer for scroll items which allow a character to learn <see cref="Skill"/>s.
/// </summary>
/// <seealso cref="MUnique.OpenMU.Persistence.Initialization.InitializerBase" />
public class Scrolls : Version075.Items.Scrolls
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Scrolls"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public Scrolls(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <summary>
    /// Initializes the scroll data.
    /// </summary>
    public override void Initialize()
    {
        base.Initialize();
        this.CreateScroll(12, 13, "Scroll of Cometfall", 80, 0, 436, 175000);
        this.CreateScroll(13, 14, "Scroll of Inferno", 88, 0, 578, 265000);
    }
}
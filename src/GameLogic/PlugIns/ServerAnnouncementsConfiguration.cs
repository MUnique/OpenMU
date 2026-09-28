// <copyright file="ServerAnnouncementsConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using MUnique.OpenMU.Interfaces;

/// <summary>
/// One message of the rotation of <see cref="ServerAnnouncementsPlugIn"/>.
/// </summary>
public class AnnouncementMessage
{
    /// <summary>
    /// Gets or sets the text of the message.
    /// </summary>
    [Required]
    [Display(Name = "Text", Description = "Texto del mensaje. Se muestra tal cual, sin traducciones automáticas.")]
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets how the message is shown: the same styles used by the /post, /goldnotice and /slidenotice commands.
    /// </summary>
    [Display(Name = "Type", Description = "Cómo se ve: BlueNormal (como /post), GoldenCenter (como /goldnotice, en el centro), SlideNotice (se desliza, como /slidenotice) o GuildNotice.")]
    public MessageType Type { get; set; } = MessageType.BlueNormal;

    /// <inheritdoc />
    public override string ToString() => this.Text;

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is AnnouncementMessage other && this.Text == other.Text && this.Type == other.Type;

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(this.Text, this.Type);
}

/// <summary>
/// Configuration of <see cref="ServerAnnouncementsPlugIn"/>.
/// </summary>
public class ServerAnnouncementsConfiguration
{
    /// <summary>
    /// Gets the default configuration: two example messages, one every 15 minutes.
    /// </summary>
    public static ServerAnnouncementsConfiguration Default => new()
    {
        IntervalMinutes = 15,
        Messages =
        [
            new AnnouncementMessage { Text = "Recordá: nadie del staff te va a pedir tu contraseña.", Type = MessageType.GoldenCenter },
            new AnnouncementMessage { Text = "Unite a nuestro Discord para enterarte de todo.", Type = MessageType.BlueNormal },
        ],
    };

    /// <summary>
    /// Gets or sets the interval, in minutes, between one announcement and the next.
    /// </summary>
    [Range(1, 1440)]
    [Display(Name = "Interval (Minutes)", Description = "Cada cuántos minutos aparece un mensaje. Como se muestra uno solo por vez y se espera este intervalo completo antes del siguiente, los mensajes nunca se pisan entre sí.", Order = 1)]
    public int IntervalMinutes { get; set; } = 15;

    /// <summary>
    /// Gets or sets the messages, shown one after another in order, looping back to the first when the list ends.
    /// </summary>
    [Display(Name = "Messages", Description = "Los mensajes, en el orden en que se muestran. Al llegar al último, se vuelve a empezar por el primero. Con un solo mensaje, ese se repite siempre.", Order = 2)]
    public IList<AnnouncementMessage> Messages { get; set; } = [];
}

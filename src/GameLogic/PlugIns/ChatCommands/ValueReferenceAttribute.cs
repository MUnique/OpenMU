// <copyright file="ValueReferenceAttribute.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

/// <summary>
/// Describes which kind of game object an argument property of a chat command refers to.
/// </summary>
/// <remarks>
/// It's a hint for user interfaces only; it doesn't change how the argument is parsed or validated.
/// </remarks>
[AttributeUsage(AttributeTargets.Property)]
public sealed class ValueReferenceAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValueReferenceAttribute"/> class.
    /// </summary>
    /// <param name="reference">The kind of game object the argument refers to.</param>
    public ValueReferenceAttribute(ChatCommandValueReference reference)
    {
        this.Reference = reference;
    }

    /// <summary>
    /// Gets the kind of game object the argument refers to.
    /// </summary>
    public ChatCommandValueReference Reference { get; }

    /// <summary>
    /// Gets or sets the name of another property which identifies the game object together with this one,
    /// e.g. the group of an item, for its number.
    /// </summary>
    public string? GroupWith { get; set; }
}

// <copyright file="ChatCommandHelpAttribute.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

/// <summary>
/// An attribute which decorates an <see cref="IChatCommandPlugIn"/> with help information.
/// This information is then used in the <see cref="HelpCommand"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class ChatCommandHelpAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatCommandHelpAttribute" /> class.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="description">The description of the command.</param>
    public ChatCommandHelpAttribute(string command, string description)
        : this(command, description, CharacterStatus.Normal)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatCommandHelpAttribute" /> class.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="description">The description of the command.</param>
    /// <param name="minimumCharacterStatus">The minimum character status.</param>
    public ChatCommandHelpAttribute(string command, string description, CharacterStatus minimumCharacterStatus)
        : this(command, description, null, minimumCharacterStatus)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatCommandHelpAttribute" /> class.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="description">The description of the command.</param>
    /// <param name="argumentsType">Type of the arguments.</param>
    public ChatCommandHelpAttribute(string command, string description, Type? argumentsType)
        : this(command, description, argumentsType, CharacterStatus.Normal)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatCommandHelpAttribute" /> class.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="description">The description of the command.</param>
    /// <param name="argumentsType">Type of the arguments.</param>
    /// <param name="minimumCharacterStatus">The minimum character status.</param>
    public ChatCommandHelpAttribute(string command, string description, Type? argumentsType, CharacterStatus minimumCharacterStatus)
    {
        this.Command = command;
        this.Description = description;
        this.ArgumentsType = argumentsType;
        this.MinimumCharacterStatus = minimumCharacterStatus;
    }

    /// <summary>
    /// Gets the command.
    /// </summary>
    public string Command { get; }

    /// <summary>
    /// Gets the description of the command.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Gets the minimum character status.
    /// </summary>
    public CharacterStatus MinimumCharacterStatus { get; }

    /// <summary>
    /// Gets the type of the arguments of the chat command.
    /// </summary>
    public Type? ArgumentsType { get; }

    /// <summary>
    /// Gets the usage text for the chat command.
    /// </summary>
    public string Usage => this.ArgumentsType is null ? this.Command : CommandExtensions.CreateUsage(this.ArgumentsType, this.Command);
}
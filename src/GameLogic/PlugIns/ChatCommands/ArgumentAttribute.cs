// <copyright file="ArgumentAttribute.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands
{
    using System;

    /// <summary>
    /// Attribute used in the arguments of the commands.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ArgumentAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentAttribute"/> class
        /// which is a required argument.
        /// </summary>
        /// <param name="shortName">The short name.</param>
        public ArgumentAttribute(string shortName)
            : this(shortName, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentAttribute" /> class.
        /// </summary>
        /// <param name="shortName">The short name.</param>
        /// <param name="isRequired">If set to <c>true</c>, this argument is required to execute the command.</param>
        public ArgumentAttribute(string shortName, bool isRequired)
        {
            this.ShortName = shortName;
            this.IsRequired = isRequired;
        }

        /// <summary>
        /// Gets or sets the short name of the argument to be used in chat.
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the argument is required to execute the command.
        /// </summary>
        public bool IsRequired { get; set; }
    }
}
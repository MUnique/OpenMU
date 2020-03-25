namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands
{
    using System;

    /// <summary>
    /// Attribute that arguments use
    /// </summary>
    public class CommandsAttributes
    {
        /// <summary>
        /// Attribute used in the arguments of the commands
        /// </summary>
        [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
        public class Argument : Attribute
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Argument"/> class.
            /// Attribute cctor.
            /// </summary>
            /// <param name="shortName">Name.</param>
            /// <param name="required">Required.</param>
            public Argument(string shortName, bool required = true)
            {
                this.ShortName = shortName;
                this.Required = required;
            }

            /// <summary>
            /// Gets or sets the short name of the argument to be used in chat.
            /// </summary>
            public string ShortName { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether required
            /// </summary>
            public bool Required { get; set; }
        }
    }
}
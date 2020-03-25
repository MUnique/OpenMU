namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands
{
    using System;
    using System.Linq;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// Extensions to make the process of creating more commands easier.
    /// </summary>
    public static class CommandExtensions
    {
        /// <summary>
        /// Parse the arguments of a command string.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <typeparam name="T">The type.</typeparam>
        /// <returns>Returns the initialized type.</returns>
        public static T ParseArguments<T>(this string command)
        {
            var arguments = command.Split(' ').Where(x => !x.Contains("/")).ToList();
            var instance = (T)Activator.CreateInstance(typeof(T));
            var properties = instance.GetType().GetProperties();

            if (arguments.Count() != properties.Count())
            {
                throw new ArgumentException($"The command needs {properties.Length} arguments and was given {arguments.Count}.");
            }

            for (var i = 0; i < properties.Count(); i++)
            {
                var property = properties[i];
                var argument = arguments[i];

                try
                {
                    property.SetValue(instance, Convert.ChangeType(argument, property.PropertyType));
                }
                catch (Exception e)
                {
                    throw new ArgumentException($"The {i}th argument with the value of ({argument}) is not a valid type, it needs to be a valid type of {property.PropertyType.Name}.");
                }
            }

            return instance;
        }

        /// <summary>
        /// Easier way to show a message to a player.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="message">The message.</param>
        /// <param name="type">Message type.</param>
        public static void ShowMessage(this Player player, string message, MessageType type = MessageType.BlueNormal)
        {
            player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage(message, type);
        }
    }
}
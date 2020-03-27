namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Persistence;

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
            var instance = (T)Activator.CreateInstance(typeof(T));
            var properties = instance.GetType().GetProperties().ToList();
            var arguments = command.Split(' ').Where(x => !x.Contains("/")).ToList();

            void SetPropertyValue(PropertyInfo propertyInfo, string value)
            {
                try
                {
                    propertyInfo.SetValue(instance, Convert.ChangeType(value, propertyInfo.PropertyType));
                }
                catch (Exception e)
                {
                    throw new ArgumentException($"The argument {propertyInfo.Name} was given a invalid type, it expects the value to be of the type {propertyInfo.PropertyType.Name}.");
                }
            }

            // [Short argument parsing]
            // If the command string contains = it means it is using the short version
            if (command.Contains('='))
            {
                // Gets a list of only the attributes that contains a short name
                var argumentProperties = properties.Where(property => property.GetCustomAttributes<CommandsAttributes.Argument>().Any()).ToList();

                // This could be better imo
                // At the moment it creates a list of a bool and the property name, and check at the end if all the required properties were used
                var requiredProperties = new Dictionary<string, bool>(argumentProperties.Where(prop => prop.GetCustomAttributes<CommandsAttributes.Argument>().Any(argument => argument.Required)).Select(property => new KeyValuePair<string, bool>(property.Name, false)));

                foreach (var property in argumentProperties)
                {
                    // Gets the attribute from the property and properly casts it to the Attribute type.
                    if (!(property.GetCustomAttributes<CommandsAttributes.Argument>().FirstOrDefault() is { } attribute))
                    {
                        continue;
                    }

                    // The correspondent argument
                    var argument = arguments.FirstOrDefault(x => x.Contains(attribute.ShortName));

                    if (argument == null)
                    {
                        continue;
                    }

                    // Cleans the argument from the short name
                    var argumentValue = argument.Replace($"{attribute.ShortName}=", string.Empty);

                    SetPropertyValue(property, argumentValue);
                    requiredProperties[property.Name] = true;
                }

                var requiredPropertiesNotUsed = requiredProperties.Where(x => x.Value == false).ToList();

                // All required properties were used
                if (!requiredPropertiesNotUsed.Any())
                {
                    return instance;
                }

                // One or many required properties were not used
                var stringBuilder = new StringBuilder();
                foreach (var pair in requiredPropertiesNotUsed)
                {
                    stringBuilder.AppendLine($"The required argument named {pair.Key} was not used.");
                }

                throw new ArgumentException(stringBuilder.ToString());
            }

            // [Normal argument parsing]
            if (arguments.Count != properties.Count)
            {
                throw new ArgumentException($"The command needs {properties.Count} arguments and was given {arguments.Count}.");
            }

            for (var i = 0; i < properties.Count(); i++)
            {
                var property = properties[i];
                var argument = arguments[i];

                SetPropertyValue(property, argument);
            }

            return instance;
        }


        /// <summary>
        /// Create the usage string for the command using the argument class.`
        /// </summary>
        /// <typeparam name="T">ArgumentBase.</typeparam>
        /// <param name="commandKey">The command name.</param>
        /// <returns>Returns the usage string.</returns>
        public static string CreateUsage<T>(string commandKey) => CreateUsage(typeof(T), commandKey);

        /// <summary>
        /// Create the usage string for the command using the argument class.`
        /// </summary>
        /// <param name="argumentsType">Type of the arguments.</param>
        /// <param name="commandName">The command name.</param>
        /// <returns>
        /// Returns the usage string.
        /// </returns>
        public static string CreateUsage(Type argumentsType, string commandName)
        {
            var properties = argumentsType.GetProperties();
            var stringBuilder = new StringBuilder();

            stringBuilder.Append($"{commandName} ");

            foreach (var property in properties)
            {
                stringBuilder.Append($"{{{properties.GetName()}|{property.PropertyType.Name}}}");
                if (property != properties.Last())
                {
                    stringBuilder.Append(" ");
                }
            }

            return stringBuilder.ToString();
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
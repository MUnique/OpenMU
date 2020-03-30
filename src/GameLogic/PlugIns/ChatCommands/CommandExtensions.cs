// <copyright file="CommandExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

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

            if (command.Contains('='))
            {
                // [Short argument parsing]
                // If the command string contains = it means it is using the short version
                ReadNamedArguments(instance, properties, arguments);
                return instance;
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

                SetPropertyValue(instance, property, argument);
            }

            return instance;
        }

        /// <summary>
        /// Create the usage string for the command using the argument class.
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
        public static void ShowMessage(this Player player, string message)
        {
            player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage(message, MessageType.BlueNormal);
        }

        private static void ReadNamedArguments(object instance, IList<PropertyInfo> properties, IList<string> arguments)
        {
            var argumentProperties = properties.Where(property => property.GetCustomAttribute<ArgumentAttribute>() is { }).ToList();
            var requiredProperties = argumentProperties.Where(prop => prop.GetCustomAttribute<ArgumentAttribute>() is { } attribute && attribute.IsRequired).ToList();

            foreach (var property in argumentProperties)
            {
                var attribute = property.GetCustomAttributes<ArgumentAttribute>().First();
                var argument = arguments.FirstOrDefault(x => x.Contains(attribute.ShortName));

                if (argument == null)
                {
                    continue;
                }

                // Cleans the argument from the short name
                var argumentValue = argument.Replace($"{attribute.ShortName}=", string.Empty);

                SetPropertyValue(instance, property, argumentValue);
                requiredProperties.Remove(property);
            }

            if (!requiredProperties.Any())
            {
                return;
            }

            // One or many required properties were not used
            var stringBuilder = new StringBuilder();
            foreach (var requiredProperty in requiredProperties)
            {
                stringBuilder.AppendLine($"The required argument named {requiredProperty.Name} was not used.");
            }

            throw new ArgumentException(stringBuilder.ToString());
        }

        private static void SetPropertyValue(object instance, PropertyInfo propertyInfo, string value)
        {
            try
            {
                propertyInfo.SetValue(instance, Convert.ChangeType(value, propertyInfo.PropertyType));
            }
            catch
            {
                throw new ArgumentException($"The argument {propertyInfo.Name} was given a invalid type, it expects the value to be of the type {propertyInfo.PropertyType.Name}.");
            }
        }
    }
}
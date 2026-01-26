// <copyright file="CommandExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Globalization;
using System.Reflection;

/// <summary>
/// Extensions to make the process of creating more commands easier.
/// </summary>
public static class CommandExtensions
{
    /// <summary>
    /// Parse the arguments of a command string.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="player">The player which issued the command.</param>
    /// <typeparam name="T">The type.</typeparam>
    /// <returns>Returns the parsed object, if successful; Otherwise <see langword="null"/>.</returns>
    public static async ValueTask<T?> TryParseArgumentsAsync<T>(this string command, Player? player)
        where T : class, new()
    {
        var instance = new T();
        var properties = typeof(T).GetProperties()
            .Where(property => property.SetMethod is { })
            .ToList();
        var arguments = command.Split(' ').Where(x => !x.Contains("/")).ToList();

        if (command.Contains('='))
        {
            // [Short argument parsing]
            // If the command string contains = it means it is using the short version
            if (await ReadNamedArgumentsAsync(instance, properties, arguments, player).ConfigureAwait(false))
            {
                return instance;
            }

            return null;
        }

        var attributedArguments = properties
            .Select(p => p.GetCustomAttribute<ArgumentAttribute>(inherit: true))
            .Where(a => a is { })
            .Select(a => a!)
            .ToList();
        var requiredArgumentCount = attributedArguments.Any()
            ? attributedArguments.Count(a => a.IsRequired)
            : arguments.Count;

        if (arguments.Count < requiredArgumentCount)
        {
            if (player is not null)
            {
                await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.CommandExtensionsInvalidArgumentCount), requiredArgumentCount, arguments.Count).ConfigureAwait(false);
            }

            return null;
        }

        var success = true;
        for (var i = 0; i < Math.Min(arguments.Count, properties.Count); i++)
        {
            var property = properties[i];
            var argument = arguments[i];

            success = await TrySetPropertyValueAsync(instance, property, argument, player).ConfigureAwait(false) && success;
        }

        return instance;
    }

    /// <summary>
    /// Create the usage string for the command using the argument class.
    /// </summary>
    /// <param name="argumentsType">Type of the arguments.</param>
    /// <param name="commandName">The command name.</param>
    /// <returns>
    /// The usage string.
    /// </returns>
    public static string CreateUsage(Type argumentsType, string commandName)
    {
        var stringBuilder = new StringBuilder();

        stringBuilder.Append($"{commandName} ");
        foreach (var parameter in GetParameters(argumentsType).ToList())
        {
            if (!string.IsNullOrWhiteSpace(parameter.ValidValues))
            {
                stringBuilder.Append($"{{{parameter.Name}:{parameter.ValidValues}}}");
            }
            else if (parameter.Type == nameof(String))
            {
                stringBuilder.Append($"{{{parameter.Name}}}");
            }
            else
            {
                stringBuilder.Append($"{{{parameter.Name}:{parameter.Type}}}");
            }

            stringBuilder.Append(" ");
        }

        stringBuilder.ToString().TrimEnd(' ');

        return stringBuilder.ToString();
    }

    /// <summary>
    /// Gets the parameters for an argument class.
    /// </summary>
    /// <param name="argumentsType">Type of the arguments.</param>
    /// <returns>A list of parameters with name, type, and valid values.</returns>
    public static IEnumerable<(string Name, string Type, string ValidValues)> GetParameters(Type argumentsType)
    {
        var properties = argumentsType.GetProperties().Where(p => p.CanWrite);
        foreach (var property in properties)
        {
            string validValues = string.Empty;

            if (property.GetCustomAttribute<ValidValuesAttribute>() is { } validValuesAttribute)
            {
                validValues = string.Join('|', validValuesAttribute.ValidValues);
            }
            else if (property.PropertyType == typeof(bool))
            {
                validValues = "0|1";
            }
            else if (property.PropertyType == typeof(byte) || property.PropertyType == typeof(ushort) || property.PropertyType == typeof(uint))
            {
                // todo: ranges in ParameterAttribute
                // validValues = "";
            }

            yield return (property.Name, property.PropertyType.Name, validValues);
        }
    }

    private static async ValueTask<bool> ReadNamedArgumentsAsync(object instance, IList<PropertyInfo> properties, IList<string> arguments, Player? player)
    {
        var argumentProperties = properties.Where(property => property.GetCustomAttribute<ArgumentAttribute>() is { }).ToList();
        var requiredProperties = argumentProperties.Where(prop => prop.GetCustomAttribute<ArgumentAttribute>() is { IsRequired: true }).ToList();

        foreach (var property in argumentProperties)
        {
            var attribute = property.GetCustomAttributes<ArgumentAttribute>().First();
            var argument = arguments.FirstOrDefault(x => x.Split('=').First().Trim() == attribute.ShortName);

            if (argument is null)
            {
                continue;
            }

            // Cleans the argument from the short name
            var argumentValue = argument.Replace($"{attribute.ShortName}=", string.Empty);

            if (!await TrySetPropertyValueAsync(instance, property, argumentValue, player).ConfigureAwait(false))
            {
                return false;
            }

            requiredProperties.Remove(property);
        }

        if (!requiredProperties.Any())
        {
            return true;
        }

        if (player is null)
        {
            return false;
        }

        // One or many required properties were not used
        foreach (var requiredProperty in requiredProperties)
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.CommandExtensions_RequiredArgumentMissing), requiredProperty.Name).ConfigureAwait(false);
        }

        return false;
    }

    private static async ValueTask<bool> TrySetPropertyValueAsync(object instance, PropertyInfo propertyInfo, string stringValue, Player? player)
    {
        try
        {
            // Special handling of booleans; we want to allow 0 and 1 as valid values.
            if (propertyInfo.PropertyType == typeof(bool) && int.TryParse(stringValue, out var intBool))
            {
                stringValue = intBool == 1 ? bool.TrueString : bool.FalseString;
            }

            propertyInfo.SetValue(instance, Convert.ChangeType(stringValue, propertyInfo.PropertyType, CultureInfo.InvariantCulture));
            return true;
        }
        catch
        {
            if (player is not null)
            {
                await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.CommandExtensionsArgumentInvalidType), propertyInfo.Name, propertyInfo.PropertyType.Name).ConfigureAwait(false);
            }

            return false;
        }
    }
}
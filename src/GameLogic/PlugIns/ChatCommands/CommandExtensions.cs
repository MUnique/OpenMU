// <copyright file="CommandExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Globalization;

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
        var properties = ChatCommandArguments.GetProperties(typeof(T));
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
            .Select(p => p.Argument)
            .OfType<ArgumentAttribute>()
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
        var properties = ChatCommandArguments.GetProperties(argumentsType);
        var parameters = GetParameterInfos(argumentsType).ToList();
        for (var i = 0; i < parameters.Count; i++)
        {
            var parameter = parameters[i];
            if (parameter.ValidValues.Count > 0)
            {
                stringBuilder.Append($"{{{parameter.Name}:{string.Join('|', parameter.ValidValues)}}}");
            }
            else if (parameter.ValueReference != ChatCommandValueReference.None)
            {
                var reference = parameter.ValueReference.ToString();
                stringBuilder.Append(reference == parameter.Name ? $"{{{parameter.Name}}}" : $"{{{parameter.Name}:{reference}}}");
            }
            else if (properties[i].Range is { } && parameter is { Minimum: { } minimum, Maximum: { } maximum } && maximum < GetRange(properties[i] with { Range = null }).Maximum)
            {
                // Only a range with an upper bound is worth to be shown, e.g. not "1-2147483647".
                stringBuilder.Append($"{{{parameter.Name}:{minimum}-{maximum}}}");
            }
            else if (parameter.TypeName == nameof(String))
            {
                stringBuilder.Append($"{{{parameter.Name}}}");
            }
            else
            {
                stringBuilder.Append($"{{{parameter.Name}:{parameter.TypeName}}}");
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
        return GetParameterInfos(argumentsType)
            .Select(parameter => (Name: parameter.Name, Type: parameter.TypeName, ValidValues: string.Join('|', parameter.ValidValues)));
    }

    /// <summary>
    /// Gets the description of the parameters of an argument class.
    /// </summary>
    /// <param name="argumentsType">Type of the arguments.</param>
    /// <returns>The described parameters, in the order in which they are expected when they are passed without their short names.</returns>
    public static IEnumerable<ChatCommandParameterInfo> GetParameterInfos(Type argumentsType)
    {
        foreach (var property in ChatCommandArguments.GetProperties(argumentsType))
        {
            IReadOnlyList<string> validValues = [];

            if (property.ValidValues is { } validValuesAttribute)
            {
                validValues = validValuesAttribute.ValidValues.ToList();
            }
            else if (property.PropertyType == typeof(bool))
            {
                validValues = ["0", "1"];
            }

            var (minimum, maximum) = GetRange(property);

            // A parameter without an ArgumentAttribute can't be required - the parser
            // only counts the required arguments of the attributed properties.
            var argumentAttribute = property.Argument;

            yield return new ChatCommandParameterInfo(
                property.Name,
                argumentAttribute?.ShortName,
                property.PropertyType.Name,
                argumentAttribute?.IsRequired ?? false,
                validValues,
                minimum,
                maximum,
                property.ValueReference?.Kind ?? ChatCommandValueReference.None,
                property.ValueReference?.GroupWith);
        }
    }

    /// <summary>
    /// Gets the range of accepted values of a numeric property. It's the range of its type,
    /// narrowed down by its <see cref="RangeAttribute"/>, if it has one.
    /// </summary>
    /// <param name="property">The property.</param>
    /// <returns>The range; or <see langword="null"/> values, if the property isn't numeric.</returns>
    private static (long? Minimum, long? Maximum) GetRange(ChatCommandArgumentProperty property)
    {
        (long Minimum, long Maximum)? typeRange = Type.GetTypeCode(property.PropertyType) switch
        {
            TypeCode.Byte => (byte.MinValue, byte.MaxValue),
            TypeCode.SByte => (sbyte.MinValue, sbyte.MaxValue),
            TypeCode.Int16 => (short.MinValue, short.MaxValue),
            TypeCode.UInt16 => (ushort.MinValue, ushort.MaxValue),
            TypeCode.Int32 => (int.MinValue, int.MaxValue),
            TypeCode.UInt32 => (uint.MinValue, uint.MaxValue),
            TypeCode.Int64 => (long.MinValue, long.MaxValue),
            TypeCode.UInt64 => (0, long.MaxValue),
            _ => null,
        };

        if (typeRange is not { } validRange)
        {
            return (null, null);
        }

        var (minimum, maximum) = validRange;
        if (property.Range is { } range)
        {
            if (TryConvert(range.Minimum, Math.Ceiling) is { } rangeMinimum)
            {
                minimum = Math.Max(minimum, rangeMinimum);
            }

            if (TryConvert(range.Maximum, Math.Floor) is { } rangeMaximum)
            {
                maximum = Math.Min(maximum, rangeMaximum);
            }
        }

        return (minimum, maximum);

        static long? TryConvert(object? value, Func<double, double> round)
        {
            if (value is not IConvertible convertible
                || !double.TryParse(convertible.ToString(CultureInfo.InvariantCulture), NumberStyles.Float, CultureInfo.InvariantCulture, out var number))
            {
                return null;
            }

            number = round(number);
            return number switch
            {
                <= long.MinValue => long.MinValue,
                >= long.MaxValue => long.MaxValue,
                _ => (long)number,
            };
        }
    }

    private static async ValueTask<bool> ReadNamedArgumentsAsync(object instance, IReadOnlyList<ChatCommandArgumentProperty> properties, IList<string> arguments, Player? player)
    {
        var argumentProperties = properties.Where(property => property.Argument is { }).ToList();
        var requiredProperties = argumentProperties.Where(prop => prop.Argument is { IsRequired: true }).ToList();

        foreach (var property in argumentProperties)
        {
            var attribute = property.Argument!;
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

    private static async ValueTask<bool> TrySetPropertyValueAsync(object instance, ChatCommandArgumentProperty propertyInfo, string stringValue, Player? player)
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
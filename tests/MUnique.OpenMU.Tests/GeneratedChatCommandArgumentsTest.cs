// <copyright file="GeneratedChatCommandArgumentsTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Globalization;
using System.Reflection;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

/// <summary>
/// Tests for the properties of the chat command arguments classes, which are registered by generated code.
/// They have to be the same as the ones which are determined by reflection.
/// </summary>
[TestFixture]
public class GeneratedChatCommandArgumentsTest
{
    /// <summary>
    /// Gets the arguments classes of the game logic.
    /// </summary>
    private static IEnumerable<Type> ArgumentsTypes => typeof(ArgumentsBase).Assembly.GetTypes()
        .Where(type => type.IsClass && !type.IsGenericType && type.IsAssignableTo(typeof(ArgumentsBase)))
        .OrderBy(type => type.FullName, StringComparer.Ordinal);

    /// <summary>
    /// Tests that the properties of the arguments class are registered by the generated code, and that they are the same as the ones
    /// which are determined by reflection, in the same order.
    /// </summary>
    /// <param name="argumentsType">The type of the arguments class.</param>
    [TestCaseSource(nameof(ArgumentsTypes))]
    public void PropertiesAreEqual(Type argumentsType)
    {
        var generated = ChatCommandArguments.GetProperties(argumentsType);
        var reflected = ChatCommandArguments.CreatePropertiesByReflection(argumentsType);

        Assert.That(generated.All(p => p.SetValue.Target is not PropertyInfo), Is.True, "The properties were not registered by the generated code.");
        Assert.That(generated.Select(Describe), Is.EqualTo(reflected.Select(Describe)));
    }

    /// <summary>
    /// Tests that the generated setters set the same values as the setters by reflection.
    /// </summary>
    /// <param name="argumentsType">The type of the arguments class.</param>
    [TestCaseSource(nameof(ArgumentsTypes))]
    public void SettersSetTheValues(Type argumentsType)
    {
        if (argumentsType.IsAbstract || argumentsType.GetConstructor(Type.EmptyTypes) is null)
        {
            return;
        }

        var generatedInstance = Activator.CreateInstance(argumentsType)!;
        var reflectedInstance = Activator.CreateInstance(argumentsType)!;
        var generated = ChatCommandArguments.GetProperties(argumentsType);
        var reflected = ChatCommandArguments.CreatePropertiesByReflection(argumentsType);
        for (var i = 0; i < generated.Count; i++)
        {
            var value = CreateValue(generated[i].PropertyType);
            if (value is null)
            {
                continue;
            }

            generated[i].SetValue(generatedInstance, value);
            reflected[i].SetValue(reflectedInstance, value);
        }

        Assert.That(generatedInstance.ToString(), Is.EqualTo(reflectedInstance.ToString()));
    }

    private static object? CreateValue(Type type)
    {
        if (type == typeof(string))
        {
            return "test";
        }

        if (type == typeof(bool))
        {
            return true;
        }

        if (type.IsPrimitive)
        {
            return Convert.ChangeType("7", type, CultureInfo.InvariantCulture);
        }

        return null;
    }

    private static string Describe(ChatCommandArgumentProperty property)
    {
        var argument = property.Argument is { } a ? $"{a.ShortName}:{a.IsRequired}" : "-";
        var validValues = property.ValidValues is { } v ? string.Join('|', v.ValidValues) : "-";
        return $"{property.Name} {property.PropertyType} {argument} {validValues}";
    }
}

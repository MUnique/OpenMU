// <copyright file="TypeNameFactoryAttribute.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Annotations;

/// <summary>
/// Marks a static partial method which creates an object of a type which is specified by its full name,
/// e.g. <c>static partial INpcIntelligence? Create(string typeName, GameMap map)</c>.
/// The method is implemented by a code generator, without reflection: for each class of the assembly
/// which is assignable to the return type, it calls the public constructor with the most parameters
/// which can be supplied by the other parameters of the method, matched by their types.
/// If the type name is unknown, the method returns <c>null</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class TypeNameFactoryAttribute : Attribute
{
}

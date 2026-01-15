// <copyright file="PlugInAttribute.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.PlugIns;

/// <summary>
/// An attribute which describes an implementation of a plugin interface.
/// May be helpful for debugging and the user interface.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class PlugInAttribute : Attribute;

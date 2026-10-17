// <copyright file="PlugInRegistration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.PlugIns;

/// <summary>
/// A plugin type and the action which registers it at a <see cref="PlugInManager"/>.
/// </summary>
/// <param name="Type">The type of the plugin.</param>
/// <param name="Register">
/// The action which registers the plugin at the plugin manager, by calling
/// <see cref="PlugInManager.RegisterPlugIn{TPlugInInterface, TPlugInClass}"/> for each of its plugin interfaces.
/// </param>
public sealed record PlugInRegistration(Type Type, Action<PlugInManager> Register);

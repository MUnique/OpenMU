// <copyright file="ISkillRequirementsViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views;

/// <summary>
/// Interface of a view whose client takes the requirements of the skills, and of the items which
/// teach them, from the server, instead of its own data files.
/// </summary>
public interface ISkillRequirementsViewPlugIn : IViewPlugIn
{
    /// <summary>
    /// Sends the requirements and costs of all skills and the requirements to learn them with items,
    /// as the server checks them. The client keeps them for the rest of the session.
    /// </summary>
    ValueTask ShowSkillRequirementsAsync();
}

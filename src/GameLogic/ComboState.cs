// <copyright file="ComboState.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

/// <summary>
/// Defines a state in the <see cref="ComboStateMachine"/>.
/// It includes data about the <see cref="RequiredSkill"/> to achieve this state.
/// </summary>
/// <seealso cref="MUnique.OpenMU.GameLogic.State" />
public class ComboState : State
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ComboState"/> class.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="requiredSkill">The required skill.</param>
    public ComboState(Guid id, Skill? requiredSkill)
        : base(id)
    {
        this.RequiredSkill = requiredSkill;
    }

    /// <summary>
    /// Gets the required skill to achieve this state.
    /// </summary>
    public Skill? RequiredSkill { get; }
}
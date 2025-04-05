// <copyright file="ComboStateMachine.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using System.Collections.Concurrent;
using System.Diagnostics;

/// <summary>
/// A state machine which is dynamically built, based on a <see cref="SkillComboDefinition"/>.
/// For creation, use the factory method <see cref="Create"/>.
/// </summary>
public sealed class ComboStateMachine : StateMachine
{
    private static readonly ConcurrentDictionary<SkillComboDefinition, (ComboState Inital, ComboState Final)> StateCache = new();

    private readonly TimeSpan _maximumCompletionTime;
    private DateTime _lastStartedCombo;

    /// <summary>
    /// Initializes a new instance of the <see cref="ComboStateMachine"/> class.
    /// </summary>
    /// <param name="initial">The initial state.</param>
    /// <param name="final">The final state.</param>
    /// <param name="maximumCompletionTime">The maximum completion time for combos.</param>
    private ComboStateMachine(ComboState initial, ComboState final, TimeSpan maximumCompletionTime)
        : base(initial)
    {
        this._maximumCompletionTime = maximumCompletionTime;
        this.InitialState = initial;
        this.FinalState = final;
    }

    /// <summary>
    /// Gets or sets the final state.
    /// When this state is achieved, the state machine will proceed to the <see cref="InitialState"/>
    /// to handle the next combo attempt.
    /// </summary>
    public ComboState FinalState { get; set; }

    /// <summary>
    /// Gets or sets the initial state.
    /// </summary>
    public ComboState InitialState { get; set; }

    /// <summary>
    /// Creates the specified combo definition, based on the <see cref="SkillComboDefinition"/>.
    /// </summary>
    /// <param name="comboDefinition">The combo definition.</param>
    /// <returns>The created <see cref="ComboStateMachine"/>.</returns>.
    public static ComboStateMachine Create(SkillComboDefinition comboDefinition)
    {
        var states = GetOrCreateStates(comboDefinition);
        return new ComboStateMachine(states.Initial, states.Final, comboDefinition.MaximumCompletionTime);
    }

    /// <summary>
    /// Registers the skill to trigger potential state advancements.
    /// </summary>
    /// <param name="skill">The performed skill.</param>
    /// <returns><see langword="true"/>, if the combo completed; otherwise, <see langword="false"/>.</returns>
    public async ValueTask<bool> RegisterSkillAsync(Skill skill)
    {
        if (DateTime.UtcNow - this._lastStartedCombo > this._maximumCompletionTime)
        {
            // If it took to long, reset to initial state.
            await this.TryAdvanceToAsync(this.InitialState).ConfigureAwait(false);
        }

        var nextPossibleSkillState = this.CurrentState?.PossibleTransitions?.OfType<ComboState>().FirstOrDefault(t => t.RequiredSkill == skill.GetBaseSkill());
        if (nextPossibleSkillState is null)
        {
            // If it's the wrong skill, reset to initial state.
            await this.TryAdvanceToAsync(this.InitialState).ConfigureAwait(false);
            return false;
        }

        if (this.CurrentState == this.InitialState)
        {
            this._lastStartedCombo = DateTime.UtcNow;
        }

        await this.TryAdvanceToAsync(nextPossibleSkillState).ConfigureAwait(false);

        var canComplete = this.CurrentState?.PossibleTransitions?.Contains(this.FinalState) ?? false;
        if (canComplete && await this.TryAdvanceToAsync(this.FinalState).ConfigureAwait(false))
        {
            await this.TryAdvanceToAsync(this.InitialState).ConfigureAwait(false);
            return true;
        }

        return false;
    }

    private static (ComboState Initial, ComboState Final) GetOrCreateStates(SkillComboDefinition comboDefinition)
    {
        return StateCache.GetOrAdd(comboDefinition, BuildStates);
    }

    private static (ComboState Initial, ComboState Final) BuildStates(SkillComboDefinition comboDefinition)
    {
        var initialState = new ComboState(Guid.NewGuid(), null) { Name = "Initial", PossibleTransitions = new List<State>() };
        var finalState = new ComboState(Guid.NewGuid(), null) { Name = "Finished", PossibleTransitions = new List<State> { initialState } };

        var statesPerStep = new Dictionary<int, List<State>>();
        foreach (var groupedSteps in comboDefinition.Steps.GroupBy(s => s.Order).OrderByDescending(s => s.Key))
        {
            foreach (var step in groupedSteps)
            {
                var stepState = new ComboState(Guid.NewGuid(), step.Skill);
                stepState.Name = $"Step {step.Order}: {step.Skill?.Name}";
                stepState.PossibleTransitions = new List<State>();
                stepState.PossibleTransitions.Add(initialState);

                if (step.Order == 1)
                {
                    initialState.PossibleTransitions.Add(stepState);
                }

                if (step.IsFinalStep)
                {
                    stepState.PossibleTransitions.Add(finalState);
                }
                else
                {
                    if (statesPerStep.TryGetValue(step.Order + 1, out var nextSteps))
                    {
                        nextSteps
                            .OfType<ComboState>()
                            .Where(p => p.RequiredSkill != step.Skill)
                            .ForEach(stepState.PossibleTransitions.Add);
                    }
                    else
                    {
                        Debug.Fail("Inconsistent combo data");
                    }
                }

                if (!statesPerStep.TryGetValue(step.Order, out var list))
                {
                    list = new List<State>();
                    statesPerStep[step.Order] = list;
                }

                list.Add(stepState);
            }
        }

        return (initialState, finalState);
    }
}
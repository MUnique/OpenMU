// <copyright file="SummoningOrbConsumeHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The summoning orb consume handler.
/// There is only one "Orb" item definition which allows to learn different skills, depending on the item level.
/// The skill is determined by adding the item level to the skill number of the <see cref="ItemDefinition.Skill"/>,
/// see <see cref="LearnableSkillRequirements.GetLearnableSkill"/>.
/// </summary>
[Guid("71C8E542-4868-487E-BC92-0B7CC7CAEC8B")]
[PlugIn]
[Display(Name = nameof(PlugInResources.SummoningOrbConsumeHandlerPlugIn_Name), Description = nameof(PlugInResources.SummoningOrbConsumeHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
public class SummoningOrbConsumeHandlerPlugIn : LearnablesConsumeHandlerPlugIn
{
    /// <inheritdoc />
    public override ItemIdentifier Key => ItemConstants.SummonOrb;
}
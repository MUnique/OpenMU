// <copyright file="SkillAttributeSystem.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Attributes;

using MUnique.OpenMU.AttributeSystem;

/// <summary>
/// The attributes of a skill of a character, e.g. its damage bonus, which depend on the skill level and on
/// the attributes of the character using it (see <see cref="Skill.AttributeRelationships"/>).
/// </summary>
/// <remarks>
/// The system follows the attribute system of the character, which is replaced whenever the character enters
/// the game again. It's released together with the skill list of the character, see <see cref="SkillList"/>.
/// </remarks>
public sealed class SkillAttributeSystem : AttributeSystem
{
    private readonly SkillEntry _skillEntry;

    private readonly SimpleElement _levelElement;

    /// <summary>
    /// Initializes a new instance of the <see cref="SkillAttributeSystem"/> class.
    /// </summary>
    /// <param name="skillEntry">The skill entry of the character.</param>
    /// <param name="relationships">The attribute relationships of the skill and its base skills.</param>
    /// <param name="characterAttributes">The attribute system of the character which uses the skill.</param>
    public SkillAttributeSystem(SkillEntry skillEntry, IEnumerable<AttributeRelationship> relationships, IAttributeSystem characterAttributes)
        : base([], [], [])
    {
        this._skillEntry = skillEntry;
        this._levelElement = new SimpleElement(skillEntry.Level, AggregateType.AddRaw);
        this.AddElement(this._levelElement, Stats.SkillLevel);
        foreach (var relationship in relationships)
        {
            this.AddAttributeRelationship(relationship, characterAttributes, relationship.AggregateType);
        }

        this._skillEntry.PropertyChanged += this.OnSkillEntryChanged;
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            this._skillEntry.PropertyChanged -= this.OnSkillEntryChanged;
        }

        base.Dispose(disposing);
    }

    private void OnSkillEntryChanged(object? sender, EventArgs e)
    {
        this._levelElement.Value = this._skillEntry.Level;
    }
}

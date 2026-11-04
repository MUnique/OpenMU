// <copyright file="AttributeDefinition.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AttributeSystem;

/// <summary>
/// Defines and Identifies a Attribute.
/// In the future it may also contain additional data, like a maximum limit of the reachable value to do balancing.
/// </summary>
public class AttributeDefinition : IEquatable<AttributeDefinition>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AttributeDefinition"/> class.
    /// </summary>
    public AttributeDefinition()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AttributeDefinition"/> class.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="designation">The designation.</param>
    /// <param name="description">The description.</param>
    public AttributeDefinition(Guid id, string designation, string description)
    {
        this.Id = id;
        this.Designation = designation;
        this.Description = description;
    }

    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the designation.
    /// </summary>
    public string? Designation { get; set; }

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the maximum value of this attribute, if the value should be capped.
    /// </summary>
    public float? MaximumValue { get; set; }

    /// <summary>
    /// Implements the operator ==.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static bool operator ==(AttributeDefinition? lhs, AttributeDefinition? rhs)
    {
        if (ReferenceEquals(lhs, rhs))
        {
            return true;
        }

        if (lhs is null || rhs is null)
        {
            return false;
        }

        return lhs.Equals(rhs);
    }

    /// <summary>
    /// Implements the operator !=.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static bool operator !=(AttributeDefinition? lhs, AttributeDefinition? rhs)
    {
        return !(lhs == rhs);
    }

    /// <summary>
    /// Returns a <see cref="string" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="string" /> that represents this instance.
    /// </returns>
    public override string? ToString()
    {
        return this.Designation;
    }

    /// <summary>
    /// Determines whether the specified <see cref="object" />, is equal to this instance.
    /// </summary>
    /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
    /// <returns>
    ///   <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
    /// </returns>
    public override bool Equals(object? obj)
    {
        return this.Equals(obj as AttributeDefinition);
    }

    /// <inheritdoc />
    public bool Equals(AttributeDefinition? other)
    {
        return this.Id == other?.Id;
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>
    /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
    /// </returns>
    public override int GetHashCode()
    {
        return this.Id.GetHashCode();
    }
}
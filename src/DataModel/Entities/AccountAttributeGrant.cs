// <copyright file="AccountAttributeGrant.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Entities;

using MUnique.OpenMU.AttributeSystem;

/// <summary>
/// A value of an attribute which is granted to an <see cref="Account"/>, optionally until a point in time,
/// e.g. a VIP status which was bought for 30 days.
/// </summary>
/// <remarks>
/// While it's valid, the value is added to the attribute of every character of the account, and it counts
/// for the <see cref="Configuration.GameServerDefinition.JoinRequirements"/> of the game servers.
/// Unlike the <see cref="Account.Attributes"/>, which are permanent values of the account, a grant
/// can expire, and several grants of the same attribute add up.
/// </remarks>
public class AccountAttributeGrant
{
    /// <summary>
    /// Gets or sets the attribute which is granted.
    /// </summary>
    [Required]
    public virtual AttributeDefinition? Attribute { get; set; }

    /// <summary>
    /// Gets or sets the value which is added to the attribute.
    /// </summary>
    public float Value { get; set; }

    /// <summary>
    /// Gets or sets the timestamp (UTC) until which the grant is valid; <c>null</c>, if it doesn't expire.
    /// </summary>
    public DateTime? ValidUntil { get; set; }

    /// <summary>
    /// Determines whether the grant is valid at the specified timestamp.
    /// </summary>
    /// <param name="utcNow">The current timestamp (UTC).</param>
    /// <returns><c>true</c>, if the grant is valid at the specified timestamp.</returns>
    public bool IsValidAt(DateTime utcNow) => this.ValidUntil is not { } validUntil || validUntil > utcNow;

    /// <inheritdoc />
    public override string ToString()
    {
        return this.ValidUntil is { } validUntil
            ? $"{this.Attribute}: {this.Value} (until {validUntil:u})"
            : $"{this.Attribute}: {this.Value}";
    }
}

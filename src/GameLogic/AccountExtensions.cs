// <copyright file="AccountExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// Extension methods for the <see cref="Account"/>.
/// </summary>
public static class AccountExtensions
{
    /// <summary>
    /// Gets the value of an attribute of the account, which is the sum of the value of its
    /// <see cref="Account.Attributes"/> and of its valid <see cref="Account.AttributeGrants"/>.
    /// </summary>
    /// <param name="account">The account.</param>
    /// <param name="attribute">The attribute.</param>
    /// <param name="utcNow">The current timestamp (UTC), at which the grants have to be valid.</param>
    /// <returns>The value of the attribute of the account.</returns>
    public static float GetAccountAttributeValue(this Account account, AttributeDefinition attribute, DateTime utcNow)
    {
        var permanentValue = (account.Attributes ?? [])
            .Where(statAttribute => statAttribute.Definition == attribute)
            .Sum(statAttribute => statAttribute.Value);
        var grantedValue = (account.AttributeGrants ?? [])
            .Where(grant => grant.Attribute == attribute && grant.IsValidAt(utcNow))
            .Sum(grant => grant.Value);
        return permanentValue + grantedValue;
    }

    /// <summary>
    /// Gets the first of the requirements which the account doesn't fulfill.
    /// Only the <see cref="Account.Attributes"/> and the valid <see cref="Account.AttributeGrants"/> count.
    /// </summary>
    /// <param name="account">The account.</param>
    /// <param name="requirements">The requirements.</param>
    /// <param name="utcNow">The current timestamp (UTC), at which the grants have to be valid.</param>
    /// <returns>The first requirement which isn't fulfilled; <c>null</c>, if all of them are fulfilled.</returns>
    public static AttributeRequirement? GetFirstUnfulfilledRequirement(this Account account, IEnumerable<AttributeRequirement>? requirements, DateTime utcNow)
    {
        return requirements?.FirstOrDefault(requirement =>
            requirement.Attribute is { } attribute
            && account.GetAccountAttributeValue(attribute, utcNow) < requirement.MinimumValue);
    }
}

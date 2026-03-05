// <copyright file="LessThanOrEqualToAttribute.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Validation;

/// <summary>
/// Validates that the decorated property's value is less than or equal to the value
/// of the property named by <see cref="OtherProperty"/>.
/// Both properties must implement <see cref="IComparable"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class LessThanOrEqualToAttribute : ValidationAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LessThanOrEqualToAttribute"/> class.
    /// </summary>
    /// <param name="otherProperty">The name of the property whose value must be greater than or equal to this property's value.</param>
    public LessThanOrEqualToAttribute(string otherProperty)
        : base("{0} must be less than or equal to {1}.")
    {
        this.OtherProperty = otherProperty;
    }

    /// <summary>
    /// Gets the name of the property to compare against.
    /// </summary>
    public string OtherProperty { get; }

    /// <inheritdoc />
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var otherPropertyInfo = validationContext.ObjectType.GetProperty(this.OtherProperty);
        if (otherPropertyInfo is null)
        {
            return new ValidationResult($"Unknown property: {this.OtherProperty}.");
        }

        var otherValue = otherPropertyInfo.GetValue(validationContext.ObjectInstance);

        if (value is IComparable comparable && otherValue is not null)
        {
            if (comparable.CompareTo(otherValue) > 0)
            {
                var memberNames = new[] { validationContext.MemberName ?? string.Empty, this.OtherProperty };
                return new ValidationResult(
                    this.FormatErrorMessage($"{validationContext.DisplayName} ({value}) to {this.OtherProperty} ({otherValue})"),
                    memberNames);
            }
        }

        return ValidationResult.Success;
    }
}

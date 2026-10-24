// <copyright file="ShowInTableAttribute.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Composition;

/// <summary>
/// Marks a property to be shown as additional column on the UI, when objects of the declaring type are listed in a table.
/// By default, only the name of the objects is shown.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class ShowInTableAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the order of the column, relative to the other additional columns.
    /// </summary>
    public int Order { get; set; }
}

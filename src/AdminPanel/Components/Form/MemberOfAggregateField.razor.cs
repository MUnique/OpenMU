// <copyright file="MemberOfAggregateField.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Components.Form
{
    using MUnique.OpenMU.DataModel.Composition;

    /// <summary>
    /// Component which shows a property which is marked with the <see cref="MemberOfAggregateAttribute"/>
    /// as a form. If it's null, a Create-Button is rendered.
    /// </summary>
    /// <typeparam name="TObject">The type of the object.</typeparam>
    public partial class MemberOfAggregateField<TObject>
        where TObject : class
    {
    }
}

// <copyright file="CreationAutoFields.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Components.Form
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.AspNetCore.Components.Forms;
    using MUnique.OpenMU.DataModel.Composition;

    /// <summary>
    /// A razor component which automatically generates input fields for all properties for the type of the enclosing form model,
    /// excluding properties marked with the <see cref="HiddenAtCreationAttribute"/>.
    /// Must be used inside a <see cref="EditForm"/>.
    /// </summary>
    public class CreationAutoFields : AutoFields
    {
        /// <inheritdoc />
        protected override IEnumerable<PropertyInfo> Properties =>
            base.Properties.Where(p => p.GetCustomAttribute<HiddenAtCreationAttribute>() is null);
    }
}
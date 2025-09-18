// <copyright file="PlugInConfiguration.Assignable.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.Model;

using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Adds assignment support so cached instances can be updated on save.
/// </summary>
internal partial class PlugInConfiguration :
    IAssignable,
    IAssignable<MUnique.OpenMU.Persistence.EntityFramework.Model.PlugInConfiguration>
{
    /// <inheritdoc />
    public virtual void AssignValuesOf(object other, MUnique.OpenMU.DataModel.Configuration.GameConfiguration gameConfiguration)
    {
        if (other is MUnique.OpenMU.Persistence.EntityFramework.Model.PlugInConfiguration typedOther)
        {
            this.AssignValuesOf(typedOther, gameConfiguration);
        }
    }

    /// <inheritdoc />
    public virtual void AssignValuesOf(MUnique.OpenMU.Persistence.EntityFramework.Model.PlugInConfiguration other, MUnique.OpenMU.DataModel.Configuration.GameConfiguration gameConfiguration)
    {
        this.Id = other.Id;
        this.TypeId = other.TypeId;
        this.IsActive = other.IsActive;
        this.CustomPlugInSource = other.CustomPlugInSource;
        this.ExternalAssemblyName = other.ExternalAssemblyName;
        this.CustomConfiguration = other.CustomConfiguration;
    }
}

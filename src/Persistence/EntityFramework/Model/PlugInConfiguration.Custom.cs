// <copyright file="PlugInConfiguration.Custom.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.Model;

using MUnique.OpenMU.DataModel;

/// <summary>
/// Custom additions to the generated <see cref="PlugInConfiguration"/>.
/// </summary>
internal partial class PlugInConfiguration : IAssignable, IAssignable<MUnique.OpenMU.PlugIns.PlugInConfiguration>, ICloneable<MUnique.OpenMU.PlugIns.PlugInConfiguration>
{
    /// <inheritdoc />
    public virtual MUnique.OpenMU.PlugIns.PlugInConfiguration Clone(DataModel.Configuration.GameConfiguration gameConfiguration)
    {
        var clone = new PlugInConfiguration();
        clone.AssignValuesOf(this, gameConfiguration);
        return clone;
    }

    /// <inheritdoc />
    public virtual void AssignValuesOf(object other, MUnique.OpenMU.DataModel.Configuration.GameConfiguration gameConfiguration)
    {
        if (other is MUnique.OpenMU.Persistence.EntityFramework.Model.PlugInConfiguration typedOther)
        {
            this.AssignValuesOf(typedOther, gameConfiguration);
        }
    }

    /// <inheritdoc />
    public virtual void AssignValuesOf(MUnique.OpenMU.PlugIns.PlugInConfiguration other, MUnique.OpenMU.DataModel.Configuration.GameConfiguration gameConfiguration)
    {
        this.TypeId = other.TypeId;
        this.IsActive = other.IsActive;
        this.CustomPlugInSource = other.CustomPlugInSource;
        this.ExternalAssemblyName = other.ExternalAssemblyName;
        this.CustomConfiguration = other.CustomConfiguration;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{base.ToString()} ({this.Id})";
    }
}
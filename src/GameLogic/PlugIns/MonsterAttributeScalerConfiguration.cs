// <copyright file="MonsterAttributeScalerConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

/// <summary>
/// Configuration for the <see cref="MonsterAttributeScaler"/>.
/// </summary>
public class MonsterAttributeScalerConfiguration
{
    private float _scaleAllPercentage;
    private float _damagePercentage = 25.0f;
    private float _attackRatePercentage = 25.0f;
    private float _defenseRatePercentage = 25.0f;
    private float _defensePercentage = 25.0f;
    private float _healthPercentage = 25.0f;

    /// <summary>
    /// Gets or sets the percentage applied to all stats at once.
    /// When set above 0, cascades to all individual fields.
    /// </summary>
    [System.ComponentModel.DataAnnotations.Display(Name = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.MonsterAttributeScalerConfiguration_ScaleAllPercentage_Caption), ResourceType = typeof(MUnique.OpenMU.GameLogic.Properties.PlugInResources))]
    public float ScaleAllPercentage
    {
        get => this._scaleAllPercentage;
        set
        {
            this._scaleAllPercentage = value;
            if (value > 0)
            {
                this._damagePercentage = value;
                this._attackRatePercentage = value;
                this._defenseRatePercentage = value;
                this._defensePercentage = value;
                this._healthPercentage = value;
            }
        }
    }

    /// <summary>
    /// Gets or sets the percentage by which monster damage (MinPhysBaseDmg, MaxPhysBaseDmg) is increased.
    /// </summary>
    [System.ComponentModel.DataAnnotations.Display(Name = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.MonsterAttributeScalerConfiguration_DamagePercentage_Caption), ResourceType = typeof(MUnique.OpenMU.GameLogic.Properties.PlugInResources))]
    public float DamagePercentage
    {
        get => this.ScaleAllActive ? this._scaleAllPercentage : this._damagePercentage;
        set => this.SetIndividualField(ref this._damagePercentage, value);
    }

    /// <summary>
    /// Gets or sets the percentage by which the monster attack rate is increased.
    /// </summary>
    [System.ComponentModel.DataAnnotations.Display(Name = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.MonsterAttributeScalerConfiguration_AttackRatePercentage_Caption), ResourceType = typeof(MUnique.OpenMU.GameLogic.Properties.PlugInResources))]
    public float AttackRatePercentage
    {
        get => this.ScaleAllActive ? this._scaleAllPercentage : this._attackRatePercentage;
        set => this.SetIndividualField(ref this._attackRatePercentage, value);
    }

    /// <summary>
    /// Gets or sets the percentage by which monster defense rate is increased.
    /// </summary>
    [System.ComponentModel.DataAnnotations.Display(Name = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.MonsterAttributeScalerConfiguration_DefenseRatePercentage_Caption), ResourceType = typeof(MUnique.OpenMU.GameLogic.Properties.PlugInResources))]
    public float DefenseRatePercentage
    {
        get => this.ScaleAllActive ? this._scaleAllPercentage : this._defenseRatePercentage;
        set => this.SetIndividualField(ref this._defenseRatePercentage, value);
    }

    /// <summary>
    /// Gets or sets the percentage by which monster defense is increased.
    /// </summary>
    [System.ComponentModel.DataAnnotations.Display(Name = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.MonsterAttributeScalerConfiguration_DefensePercentage_Caption), ResourceType = typeof(MUnique.OpenMU.GameLogic.Properties.PlugInResources))]
    public float DefensePercentage
    {
        get => this.ScaleAllActive ? this._scaleAllPercentage : this._defensePercentage;
        set => this.SetIndividualField(ref this._defensePercentage, value);
    }

    /// <summary>
    /// Gets or sets the percentage by which monster maximum health is increased.
    /// </summary>
    [System.ComponentModel.DataAnnotations.Display(Name = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.MonsterAttributeScalerConfiguration_HealthPercentage_Caption), ResourceType = typeof(MUnique.OpenMU.GameLogic.Properties.PlugInResources))]
    public float HealthPercentage
    {
        get => this.ScaleAllActive ? this._scaleAllPercentage : this._healthPercentage;
        set => this.SetIndividualField(ref this._healthPercentage, value);
    }

    private bool ScaleAllActive => this._scaleAllPercentage > 0;

    private void SetIndividualField(ref float field, float value)
    {
        if (this.ScaleAllActive)
        {
            return;
        }

        field = value;
    }
}

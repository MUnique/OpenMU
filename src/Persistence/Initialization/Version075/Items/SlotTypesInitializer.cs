// <copyright file="SlotTypesInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version075.Items;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// Initializer for <see cref="ItemSlotType"/>s.
/// </summary>
public class SlotTypesInitializer : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SlotTypesInitializer"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public SlotTypesInitializer(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    public override void Initialize()
    {
        var leftHand = this.Context.CreateNew<ItemSlotType>();
        leftHand.SetGuid(100);
        leftHand.Description = "Left Hand";
        leftHand.ItemSlots.Add(0);
        this.GameConfiguration.ItemSlotTypes.Add(leftHand);

        var rightHand = this.Context.CreateNew<ItemSlotType>();
        rightHand.SetGuid(101);
        rightHand.Description = "Right Hand";
        rightHand.ItemSlots.Add(1);
        this.GameConfiguration.ItemSlotTypes.Add(rightHand);

        var leftOrRightHand = this.Context.CreateNew<ItemSlotType>();
        leftOrRightHand.SetGuid(1);
        leftOrRightHand.Description = "Left or Right Hand";
        leftOrRightHand.ItemSlots.Add(0);
        leftOrRightHand.ItemSlots.Add(1);
        this.GameConfiguration.ItemSlotTypes.Add(leftOrRightHand);

        var helm = this.Context.CreateNew<ItemSlotType>();
        helm.SetGuid(2);
        helm.Description = "Helm";
        helm.ItemSlots.Add(2);
        this.GameConfiguration.ItemSlotTypes.Add(helm);

        var armor = this.Context.CreateNew<ItemSlotType>();
        armor.SetGuid(3);
        armor.Description = "Armor";
        armor.ItemSlots.Add(3);
        this.GameConfiguration.ItemSlotTypes.Add(armor);

        var pants = this.Context.CreateNew<ItemSlotType>();
        pants.SetGuid(4);
        pants.Description = "Pants";
        pants.ItemSlots.Add(4);
        this.GameConfiguration.ItemSlotTypes.Add(pants);

        var gloves = this.Context.CreateNew<ItemSlotType>();
        gloves.SetGuid(5);
        gloves.Description = "Gloves";
        gloves.ItemSlots.Add(5);
        this.GameConfiguration.ItemSlotTypes.Add(gloves);

        var boots = this.Context.CreateNew<ItemSlotType>();
        boots.SetGuid(6);
        boots.Description = "Boots";
        boots.ItemSlots.Add(6);
        this.GameConfiguration.ItemSlotTypes.Add(boots);

        var wings = this.Context.CreateNew<ItemSlotType>();
        wings.SetGuid(7);
        wings.Description = "Wings";
        wings.ItemSlots.Add(7);
        this.GameConfiguration.ItemSlotTypes.Add(wings);

        var pet = this.Context.CreateNew<ItemSlotType>();
        pet.SetGuid(8);
        pet.Description = "Pet";
        pet.ItemSlots.Add(8);
        this.GameConfiguration.ItemSlotTypes.Add(pet);

        var pendant = this.Context.CreateNew<ItemSlotType>();
        pendant.SetGuid(9);
        pendant.Description = "Pendant";
        pendant.ItemSlots.Add(9);
        this.GameConfiguration.ItemSlotTypes.Add(pendant);

        var ring = this.Context.CreateNew<ItemSlotType>();
        ring.SetGuid(10);
        ring.Description = "Ring";
        ring.ItemSlots.Add(10);
        ring.ItemSlots.Add(11);
        this.GameConfiguration.ItemSlotTypes.Add(ring);
    }
}
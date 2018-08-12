namespace MUnique.OpenMU.Persistence.Initialization.Items
{
    using MUnique.OpenMU.DataModel.Entities;

    interface IItemBuilder
    {
        IBuilderWeaponEnter WeaponAtSlot(byte slotNumber);

        IBuilderShieldEnter ShieldAtSlot(byte slotNumber);

        IBuilderSetItemEnter SetItemAtSlot(byte slotNumber);

        IBuilderOrb OrbAtSlot(byte slotNumber);

        IBuilderScroll ScrollAtSlot(byte slotNumber);

        IBuilderPotion PotionAtSlot(byte slotNumber);
    }

    public interface IBuilderWeaponEnter
    {
        IBuilderEquipableChained Sword(byte itemNumber);

        IBuilderEquipableChained Axe(byte itemNumber);

        IBuilderEquipableChained MaceOrBlunt(byte itemNumber);

        IBuilderEquipableChained Spear(byte itemNumber);

        IBuilderEquipableChained BowOrCrossbow(byte itemNumber);

        IBuilderEquipableChained StaffOrBook(byte itemNumber);
    }

    public interface IBuilderShieldEnter
    {
        IBuilderEquipableChained Shield(byte itemNumber);
    }

    public interface IBuilderSetItemEnter
    {
        IBuilderSetItemPartChained SetNumber(byte groupNumber);
    }

    public interface IBuilderOrb
    {
        IBuilderOrbAndScrollChained Orb(byte itemNumber);
    }

    public interface IBuilderScroll
    {
        IBuilderOrbAndScrollChained Scroll(byte itemNumber);
    }

    public interface IBuilderPotion
    {
        IBuilderPotionChained Potion(byte itemNumber);
    }

    public interface IBuilderEquipableChained
    {
        IBuilderDurabilityChained Durability(byte durability);

        IBuilderLevelChained Level(byte level);

        IBuilderOptionLevelChained OptionLevel(byte optionLevel);

        IBuilderWithSkillChained WithSkill();

        IBuilderWithLuckChained WithLuck();

        Item Make();
    }

    public interface IBuilderDurabilityChained
    {
        IBuilderLevelChained Level(byte level);

        IBuilderOptionLevelChained OptionLevel(byte optionLevel);

        IBuilderWithSkillChained WithSkill();

        IBuilderWithLuckChained WithLuck();

        Item Make();
    }

    public interface IBuilderPotionDurabilityChained
    {
        IBuilderPotionLevelChained PotionLevel(byte level);

        Item Make();
    }

    public interface IBuilderLevelChained
    {
        IBuilderDurabilityChained Durability(byte durability);

        IBuilderOptionLevelChained OptionLevel(byte optionLevel);

        IBuilderWithSkillChained WithSkill();

        IBuilderWithLuckChained WithLuck();

        Item Make();
    }

    public interface IBuilderPotionLevelChained
    {
        IBuilderPotionDurabilityChained StackSize(byte durability);

        Item Make();
    }

    public interface IBuilderOptionLevelChained
    {
        IBuilderDurabilityChained Durability(byte durability);

        IBuilderLevelChained Level(byte level);

        IBuilderWithSkillChained WithSkill();

        IBuilderWithLuckChained WithLuck();

        Item Make();
    }

    public interface IBuilderWithSkillChained
    {
        IBuilderDurabilityChained Durability(byte durability);

        IBuilderLevelChained Level(byte level);

        IBuilderOptionLevelChained OptionLevel(byte optionLevel);

        IBuilderWithLuckChained WithLuck();

        Item Make();
    }

    public interface IBuilderWithLuckChained
    {
        IBuilderDurabilityChained Durability(byte durability);

        IBuilderLevelChained Level(byte level);

        IBuilderOptionLevelChained OptionLevel(byte optionLevel);

        IBuilderWithSkillChained WithSkill();

        Item Make();
    }

    public interface IBuilderSetItemPartChained
    {
        IBuilderEquipableChained Helm();

        IBuilderEquipableChained Armor();

        IBuilderEquipableChained Pants();

        IBuilderEquipableChained Gloves();

        IBuilderEquipableChained Boots();
    }

    public interface IBuilderOrbAndScrollChained
    {
        Item Make();
    }

    public interface IBuilderPotionChained
    {
        IBuilderPotionDurabilityChained StackSize(byte durability);

        IBuilderPotionLevelChained PotionLevel(byte level);

        Item Make();
    }
}

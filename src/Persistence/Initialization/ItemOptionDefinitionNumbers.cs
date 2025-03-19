// <copyright file="ItemOptionDefinitionNumbers.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization;

/// <summary>
/// Internal numbers for item option definitions which are helpful when creating their identifiers.
/// </summary>
internal static class ItemOptionDefinitionNumbers
{
    /// <summary>
    /// Gets the luck option number.
    /// </summary>
    public static short Luck => 0x01;

    /// <summary>
    /// Gets the defense option option number.
    /// </summary>
    public static short DefenseOption => 0x02;

    /// <summary>
    /// Gets the physical attack option number.
    /// </summary>
    public static short PhysicalAttack => 0x03;

    /// <summary>
    /// Gets the wizardry attack option number.
    /// </summary>
    public static short WizardryAttack => 0x04;

    /// <summary>
    /// Gets the curse attack option number.
    /// </summary>
    public static short CurseAttack => 0x05;

    /// <summary>
    /// Gets the defense option option number.
    /// </summary>
    public static short DefenseRateOption => 0x06;

    /// <summary>
    /// Gets the physical and wizardry attack option number.
    /// </summary>
    /// <remarks>Used for MG "magic swords", which always increase both damage types simultaneously.</remarks>
    public static short PhysicalAndWizardryAttack => 0x07;

    /// <summary>
    /// Gets the jewelery health option number.
    /// </summary>
    public static short JeweleryHealth => 0x10;

    /// <summary>
    /// Gets the excellent defense option number.
    /// </summary>
    public static short ExcellentDefense => 0x12;

    /// <summary>
    /// Gets the excellent physical option number.
    /// </summary>
    public static short ExcellentPhysical => 0x13;

    /// <summary>
    /// Gets the excellent wizardry option number.
    /// </summary>
    public static short ExcellentWizardry => 0x14;

    /// <summary>
    /// Gets the excellent curse option number.
    /// </summary>
    public static short ExcellentCurse => 0x15;

    /// <summary>
    /// Gets the defense set bonus option number.
    /// </summary>
    public static short DefenseSetBonusOption => 0x20;

    /// <summary>
    /// Gets the defense rate set bonus option number.
    /// </summary>
    public static short DefenseRateSetBonusOption => 0x21;

    /// <summary>
    /// Gets the ancient option option number.
    /// </summary>
    public static short AncientOption => 0x29;

    /// <summary>
    /// Gets the ancient bonus option number.
    /// </summary>
    public static short AncientBonus => 0x30;

    /// <summary>
    /// Gets the socket bonus option number.
    /// </summary>
    public static short SocketBonus => 0x31;

    /// <summary>
    /// Gets the socket fire option number.
    /// </summary>
    public static short SocketFire => 0x32;

    /// <summary>
    /// Gets the socket water option number.
    /// </summary>
    public static short SocketWater => 0x33;

    /// <summary>
    /// Gets the socket ice option number.
    /// </summary>
    public static short SocketIce => 0x34;

    /// <summary>
    /// Gets the socket wind option number.
    /// </summary>
    public static short SocketWind => 0x35;

    /// <summary>
    /// Gets the socket lightning option number.
    /// </summary>
    public static short SocketLightning => 0x36;

    /// <summary>
    /// Gets the socket earth option number.
    /// </summary>
    public static short SocketEarth => 0x37;

    /// <summary>
    /// Gets the guardian option1 option number.
    /// </summary>
    public static short GuardianOption1 => 0x41;

    /// <summary>
    /// Gets the guardian option2 option number.
    /// </summary>
    public static short GuardianOption2 => 0x42;

    /// <summary>
    /// Gets the harmony defense option number.
    /// </summary>
    public static short HarmonyDefense => 0x50;

    /// <summary>
    /// Gets the harmony physical option number.
    /// </summary>
    public static short HarmonyPhysical => 0x51;

    /// <summary>
    /// Gets the harmony wizardry option number.
    /// </summary>
    public static short HarmonyWizardry => 0x52;

    /// <summary>
    /// Gets the harmony curse option number.
    /// </summary>
    public static short HarmonyCurse => 0x53;

    /// <summary>
    /// Gets the wing defense option number.
    /// </summary>
    public static short WingDefense => 0x60;

    /// <summary>
    /// Gets the wing physical option number.
    /// </summary>
    public static short WingPhysical => 0x61;

    /// <summary>
    /// Gets the wing wizardry option number.
    /// </summary>
    public static short WingWizardry => 0x62;

    /// <summary>
    /// Gets the wing curse option number.
    /// </summary>
    public static short WingCurse => 0x63;

    /// <summary>
    /// Gets the wing health recover option number.
    /// </summary>
    public static short WingHealthRecover => 0x64;

    /// <summary>
    /// Gets the wing2nd option number.
    /// </summary>
    public static short Wing2nd => 0x65;

    /// <summary>
    /// Gets the cape option number.
    /// </summary>
    public static short Cape => 0x66;

    /// <summary>
    /// Gets the wing3rd option number.
    /// </summary>
    public static short Wing3rd => 0x67;

    /// <summary>
    /// Gets the elite transfer skeleton ring option number.
    /// </summary>
    public static short EliteTransferSkeletonRing => 0x70;

    /// <summary>
    /// Gets the skeleton transformation ring option number.
    /// </summary>
    public static short SkeletonTransformationRing => 0x71;

    /// <summary>
    /// Gets the panda ring option number.
    /// </summary>
    public static short PandaRing => 0x72;

    /// <summary>
    /// Gets the wizard ring option number.
    /// </summary>
    public static short WizardRing => 0x73;

    /// <summary>
    /// Gets the dino option number.
    /// </summary>
    public static short Dino => 0x80;

    /// <summary>
    /// Gets the fenrir option number.
    /// </summary>
    public static short Fenrir => 0x81;

    /// <summary>
    /// Gets the dark horse option number.
    /// </summary>
    public static short Horse => 0x82;
}
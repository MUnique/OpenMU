// <copyright file="ItemSerializerHelper.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView;

using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.Persistence;

/// <summary>
/// A helper class to <see cref="ItemSerializer"/>.
/// </summary>
public static class ItemSerializerHelper
{
    /// <summary>
    /// Empty socket.
    /// </summary>
    internal const byte EmptySocket = 0xFE;

    /// <summary>
    /// No socket.
    /// </summary>
    internal const byte NoSocket = 0xFF;

    /// <summary>
    /// Maximum sockets.
    /// </summary>
    internal const int MaximumSockets = 5;

    /// <summary>
    /// Maximum socket options.
    /// </summary>
    internal const byte MaximumSocketOptions = 50;

    /// <summary>
    /// The Black Fenrir flag.
    /// </summary>
    private const byte BlackFenrirFlag = 0x01;

    /// <summary>
    /// The Blue Fenrir flag.
    /// </summary>
    private const byte BlueFenrirFlag = 0x02;

    /// <summary>
    /// The Gold Fenrir flag.
    /// </summary>
    private const byte GoldFenrirFlag = 0x04;

    /// <summary>
    /// The socket seed index offsets, where the key is the numerical value of a <see cref="SocketSubOptionType"/>
    /// and the value is the first index of this corresponding elemental seed.
    /// </summary>
    /// <remarks>
    /// Webzen decided to put every possible socket option of each elemental seed type into one big list,
    /// which may contain up to <see cref="MaximumSocketOptions"/> elements.
    /// I couldn't figure out a pattern, but found these index offsets by trial and error.
    /// Their list contains holes, so expect that index 9 doesn't define an option.
    /// </remarks>
    private static readonly byte[] SocketOptionIndexOffsets = { 0, 10, 16, 21, 29, 36 };

    /// <summary>
    /// Gets the excellent (or wing) option byte for an item.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>The byte.</returns>
    public static byte GetExcellentByte(Item item)
    {
        byte result = 0;
        var excellentOptions = item.ItemOptions.Where(o => o.ItemOption?.OptionType == ItemOptionTypes.Excellent || o.ItemOption?.OptionType == ItemOptionTypes.Wing);

        foreach (var option in excellentOptions)
        {
            result |= (byte)(1 << (option.ItemOption!.Number - 1));
        }

        return result;
    }

    /// <summary>
    /// Gets the Jewel of Harmony option byte for an item.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>The byte.</returns>
    public static byte GetHarmonyByte(Item item)
    {
        byte result = 0;
        var harmonyOption = item.ItemOptions.FirstOrDefault(o => o.ItemOption?.OptionType == ItemOptionTypes.HarmonyOption);
        if (harmonyOption?.ItemOption is not null)
        {
            result = (byte)(harmonyOption.ItemOption.Number << 4);
            result |= (byte)harmonyOption.Level;
        }

        return result;
    }

    /// <summary>
    /// Gets the socket option byte for an item.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>The byte.</returns>
    public static byte GetSocketBonusByte(Item item)
    {
        if (item.SocketCount == 0)
        {
            return 0;
        }

        var bonusOption = item.ItemOptions.FirstOrDefault(o => o.ItemOption?.OptionType == ItemOptionTypes.SocketBonusOption);
        if (bonusOption?.ItemOption != null)
        {
            return (byte)bonusOption.ItemOption.Number;
        }

        return 0xFF;
    }

    /// <summary>
    /// Sets the socket bytes for an item.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="item">The item.</param>
    public static void SetSocketBytes(Span<byte> target, Item item)
    {
        for (int i = 0; i < target.Length; i++)
        {
            target[i] = i < item.SocketCount ? GetSocketByte(i) : NoSocket;
        }

        byte GetSocketByte(int socketSlot)
        {
            var optionLink = item.ItemOptions.FirstOrDefault(o => o.ItemOption?.OptionType == ItemOptionTypes.SocketOption && o.Index == socketSlot);
            if (optionLink is null)
            {
                return EmptySocket;
            }

            var sphereLevel = optionLink.Level;
            var elementType = optionLink.ItemOption!.SubOptionType;
            var elementOption = optionLink.ItemOption.Number;
            var optionIndex = SocketOptionIndexOffsets[elementType] + elementOption;

            return (byte)((sphereLevel * MaximumSocketOptions) + optionIndex);
        }
    }

    /// <summary>
    /// Reads the Fenrir options for an item.
    /// </summary>
    /// <param name="fenrirByte">The fenrir byte.</param>
    /// <param name="persistenceContext">The persistence context.</param>
    /// <param name="item">The item.</param>
    public static void ReadFenrirOptions(byte fenrirByte, IContext persistenceContext, Item item)
    {
        if (fenrirByte == 0)
        {
            return;
        }

        AddFenrirOptionIfFlagSet(fenrirByte, BlackFenrirFlag, ItemOptionTypes.BlackFenrir, persistenceContext, item);
        AddFenrirOptionIfFlagSet(fenrirByte, BlueFenrirFlag, ItemOptionTypes.BlueFenrir, persistenceContext, item);
        AddFenrirOptionIfFlagSet(fenrirByte, GoldFenrirFlag, ItemOptionTypes.GoldFenrir, persistenceContext, item);
    }

    /// <summary>
    /// Reads the sockets for an item.
    /// </summary>
    /// <param name="socketBytes">The socket byte.</param>
    /// <param name="persistenceContext">The persistence context.</param>
    /// <param name="item">The item.</param>
    public static void ReadSockets(Span<byte> socketBytes, IContext persistenceContext, Item item)
    {
        if (item.Definition!.MaximumSockets == 0)
        {
            return;
        }

        var numberOfSockets = 0;
        for (int i = 0; i < socketBytes.Length; i++)
        {
            var socketByte = socketBytes[i];
            if (socketByte == NoSocket)
            {
                continue;
            }

            numberOfSockets++;
            if (socketByte == EmptySocket)
            {
                continue;
            }

            var sphereLevel = socketByte / MaximumSocketOptions;
            var optionIndex = socketByte % MaximumSocketOptions;
            var indexOffset = SocketOptionIndexOffsets.Last(offset => offset <= optionIndex);
            var elementType = Array.IndexOf(SocketOptionIndexOffsets, indexOffset);
            var optionNumber = optionIndex - indexOffset;

            var socketOption = item.Definition.PossibleItemOptions
                                   .SelectMany(o => o.PossibleOptions
                                       .Where(p => p.OptionType == ItemOptionTypes.SocketOption
                                                   && p.SubOptionType == elementType
                                                   && p.Number == optionNumber))
                                   .FirstOrDefault()
                               ?? throw new ArgumentException($"The socket option {socketByte} was set, but the option is not defined as possible option in the item definition ({item.Definition.Number}, {item.Definition.Group}).");
            var optionLink = persistenceContext.CreateNew<ItemOptionLink>();
            optionLink.ItemOption = socketOption;
            optionLink.Level = sphereLevel;
            optionLink.Index = i;
            item.ItemOptions.Add(optionLink);
        }

        item.SocketCount = numberOfSockets;
    }

    /// <summary>
    /// Reads the socket bonus for an item.
    /// </summary>
    /// <param name="socketBonusByte">The socket bonus byte.</param>
    /// <param name="persistenceContext">The persistence context.</param>
    /// <param name="item">The item.</param>
    public static void ReadSocketBonus(byte socketBonusByte, IContext persistenceContext, Item item)
    {
        if (socketBonusByte == 0 || socketBonusByte == 0xFF || socketBonusByte == 0xF)
        {
            return;
        }

        var bonusOption = item.Definition!.PossibleItemOptions
            .SelectMany(o => o.PossibleOptions
                .Where(p => p.OptionType == ItemOptionTypes.SocketBonusOption && p.Number == socketBonusByte))
            .FirstOrDefault();
        var optionLink = persistenceContext.CreateNew<ItemOptionLink>();
        optionLink.ItemOption = bonusOption;
        item.ItemOptions.Add(optionLink);
    }

    /// <summary>
    /// Adds the Jewel of Harmony option to an item.
    /// </summary>
    /// <param name="harmonyByte">The harmony option byte.</param>
    /// <param name="persistenceContext">The persistence context.</param>
    /// <param name="item">The item.</param>
    public static void AddHarmonyOption(byte harmonyByte, IContext persistenceContext, Item item)
    {
        if (harmonyByte == 0)
        {
            return;
        }

        var level = harmonyByte & 0x0F;
        var optionNumber = (harmonyByte & 0xF0) >> 4;
        var harmonyOption = item.Definition!.PossibleItemOptions
                                .SelectMany(o => o.PossibleOptions.Where(p =>
                                    p.OptionType == ItemOptionTypes.HarmonyOption && p.Number == optionNumber))
                                .FirstOrDefault()
                            ?? throw new ArgumentException(
                                $"The harmony option {optionNumber} was set, but the option is not defined as possible option in the item definition ({item.Definition.Number}, {item.Definition.Group}).");
        var optionLink = persistenceContext.CreateNew<ItemOptionLink>();
        optionLink.ItemOption = harmonyOption;
        optionLink.Level = level;
        item.ItemOptions.Add(optionLink);
    }

    /// <summary>
    /// Adds the level 380 option to an item.
    /// </summary>
    /// <param name="persistenceContext">The persistence context.</param>
    /// <param name="item">The item.</param>
    public static void AddLevel380Option(IContext persistenceContext, Item item)
    {
        if (!item.Definition!.PossibleItemOptions.Any(o => o.PossibleOptions.Any(i => i.OptionType == ItemOptionTypes.GuardianOption)))
        {
            throw new ArgumentException($"The lvl380 option flag was set, but the option is not defined as possible option in the item definition ({item.Definition.Number}, {item.Definition.Group}).");
        }

        var guardianOptions = item.Definition.PossibleItemOptions
            .SelectMany(o => o.PossibleOptions.Where(i => i.OptionType == ItemOptionTypes.GuardianOption));
        foreach (var option in guardianOptions)
        {
            var optionLink = persistenceContext.CreateNew<ItemOptionLink>();
            optionLink.ItemOption = option;
            item.ItemOptions.Add(optionLink);
        }
    }

    /// <summary>
    /// Gets the Fenrir byte from an item.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>The byte.</returns>
    public static byte GetFenrirByte(Item item)
    {
        byte result = 0;
        if (item.ItemOptions.Any(o => o.ItemOption?.OptionType == ItemOptionTypes.BlackFenrir))
        {
            result |= BlackFenrirFlag;
        }

        if (item.ItemOptions.Any(o => o.ItemOption?.OptionType == ItemOptionTypes.BlueFenrir))
        {
            result |= BlueFenrirFlag;
        }

        if (item.ItemOptions.Any(o => o.ItemOption?.OptionType == ItemOptionTypes.GoldFenrir))
        {
            result |= GoldFenrirFlag;
        }

        return result;
    }

    /// <summary>
    /// Adds the luck option to an item.
    /// </summary>
    /// <param name="persistenceContext">The persistence context.</param>
    /// <param name="item">The item.</param>
    /// <exception cref="ArgumentException">If the luck option is not part of the item's <see cref="ItemDefinition"/>.</exception>
    public static void AddLuckOption(IContext persistenceContext, Item item)
    {
        var luckOption = item.Definition!.PossibleItemOptions
                             .SelectMany(o => o.PossibleOptions.Where(i => i.OptionType == ItemOptionTypes.Luck))
                             .FirstOrDefault()
                         ?? throw new ArgumentException($"The luck flag was set, but luck option is not defined as possible option in the item definition ({item.Definition.Number}, {item.Definition.Group}).");
        var optionLink = persistenceContext.CreateNew<ItemOptionLink>();
        optionLink.ItemOption = luckOption;
        item.ItemOptions.Add(optionLink);
    }

    /// <summary>
    /// Reads the wing option bits of an item.
    /// </summary>
    /// <param name="wingBits">The wing bits.</param>
    /// <param name="persistenceContext">The persistence context.</param>
    /// <param name="item">The item.</param>
    public static void ReadWingOptionBits(int wingBits, IContext persistenceContext, Item item)
    {
        var wingOptionDefinition = item.Definition!.PossibleItemOptions.First(o =>
            o.PossibleOptions.Any(i => i.OptionType == ItemOptionTypes.Wing));
        foreach (var wingOption in wingOptionDefinition.PossibleOptions)
        {
            var optionMask = 1 << (wingOption.Number - 1);
            if ((wingBits & optionMask) == optionMask)
            {
                var optionLink = persistenceContext.CreateNew<ItemOptionLink>();
                optionLink.ItemOption = wingOption;
                item.ItemOptions.Add(optionLink);
            }
        }
    }

    /// <summary>
    /// Reads the excellent option bits of an item.
    /// </summary>
    /// <param name="excellentBits">The excellent bits.</param>
    /// <param name="persistenceContext">The persistence context.</param>
    /// <param name="item">The item.</param>
    public static void ReadExcellentOptionBits(int excellentBits, IContext persistenceContext, Item item)
    {
        var excellentOptionDefinition = item.Definition!.PossibleItemOptions.First(o =>
            o.PossibleOptions.Any(i => i.OptionType == ItemOptionTypes.Excellent));
        foreach (var excellentOption in excellentOptionDefinition.PossibleOptions)
        {
            var optionMask = 1 << (excellentOption.Number - 1);
            if ((excellentBits & optionMask) == optionMask)
            {
                var optionLink = persistenceContext.CreateNew<ItemOptionLink>();
                optionLink.ItemOption = excellentOption;
                item.ItemOptions.Add(optionLink);
            }
        }
    }

    /// <summary>
    /// Adds a "normal" option to an item.
    /// </summary>
    /// <param name="optionNumber">The option number.</param>
    /// <param name="optionLevel">The option level.</param>
    /// <param name="persistenceContext">The persistence context.</param>
    /// <param name="item">The item.</param>
    /// <exception cref="ArgumentException">If the "normal" option is not part of the item's <see cref="ItemDefinition"/>.</exception>
    public static void AddNormalOption(int optionNumber, int optionLevel, IContext persistenceContext, Item item)
    {
        if (optionLevel == 0)
        {
            return;
        }

        var option = item.Definition?.PossibleItemOptions.SelectMany(o => o.PossibleOptions.Where(i => i.OptionType == ItemOptionTypes.Option && i.Number == optionNumber))
                         .FirstOrDefault()
                     ?? throw new ArgumentException($"The option with level {optionLevel} and number {optionNumber} is not defined as possible option in the item definition ({item.Definition?.Number}, {item.Definition?.Group}).");
        var optionLink = persistenceContext.CreateNew<ItemOptionLink>();
        optionLink.ItemOption = option;
        optionLink.Level = optionLevel;
        item.ItemOptions.Add(optionLink);
    }

    /// <summary>
    /// Adds an ancient option to an item.
    /// </summary>
    /// <param name="setDiscriminator">The set discriminator number.</param>
    /// <param name="bonusLevel">The ancient bonus level.</param>
    /// <param name="persistenceContext">The persistence context.</param>
    /// <param name="item">The item.</param>
    /// <exception cref="ArgumentException">If the set discriminator number is ambiguous with regards to <see cref="ItemDefinition.PossibleItemOptions"/>.</exception>
    public static void AddAncientOption(int setDiscriminator, int bonusLevel, IContext persistenceContext, Item item)
    {
        var ancientSets = item.Definition!.PossibleItemSetGroups
            .Where(set => set.Options?.PossibleOptions.Any(o => o.OptionType == ItemOptionTypes.AncientOption) ?? false)
            .SelectMany(i => i.Items).Where(i => i.ItemDefinition == item.Definition)
            .Where(set => set.AncientSetDiscriminator == setDiscriminator).ToList();
        if (ancientSets.Count > 1)
        {
            throw new ArgumentException($"Ambiguous ancient set discriminator: {ancientSets.Count} sets with discriminator {setDiscriminator} found for item definition ({item.Definition.Number}, {item.Definition.Group}).");
        }

        var itemOfSet = ancientSets.FirstOrDefault()
                        ?? throw new ArgumentException($"Couldn't find ancient set (discriminator {setDiscriminator}) for item ({item.Definition.Number}, {item.Definition.Group}).");
        item.ItemSetGroups.Add(itemOfSet);
        if (bonusLevel > 0)
        {
            var optionLink = persistenceContext.CreateNew<ItemOptionLink>();
            optionLink.ItemOption = itemOfSet.BonusOption;
            optionLink.Level = bonusLevel;
            item.ItemOptions.Add(optionLink);
        }
    }

    private static void AddFenrirOptionIfFlagSet(byte fenrirByte, byte fenrirFlag, ItemOptionType fenrirOptionType, IContext persistenceContext, Item item)
    {
        var isFlagSet = (fenrirByte & fenrirFlag) == fenrirFlag;
        if (!isFlagSet)
        {
            return;
        }

        var blackOption = item.Definition!.PossibleItemOptions.FirstOrDefault(o => o.PossibleOptions.Any(p => p.OptionType == fenrirOptionType))
                          ?? throw new ArgumentException($"The fenrir flag {fenrirFlag} in {fenrirByte} was set, but the option is not defined as possible option in the item definition ({item.Definition.Number}, {item.Definition.Group}).");
        var optionLink = persistenceContext.CreateNew<ItemOptionLink>();
        optionLink.ItemOption = blackOption.PossibleOptions.First();
        item.ItemOptions.Add(optionLink);
    }
}
// <copyright file="ItemOptionTypes.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration.Items
{
    using System;

    /// <summary>
    /// Some standard option types.
    /// </summary>
    public static class ItemOptionTypes
    {
        private static ItemOptionType excellent = new ItemOptionType { Name = "Excellent Option", Id = new Guid("{6487C498-58E0-48E5-B409-35D7598313FC}"), IsVisible = true };
        private static ItemOptionType wing = new ItemOptionType { Name = "Wing Option", Id = new Guid("{55CB57A7-4FC6-47BB-9FEE-84E6C4EBCE95}") };
        private static ItemOptionType luck = new ItemOptionType { Name = "Luck (Critical Damage Chance 10%)", Id = new Guid("{3E3E9BE8-4E16-4F27-A7CF-986D48454D76}") };
        private static ItemOptionType option = new ItemOptionType { Name = "Option", Id = new Guid("{F193F91E-86D7-4456-ADD8-A3667E731303}") };
        private static ItemOptionType harmonyOption = new ItemOptionType { Name = "Jewel of Harmony Option", Id = new Guid("{0CA234F0-4A0F-4FA1-8E07-CFB89C1EC94F}") };
        private static ItemOptionType ancientOption = new ItemOptionType { Name = "Ancient Option", Id = new Guid("{436D820F-6D50-429D-AF63-BB0F59567DD1}"), IsVisible = true };
        private static ItemOptionType ancientBonus = new ItemOptionType { Name = "Ancient Bonus Option", Id = new Guid("{5E2C10EF-E580-48D5-A48B-0FFCD0678966}") };
        private static ItemOptionType level380Option = new ItemOptionType { Name = "Level 380 Option", Id = new Guid("{4AA95715-1ED3-453D-8D1D-093B281416CA}") };
        private static ItemOptionType socketOption = new ItemOptionType { Name = "Socket Option", Id = new Guid("{AAB309D3-CD97-4F77-AE1B-E9F904102502}") };

        /// <summary>
        /// Gets the excellent item option type.
        /// </summary>
        public static ItemOptionType Excellent
        {
            get { return excellent; }
        }

        /// <summary>
        /// Gets the wing option type.
        /// </summary>
        public static ItemOptionType Wing
        {
            get { return wing; }
        }

        /// <summary>
        /// Gets the luck option type.
        /// </summary>
        public static ItemOptionType Luck
        {
            get { return luck; }
        }

        /// <summary>
        /// Gets the standard option option type.
        /// </summary>
        public static ItemOptionType Option
        {
            get { return option; }
        }

        /// <summary>
        /// Gets the harmony option type.
        /// </summary>
        public static ItemOptionType HarmonyOption
        {
            get { return harmonyOption; }
        }

        /// <summary>
        /// Gets the ancient option type.
        /// </summary>
        public static ItemOptionType AncientOption
        {
            get { return ancientOption; }
        }

        /// <summary>
        /// Gets the ancient bonus option type.
        /// </summary>
        public static ItemOptionType AncientBonus
        {
            get { return ancientBonus; }
        }

        /// <summary>
        /// Gets the level380 option type.
        /// </summary>
        public static ItemOptionType Level380Option
        {
            get { return level380Option; }
        }

        /// <summary>
        /// Gets the socket option type.
        /// </summary>
        public static ItemOptionType SocketOption
        {
            get { return socketOption; }
        }
    }
}
// <copyright file="Settings.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ChatServer.ExDbConnector
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// A class which reads settings from a file.
    /// Line Format:
    /// [Key]=[Value]
    /// Line comments can be added by starting with "#".
    /// </summary>
    internal class Settings
    {
        private readonly IDictionary<string, string> settingsDictionary = new Dictionary<string, string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Settings"/> class.
        /// Reads the file contents in, if the file is available.
        /// </summary>
        /// <param name="file">The file.</param>
        public Settings(string file)
        {
            if (!File.Exists(file))
            {
                return;
            }

            foreach (var line in File.ReadAllLines(file))
            {
                var elements = line.Split('=');
                if (elements.Length > 1
                    && !elements[0].StartsWith("#", StringComparison.InvariantCulture)
                    && !this.settingsDictionary.ContainsKey(elements[0]))
                {
                    this.settingsDictionary.Add(elements[0], elements[1]);
                }
            }
        }

        /// <summary>
        /// Gets the configured chat server listener port.
        /// </summary>
        public int? ChatServerListenerPort
        {
            get
            {
                if (this["ChatServerListenerPort"] != null && int.TryParse(this["ChatServerListenerPort"], out var result))
                {
                    return result;
                }

                return default;
            }
        }

        /// <summary>
        /// Gets the configured exDb server port.
        /// </summary>
        public int? ExDbPort
        {
            get
            {
                if (this["ExDbPort"] != null)
                {
                    if (int.TryParse(this["ExDbPort"], out int result))
                    {
                        return result;
                    }

                    return default;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the configured exDb server host.
        /// </summary>
        public string? ExDbHost => this["ExDbHost"];

        /// <summary>
        /// Gets the configured xor32 key.
        /// </summary>
        public byte[]? Xor32Key
        {
            get
            {
                if (this["Xor32Key"] != null)
                {
                    var customXor32KeyList = new List<byte>();
                    var keyAsString = this["Xor32Key"];
                    if (keyAsString is not null)
                    {
                        var bytesAsString = keyAsString.Split(' ');
                        foreach (var byteString in bytesAsString)
                        {
                            customXor32KeyList.Add(byte.Parse(byteString, System.Globalization.NumberStyles.HexNumber));
                        }
                    }

                    return customXor32KeyList.ToArray();
                }

                return null;
            }
        }

        private string? this[string key]
        {
            get
            {
                this.settingsDictionary.TryGetValue(key, out var value);
                return value;
            }
        }
    }
}

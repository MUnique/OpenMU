// <copyright file="BinaryAsHexJsonConverter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.Json
{
    using System;
    using Newtonsoft.Json;

    /// <summary>
    /// This converter converts a hex string to a byte array.
    /// Newtonsoft.Json expects a base64 string, but postgres delivers us a hex string,
    /// so this converter is needed.
    /// </summary>
    public class BinaryAsHexJsonConverter : JsonConverter
    {
        /// <summary>
        /// The prefix of a byte array string, provided by postgres.
        /// </summary>
        private const string ByteArrayPrefix = @"\x";

        /// <summary>
        /// A character to value mapping table for faster hex string parsing. Each character (0-9a-fA-F)
        /// has it's corresponding value of 0-15. All other characters are initialized with <see cref="int.MaxValue"/>.
        /// </summary>
        private readonly int[] characterToValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryAsHexJsonConverter"/> class.
        /// </summary>
        public BinaryAsHexJsonConverter()
        {
            this.characterToValue = new int[byte.MaxValue + 1];
            for (int i = 0; i < this.characterToValue.Length; i++)
            {
                this.characterToValue[i] = int.MaxValue;
            }

            for (byte i = 0; i < 10; i++)
            {
                this.characterToValue['0' + i] = i;
            }

            for (byte i = 0xA; i < 16; i++)
            {
                this.characterToValue['a' + i - 0xA] = i;
                this.characterToValue['A' + i - 0xA] = i;
            }
        }

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            var array = value as byte[];
            if (array is null)
            {
                writer.WriteNull();
                return;
            }

            var arrayString = ByteArrayPrefix + BitConverter.ToString(array)
                .Replace("-", string.Empty, StringComparison.InvariantCulture)
                .ToLowerInvariant();
            writer.WriteValue(arrayString);
        }

        /// <inheritdoc />
        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            if (reader.TokenType == JsonToken.String)
            {
                var prefixSize = ByteArrayPrefix.Length;
                if (reader.Value is string value)
                {
                    var data = new byte[(value.Length - prefixSize) / 2];
                    for (var i = 0; i < data.Length; i++)
                    {
                        var index = prefixSize + (i * 2);
                        int highNibble = this.ParseCharacter(value[index]);
                        int lowNibble = this.ParseCharacter(value[index + 1]);
                        data[i] = (byte)((highNibble << 4) | lowNibble);
                    }

                    return data;
                }

                return default(byte[]);
            }

            throw new JsonSerializationException($"Unexpected token parsing binary. Expected String, got {reader.TokenType}.");
        }

        /// <inheritdoc />
        public override bool CanConvert(Type objectType) => objectType == typeof(byte[]);

        private int ParseCharacter(char character)
        {
            var value = this.characterToValue[character];
            if (value == int.MaxValue)
            {
                throw new ArgumentException($"invalid character: {character}", nameof(character));
            }

            return value;
        }
    }
}
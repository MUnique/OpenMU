// <copyright file="LocalizedStringConverter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.Extensions;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// A value converter for <see cref="LocalizedString" /> which stores only the underlying string value.
/// </summary>
internal class LocalizedStringConverter : ValueConverter<LocalizedString, string>
{
    /// <summary>
    /// Gets the singleton instance of the <see cref="LocalizedStringConverter"/>.
    /// </summary>
    public static LocalizedStringConverter Instance { get; } = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalizedStringConverter" /> class.
    /// </summary>
    private LocalizedStringConverter()

        : base(value => value.Value ?? string.Empty, value => new LocalizedString(value))
    {
    }
}
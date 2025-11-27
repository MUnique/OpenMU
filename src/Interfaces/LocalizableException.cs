// <copyright file="LocalizableException.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces;

using System.Globalization;
using System.Resources;

/// <summary>
/// Represents an exception which wraps a base exception and provides a localized message
/// based on resources defined by <typeparamref name="TResources"/>.
/// The specified <see cref="LocalizableExceptionBase.ResourceKey"/> together with optional <see cref="LocalizableExceptionBase.FormatArguments"/>
/// is used to format the localized text at runtime.
/// </summary>
/// <typeparam name="TException">The type of the original exception which is wrapped.</typeparam>
/// <typeparam name="TResources">
/// The resources class (designer generated from a .resx file) which must expose a public static
/// <c>ResourceManager</c> property used to resolve localized strings.
/// </typeparam>
/// <remarks>
/// The resource manager of <typeparamref name="TResources"/> is resolved lazily and cached across
/// all localizable exceptions through <see cref="LocalizableExceptionBase"/>.
/// If no resource value exists for <see cref="LocalizableExceptionBase.ResourceKey"/>, the key itself is returned.
/// </remarks>
public class LocalizableException<TException, TResources> : LocalizableExceptionBase
    where TException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LocalizableException{TException, TResources}"/> class.
    /// </summary>
    /// <param name="baseException">The base exception.</param>
    /// <param name="resourceKey">The resource key.</param>
    /// <param name="formatArguments">The format arguments.</param>
    public LocalizableException(TException baseException, string resourceKey, params ReadOnlySpan<object?> formatArguments)
        : base(baseException.Message, baseException, resourceKey, formatArguments)
    {
    }

    /// <summary>
    /// Gets the inner exception.
    /// </summary>
    public new TException InnerException => (TException)base.InnerException!;

    /// <summary>
    /// Gets the localized message for this exception using the specified <paramref name="cultureInfo"/>.
    /// </summary>
    /// <param name="cultureInfo">The culture for which the message should be localized.</param>
    /// <returns>
    /// The localized message if the <see cref="ResourceManager"/> contains the specified <see cref="ResourceKey"/>;
    /// otherwise the <see cref="ResourceKey"/> itself. If <see cref="FormatArguments"/> are specified,
    /// they are applied using <see cref="string.Format(IFormatProvider,string,object?[])"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="cultureInfo"/> is <c>null</c>.</exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the resource type <typeparamref name="TResources"/> does not expose a public static
    /// <c>ResourceManager</c> property.
    /// </exception>
    public string GetLocalizedMessage(CultureInfo cultureInfo)
    {
        ArgumentNullException.ThrowIfNull(cultureInfo);

        var resourceManager = ResolveResourceManager(typeof(TResources));
        var localizedString = resourceManager.GetString(this.ResourceKey, cultureInfo) ?? this.ResourceKey;
        if (this.FormatArguments.Length > 0)
        {
            return string.Format(cultureInfo, localizedString, this.FormatArguments);
        }

        return localizedString;
    }
}
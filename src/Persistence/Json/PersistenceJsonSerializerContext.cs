// <copyright file="PersistenceJsonSerializerContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Json;

using System.Text.Json.Serialization;
using MUnique.OpenMU.AttributeSystem;

/// <summary>
/// The json metadata of the types which are read by the reference resolving converters, but which are
/// not persistent types themselves, so they're not provided by the <see cref="ReferenceResolvingTypeInfoResolver"/>.
/// </summary>
[JsonSerializable(typeof(SimpleElement))]
internal partial class PersistenceJsonSerializerContext : JsonSerializerContext
{
}

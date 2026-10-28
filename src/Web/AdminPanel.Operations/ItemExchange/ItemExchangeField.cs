// <copyright file="ItemExchangeField.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations.ItemExchange;

using System.Text.Json;
using MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// An item field of the exchange file.
/// </summary>
/// <param name="Key">The client's key of the field, as the file's <c>fields</c> list names it.</param>
/// <param name="Write">Writes the field of an item; it writes nothing when the value is the client's default.</param>
internal sealed record ItemExchangeField(string Key, Action<Utf8JsonWriter, ItemDefinition> Write);

// <copyright file="CharacterDropItemGroup.Generated.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

//------------------------------------------------------------------------------
// <auto-generated>
//     This source code was auto-generated by a roslyn code generator.
// </auto-generated>
//------------------------------------------------------------------------------

// ReSharper disable All

namespace MUnique.OpenMU.Persistence.EntityFramework.Model;

using System.ComponentModel.DataAnnotations.Schema;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.EntityFramework;

[Table(nameof(CharacterDropItemGroup), Schema = SchemaNames.AccountData)]
internal partial class CharacterDropItemGroup
{
    public Guid CharacterId { get; set; }
    public Character Character { get; set; }

    public Guid DropItemGroupId { get; set; }
    public DropItemGroup DropItemGroup { get; set; }
}

internal partial class Character
{
    public ICollection<CharacterDropItemGroup> JoinedDropItemGroups { get; } = new EntityFramework.List<CharacterDropItemGroup>();
}

// <copyright file="DropGroupAssignmentList.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Components.ItemDrops;

using System.Collections;
using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// A list of owners (e.g. maps or monsters) to which a <see cref="DropItemGroup"/> is assigned.
/// Adding or removing an owner adds or removes the drop item group to or from the
/// drop item groups of the owner.
/// </summary>
/// <typeparam name="TOwner">The type of the owner.</typeparam>
public sealed class DropGroupAssignmentList<TOwner> : IList<TOwner>
    where TOwner : class
{
    private readonly DropItemGroup _group;
    private readonly Func<TOwner, ICollection<DropItemGroup>> _groupsOfOwner;
    private readonly List<TOwner> _owners;

    /// <summary>
    /// Initializes a new instance of the <see cref="DropGroupAssignmentList{TOwner}"/> class.
    /// </summary>
    /// <param name="group">The drop item group.</param>
    /// <param name="allOwners">All possible owners.</param>
    /// <param name="groupsOfOwner">The function to get the drop item groups of an owner.</param>
    public DropGroupAssignmentList(DropItemGroup group, IEnumerable<TOwner> allOwners, Func<TOwner, ICollection<DropItemGroup>> groupsOfOwner)
    {
        this._group = group;
        this._groupsOfOwner = groupsOfOwner;
        this._owners = allOwners.Where(owner => groupsOfOwner(owner).Contains(group)).ToList();
    }

    /// <inheritdoc />
    public int Count => this._owners.Count;

    /// <inheritdoc />
    public bool IsReadOnly => false;

    /// <inheritdoc />
    public TOwner this[int index]
    {
        get => this._owners[index];
        set => throw new NotSupportedException();
    }

    /// <inheritdoc />
    public IEnumerator<TOwner> GetEnumerator() => this._owners.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    /// <inheritdoc />
    public void Add(TOwner item)
    {
        if (this._owners.Contains(item))
        {
            return;
        }

        var groups = this._groupsOfOwner(item);
        if (!groups.Contains(this._group))
        {
            groups.Add(this._group);
        }

        this._owners.Add(item);
    }

    /// <inheritdoc />
    public void Clear()
    {
        foreach (var owner in this._owners.ToList())
        {
            this.Remove(owner);
        }
    }

    /// <inheritdoc />
    public bool Contains(TOwner item) => this._owners.Contains(item);

    /// <inheritdoc />
    public void CopyTo(TOwner[] array, int arrayIndex) => this._owners.CopyTo(array, arrayIndex);

    /// <inheritdoc />
    public bool Remove(TOwner item)
    {
        this._groupsOfOwner(item).Remove(this._group);
        return this._owners.Remove(item);
    }

    /// <inheritdoc />
    public int IndexOf(TOwner item) => this._owners.IndexOf(item);

    /// <inheritdoc />
    public void Insert(int index, TOwner item) => this.Add(item);

    /// <inheritdoc />
    public void RemoveAt(int index) => this.Remove(this._owners[index]);
}

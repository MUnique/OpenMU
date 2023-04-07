// <copyright file="SocketViewModel.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Components.ItemEdit;

using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Persistence;
using Nito.AsyncEx.Synchronous;

/// <summary>
/// A view model for a socket of an item.
/// </summary>
public class SocketViewModel
{
    private readonly Item _item;
    private readonly IContext _persistenceContext;
    private readonly IEnumerable<SocketOptionViewModel> _possibleOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="SocketViewModel"/> class.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="persistenceContext">The persistence context.</param>
    /// <param name="socketIndex">The socket number.</param>
    /// <param name="possibleOptions">The possible options.</param>
    public SocketViewModel(Item item, IContext persistenceContext, int socketIndex, IEnumerable<SocketOptionViewModel> possibleOptions)
    {
        this._item = item;
        this._persistenceContext = persistenceContext;
        this._possibleOptions = possibleOptions;
        this.SocketIndex = socketIndex;
    }

    /// <summary>
    /// Occurs when the <see cref="Option"/> has been changed.
    /// </summary>
    public event EventHandler? OptionChanged;

    /// <summary>
    /// Gets the socket number.
    /// </summary>
    public int SocketIndex { get; }

    /// <summary>
    /// Gets or sets the level of the socket option.
    /// </summary>
    public int Level
    {
        get => this.OptionLink?.Level ?? 0;
        set
        {
            if (this.OptionLink is { } optionLink)
            {
                optionLink.Level = value;
            }
        }
    }

    /// <summary>
    /// Gets the option link for this socket.
    /// </summary>
    public ItemOptionLink? OptionLink => this._item.ItemOptions.FirstOrDefault(o => o.ItemOption?.OptionType == ItemOptionTypes.SocketOption && o.Index == this.SocketIndex);

    /// <summary>
    /// Gets the field caption for the socket.
    /// </summary>
    public string Caption => $"Socket {this.SocketIndex + 1}:";

    /// <summary>
    /// Gets or sets the option.
    /// </summary>
    public SocketOptionViewModel? Option
    {
        get
        {
            return this._possibleOptions.FirstOrDefault(v => v.Option == this.OptionLink?.ItemOption);
        }
        set
        {
            if (this.Option?.Option == value?.Option)
            {
                return;
            }

            if (value is null)
            {
                if (this.OptionLink is { } optionLink)
                {
                    this._item.ItemOptions.Remove(optionLink);
                    this._persistenceContext.DeleteAsync(optionLink).AsTask().WaitAndUnwrapException();
                }
            }
            else
            {
                var optionLink = this.OptionLink;
                if (optionLink is null)
                {
                    optionLink = this._persistenceContext.CreateNew<ItemOptionLink>();
                    this._item.ItemOptions.Add(optionLink);
                    optionLink.Level = 1;
                }

                optionLink.ItemOption = value?.Option;
                optionLink.Index = this.SocketIndex;
            }

            this.OptionChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
// <copyright file="ItemTable.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanelBlazor.Components.Form
{
    using System;
    using System.Collections.Generic;
    using Blazored.Modal;
    using Blazored.Modal.Services;
    using Microsoft.AspNetCore.Components;
    using MUnique.OpenMU.Persistence;

    /// <summary>
    /// A component which shows a collection of <typeparamref name="TItem"/> in a table.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    public partial class ItemTable<TItem>
        where TItem : class
    {
        private bool isEditable;

        private bool isPlayerData;

        private bool isAddingSupported;

        private bool isCreatingSupported;

        private bool isStartingCollapsed;

        private bool isCollapsed;

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        [Parameter]
        public string Label { get; set; }

        private IContext Context => this.isPlayerData ? this.PlayerContext : this.CommonContext;

        /// <inheritdoc />
        protected override void OnInitialized()
        {
            base.OnInitialized();
            this.isEditable = typeof(TItem).Namespace?.StartsWith(nameof(MUnique)) ?? false;
            this.isPlayerData = !((System.Linq.Expressions.MemberExpression)this.ValueExpression.Body).Expression.Type.IsConfigurationType();
            this.isAddingSupported = (this.isPlayerData && typeof(TItem).IsConfigurationType()) || !this.isPlayerData;
            this.isCreatingSupported = (this.isPlayerData && !typeof(TItem).IsConfigurationType()) || !this.isPlayerData;
            this.isStartingCollapsed = this.Value.Count > 10;
            this.isCollapsed = this.isStartingCollapsed;
        }

        /// <inheritdoc />
        protected override bool TryParseValueFromString(string value, out ICollection<TItem> result, out string validationErrorMessage)
        {
            throw new NotImplementedException();
        }

        private void OnToggleCollapseClick()
        {
            this.isCollapsed = !this.isCollapsed;
        }

        private void OnAddClick()
        {
            void OnModalClose(ModalResult result)
            {
                if (!result.Cancelled && result.Data is TItem item)
                {
                    this.Value.Add(item);
                    this.StateHasChanged();
                }

                this.Modal.OnClose -= OnModalClose;
            }

            this.Modal.OnClose += OnModalClose;
            this.Modal.Show<ModalObjectSelection<TItem>>($"Select {typeof(TItem).Name}");
        }

        private void OnCreateClick()
        {
            var item = this.Context.CreateNew<TItem>();
            var parameters = new ModalParameters();
            parameters.Add(nameof(ModalCreateNew<TItem>.Item), item);
            var options = new ModalOptions
            {
                DisableBackgroundCancel = true,
            };

            void OnModalClose(ModalResult result)
            {
                if (result.Cancelled)
                {
                    this.Context.Delete(item);
                }
                else
                {
                    this.Value.Add(item);
                    this.Context.SaveChanges();
                    this.StateHasChanged();
                }

                this.Modal.OnClose -= OnModalClose;
            }

            this.Modal.OnClose += OnModalClose;
            this.Modal.Show<ModalCreateNew<TItem>>($"Create {typeof(TItem).Name}", parameters, options);
        }

        private void OnRemoveClick(TItem item)
        {
            this.Value.Remove(item);
            if (this.isPlayerData && !typeof(TItem).IsConfigurationType())
            {
                this.Context.Delete(item);
            }

            this.Context.SaveChanges();
        }
    }
}

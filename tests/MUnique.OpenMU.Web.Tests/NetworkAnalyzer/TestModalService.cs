// <copyright file="TestModalService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.NetworkAnalyzer;

using Microsoft.AspNetCore.Components;
using MUnique.OpenMU.Web.Shared.Components.Modal;

/// <summary>
/// A <see cref="IModalService"/> which answers every dialog without showing one.
/// </summary>
public sealed class TestModalService : IModalService
{
    private readonly bool _answer;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestModalService"/> class.
    /// </summary>
    /// <param name="answer">The answer which is given for each question.</param>
    public TestModalService(bool answer = true)
    {
        this._answer = answer;
    }

    /// <summary>
    /// Gets the titles of the dialogs which have been shown.
    /// </summary>
    public IList<string> ShownDialogs { get; } = new List<string>();

    /// <inheritdoc />
    public IModalReference Show<TComponent>(string title, ModalParameters? parameters = null, ModalOptions? options = null)
        where TComponent : class, IComponent
    {
        return this.Show(typeof(TComponent), title, parameters, options);
    }

    /// <inheritdoc />
    public IModalReference Show(Type componentType, string title, ModalParameters? parameters = null, ModalOptions? options = null)
    {
        this.ShownDialogs.Add(title);
        return new TestModalReference(ModalResult.Ok(this._answer));
    }

    private sealed class TestModalReference : IModalReference
    {
        public TestModalReference(ModalResult result)
        {
            this.Result = Task.FromResult(result);
        }

        public Task<ModalResult> Result { get; }
    }
}

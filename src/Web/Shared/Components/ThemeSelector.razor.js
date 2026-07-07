// <copyright file="ThemeSelector.razor.js" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

export function current() {
    return document.documentElement.getAttribute("data-bs-theme");
}

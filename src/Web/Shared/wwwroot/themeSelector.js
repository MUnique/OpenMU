// <copyright file="themeSelector.js" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

/**
 * Returns the active UI theme from the <html> element's data-theme attribute
 * (set by App.razor from the OpenMU.Theme cookie). Used by ThemeSelector to
 * hydrate its toggle state after the interactive Blazor circuit takes over,
 * because the cascading HttpContext is null in interactive renders.
 *
 * Lives in wwwroot/ (not colocated as ThemeSelector.razor.js) because
 * the *.razor.js colocation static-web-asset publishing is broken in this
 * project's build pipeline; wwwroot/ is the reliable path for static assets.
 */
export function current() {
    return document.documentElement.getAttribute("data-theme");
}

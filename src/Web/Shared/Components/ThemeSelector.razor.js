// <copyright file="ThemeSelector.razor.js" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

const cookieMaxAgeSeconds = 60 * 60 * 24 * 365;

export function current() {
    return document.documentElement.getAttribute("data-theme-preference");
}

export function set(cookieName, theme) {
    document.cookie = `${cookieName}=${encodeURIComponent(theme)}; path=/; max-age=${cookieMaxAgeSeconds}; samesite=lax`;
    document.documentElement.setAttribute("data-theme-preference", theme);
    window.dispatchEvent(new Event("openmu-theme-changed"));
}

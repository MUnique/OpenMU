// <copyright file="theme.js" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

// Applies the selected UI theme to the "data-bs-theme" attribute of the document.
// It's loaded in the head of the page, so that the "auto" theme is resolved before the page is painted,
// and it follows changes of the color scheme of the operating system while the page is open.
(function () {
    const root = document.documentElement;
    const darkQuery = window.matchMedia("(prefers-color-scheme: dark)");

    function apply() {
        const preference = root.getAttribute("data-theme-preference");
        const theme = preference === "dark" || (preference === "auto" && darkQuery.matches)
            ? "dark"
            : "light";
        root.setAttribute("data-bs-theme", theme);
    }

    apply();
    darkQuery.addEventListener("change", apply);
    window.addEventListener("openmu-theme-changed", apply);
}());

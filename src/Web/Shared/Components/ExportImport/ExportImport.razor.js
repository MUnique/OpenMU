// <copyright file="ExportImport.razor.js" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

/**
 * Triggers a click on the file input of the export/import component.
 * @param {string} id - The id of the file input.
 */
export function triggerFileInput(id) {
    document.getElementById(id)?.click();
}

/**
 * Downloads a base64-encoded byte array as a file via a temporary anchor element.
 * @param {string} fileName - The suggested download file name.
 * @param {string} base64 - The base64-encoded file content.
 */
export function downloadFile(fileName, base64) {
    const bytes = Uint8Array.from(atob(base64), (character) => character.charCodeAt(0));
    const blob = new Blob([bytes], { type: "application/json" });
    const url = URL.createObjectURL(blob);
    const anchor = document.createElement("a");
    anchor.href = url;
    anchor.download = fileName;
    document.body.appendChild(anchor);
    anchor.click();
    document.body.removeChild(anchor);
    URL.revokeObjectURL(url);
}

/**
 * Clears the selected file of the file input, so that selecting the same file again raises a change event,
 * e.g. after the file has been corrected.
 * @param {string} id - The id of the file input.
 */
export function resetFileInput(id) {
    const input = document.getElementById(id);
    if (input) {
        input.value = "";
    }
}

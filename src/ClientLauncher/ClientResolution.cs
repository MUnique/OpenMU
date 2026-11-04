// <copyright file="ClientResolution.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ClientLauncher;

/// <summary>
/// The available mu online client screen resolutions.
/// </summary>
public class ClientResolution
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ClientResolution"/> class.
    /// </summary>
    public ClientResolution()
    {
        this.Index = 0;
        this.Caption = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClientResolution"/> class.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <param name="caption">The caption.</param>
    public ClientResolution(int index, string caption)
    {
        this.Index = index;
        this.Caption = caption;
    }

    /// <summary>
    /// Gets the index.
    /// </summary>
    public int Index { get; init; }

    /// <summary>
    /// Gets the caption.
    /// </summary>
    public string Caption { get; init; }
}

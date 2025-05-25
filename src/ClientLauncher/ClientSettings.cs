// <copyright file="ClientSettings.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ClientLauncher;

using System.Runtime.Versioning;
using Microsoft.Win32;

/// <summary>
/// This class allows to read and write the game client settings in the registry.
/// </summary>
internal class ClientSettings
{
    private const string DefaultLanguage = "Eng";

    /// <summary>
    /// Gets or sets the client color depth.
    /// </summary>
    public ClientColorDepth ClientColorDepth { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is music enabled.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is music enabled; otherwise, <c>false</c>.
    /// </value>
    public bool IsMusicEnabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is sound enabled.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is sound enabled; otherwise, <c>false</c>.
    /// </value>
    public bool IsSoundEnabled { get; set; }

    /// <summary>
    /// Gets or sets the volume level.
    /// </summary>
    public int VolumeLevel { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is window mode active.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is window mode active; otherwise, <c>false</c>.
    /// </value>
    public bool IsWindowModeActive { get; set; }

    /// <summary>
    /// Gets or sets the resolution.
    /// </summary>
    public int ResolutionIndex { get; set; }

    /// <summary>
    /// Gets or sets the language selection.
    /// </summary>
    public ClientLanguage LangSelection { get; set; }

    /// <summary>
    /// Loads the settings from the windows registry.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public void Load()
    {
        using var currentUserKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32);
        using var key = currentUserKey.CreateSubKey(@"SOFTWARE\WebZen\Mu\Config");
        this.ClientColorDepth = (ClientColorDepth)(key.GetValue("ColorDepth") ?? 0);
        this.IsMusicEnabled = (int)(key.GetValue("MusicOnOff") ?? 0) == 1;
        this.IsSoundEnabled = (int)(key.GetValue("SoundOnOff") ?? 0) == 1;
        this.VolumeLevel = (int)(key.GetValue("VolumeLevel") ?? 0);
        this.IsWindowModeActive = (int)(key.GetValue("WindowMode") ?? 0) == 1;
        this.ResolutionIndex = (int)(key.GetValue("Resolution") ?? 0);
        this.LangSelection = ((string?)key.GetValue("LangSelection") ?? DefaultLanguage).GetLanguage();
    }

    /// <summary>
    /// Saves the settings at the windows registry.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public void Save()
    {
        using var currentUserKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32);
        using var key = currentUserKey.CreateSubKey(@"SOFTWARE\WebZen\Mu\Config");
        key.SetValue("ColorDepth", this.ClientColorDepth, RegistryValueKind.DWord);
        key.SetValue("MusicOnOff", this.IsMusicEnabled, RegistryValueKind.DWord);
        key.SetValue("SoundOnOff", this.IsSoundEnabled, RegistryValueKind.DWord);
        key.SetValue("VolumeLevel", this.VolumeLevel, RegistryValueKind.DWord);
        key.SetValue("WindowMode", this.IsWindowModeActive, RegistryValueKind.DWord);
        key.SetValue("Resolution", this.ResolutionIndex, RegistryValueKind.DWord);
        key.SetValue("LangSelection", this.LangSelection.GetString() ?? DefaultLanguage, RegistryValueKind.String);
    }
}
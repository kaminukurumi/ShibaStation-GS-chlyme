// SPDX-FileCopyrightText: 2024 ScarKy0 <106310278+ScarKy0@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.Audio;
using Robust.Shared.GameStates;

namespace Content.Shared.Intellicard;

/// <summary>
/// Allows this entity to download the station AI onto an AiHolderComponent.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class IntellicardComponent : Component
{
    /// <summary>
    /// The duration it takes to download the AI from an AiHolder.
    /// </summary>
    [DataField, AutoNetworkedField]
    public int DownloadTime = 15;

    /// <summary>
    /// The duration it takes to upload the AI to an AiHolder.
    /// </summary>
    [DataField, AutoNetworkedField]
    public int UploadTime = 3;

    /// <summary>
    /// The sound that plays for the AI
    /// when they are being downloaded
    /// </summary>
    [DataField, AutoNetworkedField]
    public SoundSpecifier? WarningSound = new SoundPathSpecifier("/Audio/Misc/notice2.ogg");

    /// <summary>
    /// The delay before allowing the warning to play again in seconds.
    /// </summary>
    [DataField, AutoNetworkedField]
    public TimeSpan WarningDelay = TimeSpan.FromSeconds(8);

    [ViewVariables]
    public TimeSpan NextWarningAllowed = TimeSpan.Zero;
}
// SPDX-FileCopyrightText: 2023 deltanedas <39013340+deltanedas@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 deltanedas <@deltanedas:kde.org>
//
// SPDX-License-Identifier: MIT

using Robust.Shared.Audio;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Server.IgnitionSource;

/// <summary>
/// Ignites for a certain length of time when triggered.
/// Requires <see cref="IgnitionSourceComponent"/> along with triggering components.
/// </summary>
[RegisterComponent, Access(typeof(IgniteOnTriggerSystem))]
public sealed partial class IgniteOnTriggerComponent : Component
{
    /// <summary>
    /// Once ignited, the time it will unignite at.
    /// </summary>
    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer)), ViewVariables(VVAccess.ReadWrite)]
    public TimeSpan IgnitedUntil = TimeSpan.Zero;

    /// <summary>
    /// How long the ignition source is active for after triggering.
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public TimeSpan IgnitedTime = TimeSpan.FromSeconds(0.5);

    /// <summary>
    /// Sound to play when igniting.
    /// </summary>
    [DataField]
    public SoundSpecifier IgniteSound = new SoundCollectionSpecifier("WelderOn");
}
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Misandry <mary@thughunt.ing>
// SPDX-FileCopyrightText: 2025 gus <august.eymann@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Goobstation.Common.MartialArts;


/// <summary>
/// Tracks when an entity's breathing is blocked through Krav Maga techniques.
/// May cause suffocation damage over time when integrated with respiration systems.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class KravMagaBlockedBreathingComponent : Component
{
    [DataField]
    public TimeSpan BlockedTime = TimeSpan.Zero;
}
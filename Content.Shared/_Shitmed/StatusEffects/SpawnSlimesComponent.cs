// SPDX-FileCopyrightText: 2025 gluesniffler <159397573+gluesniffler@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Piras314 <p1r4s@proton.me>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.GameStates;

namespace Content.Shared._Shitmed.StatusEffects;

/// <summary>
///     For use as a status effect. Spawns slimes.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class SpawnSlimesComponent : SpawnEntityEffectComponent
{
    public override string EntityPrototype { get; set; } = "MobAdultSlimesBlueAngry";

    public override bool IsFriendly { get; set; } = true;
}
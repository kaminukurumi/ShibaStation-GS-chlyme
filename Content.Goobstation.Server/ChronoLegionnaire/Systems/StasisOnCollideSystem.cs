// SPDX-FileCopyrightText: 2024 BombasterDS <115770678+BombasterDS@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Piras314 <p1r4s@proton.me>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Misandry <mary@thughunt.ing>
// SPDX-FileCopyrightText: 2025 gus <august.eymann@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Goobstation.Server.ChronoLegionnaire.Components;
using Content.Shared.StatusEffect;
using Content.Shared.Throwing;
using JetBrains.Annotations;
using Robust.Shared.Physics.Events;

namespace Content.Goobstation.Server.ChronoLegionnaire.Systems;

[UsedImplicitly]
public sealed class StasisOnCollideSystem : EntitySystem
{
    [Dependency] private readonly StasisSystem _stasisSystem = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<StasisOnCollideComponent, StartCollideEvent>(HandleCollide);
        SubscribeLocalEvent<StasisOnCollideComponent, ThrowDoHitEvent>(HandleThrow);
    }

    private void TryCollideStasis(Entity<StasisOnCollideComponent> projectile, EntityUid target)
    {
        if (EntityManager.TryGetComponent<StatusEffectsComponent>(target, out var status))
        {
            _stasisSystem.TryStasis((target, status), true, projectile.Comp.StasisTime);
        }
    }

    /// <summary>
    /// Check if projectile hits another entity
    /// </summary>
    private void HandleCollide(Entity<StasisOnCollideComponent> projectile, ref StartCollideEvent args)
    {
        if (args.OurFixtureId != projectile.Comp.FixtureID)
            return;

        TryCollideStasis(projectile, args.OtherEntity);
    }

    /// <summary>
    /// For throwing (in chrono bola case)
    /// </summary>
    private void HandleThrow(Entity<StasisOnCollideComponent> projectile, ref ThrowDoHitEvent args)
    {
        TryCollideStasis(projectile, args.Target);
    }

}
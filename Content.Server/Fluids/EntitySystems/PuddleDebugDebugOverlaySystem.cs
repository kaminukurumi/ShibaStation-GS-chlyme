// SPDX-FileCopyrightText: 2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Ygg01 <y.laughing.man.y@gmail.com>
// SPDX-FileCopyrightText: 2023 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Visne <39844191+Visne@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Tayrtahn <tayrtahn@gmail.com>
// SPDX-FileCopyrightText: 2024 TemporalOroboros <TemporalOroboros@gmail.com>
// SPDX-FileCopyrightText: 2024 LordCarve <27449516+LordCarve@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Winkarst <74284083+Winkarst-cpu@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Numerics;
using Content.Shared.Fluids;
using Content.Shared.Fluids.Components;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Server.Fluids.EntitySystems;

public sealed class PuddleDebugDebugOverlaySystem : SharedPuddleDebugOverlaySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly IMapManager _mapManager = default!;
    [Dependency] private readonly PuddleSystem _puddle = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly SharedMapSystem _map = default!;

    private readonly HashSet<ICommonSession> _playerObservers = [];
    private List<Entity<MapGridComponent>> _grids = [];

    public bool ToggleObserver(ICommonSession observer)
    {
        NextTick ??= _timing.CurTime + Cooldown;

        if (_playerObservers.Contains(observer))
        {
            RemoveObserver(observer);
            return false;
        }

        _playerObservers.Add(observer);
        return true;
    }

    private void RemoveObserver(ICommonSession observer)
    {
        if (!_playerObservers.Remove(observer))
        {
            return;
        }

        var message = new PuddleOverlayDisableMessage();
        RaiseNetworkEvent(message, observer.Channel);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        if (NextTick == null || _timing.CurTime < NextTick)
            return;

        foreach (var session in _playerObservers)
        {
            if (session.AttachedEntity is not { Valid: true } entity)
                continue;

            var transform = EntityManager.GetComponent<TransformComponent>(entity);


            var worldBounds = Box2.CenteredAround(_transform.GetWorldPosition(transform),
                new Vector2(LocalViewRange, LocalViewRange));

            _grids.Clear();
            _mapManager.FindGridsIntersecting(transform.MapID, worldBounds, ref _grids);

            foreach (var grid in _grids)
            {
                var data = new List<PuddleDebugOverlayData>();
                var gridUid = grid.Owner;

                if (!Exists(gridUid))
                    continue;

                foreach (var uid in _map.GetAnchoredEntities(gridUid, grid, worldBounds))
                {
                    PuddleComponent? puddle = null;
                    TransformComponent? xform = null;
                    if (!Resolve(uid, ref puddle, ref xform, false))
                        continue;

                    var pos = xform.Coordinates.ToVector2i(EntityManager, _mapManager, _transform);
                    var vol = _puddle.CurrentVolume(uid, puddle);
                    data.Add(new PuddleDebugOverlayData(pos, vol));
                }

                RaiseNetworkEvent(new PuddleOverlayDebugMessage(GetNetEntity(gridUid), data.ToArray()), session);
            }
        }

        NextTick = _timing.CurTime + Cooldown;
    }
}
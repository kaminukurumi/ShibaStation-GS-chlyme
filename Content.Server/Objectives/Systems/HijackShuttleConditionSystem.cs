// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Morb <14136326+Morb0@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Aviu00 <93730715+Aviu00@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Piras314 <p1r4s@proton.me>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Server.Objectives.Components;
using Content.Server.Shuttles.Components;
using Content.Server.Shuttles.Systems;
using Content.Shared.Cuffs.Components;
using Content.Shared.Humanoid;
using Content.Shared.Mind;
using Content.Shared.Mobs.Systems;
using Content.Shared.Objectives.Components;
using Content.Shared.Roles;
using Robust.Shared.Player;

namespace Content.Server.Objectives.Systems;

public sealed class HijackShuttleConditionSystem : EntitySystem
{
    [Dependency] private readonly EmergencyShuttleSystem _emergencyShuttle = default!;
    [Dependency] private readonly SharedMindSystem _mind = default!;
    [Dependency] private readonly SharedRoleSystem _role = default!;
    [Dependency] private readonly MobStateSystem _mobState = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<HijackShuttleConditionComponent, ObjectiveGetProgressEvent>(OnGetProgress);
    }

    private void OnGetProgress(EntityUid uid, HijackShuttleConditionComponent comp, ref ObjectiveGetProgressEvent args)
    {
        args.Progress = GetProgress(args.MindId, args.Mind);
    }

    private float GetProgress(EntityUid mindId, MindComponent mind)
    {
        // Not escaping alive if you're deleted/dead
        if (mind.OwnedEntity == null || _mind.IsCharacterDeadIc(mind))
            return 0f;

        // You're not escaping if you're restrained!
        if (TryComp<CuffableComponent>(mind.OwnedEntity, out var cuffed) && cuffed.CuffedHandCount > 0)
            return 0f;

        // There no emergency shuttles
        if (!_emergencyShuttle.EmergencyShuttleArrived)
            return 0f;

        // Check hijack for each emergency shuttle
        foreach (var stationData in EntityQuery<StationEmergencyShuttleComponent>())
        {
            if (stationData.EmergencyShuttle == null)
                continue;

            if (IsShuttleHijacked(stationData.EmergencyShuttle.Value, mindId))
                return 1f;
        }

        return 0f;
    }

    private bool IsShuttleHijacked(EntityUid shuttleGridId, EntityUid mindId)
    {
        var gridPlayers = Filter.BroadcastGrid(shuttleGridId).Recipients;
        var humanoids = GetEntityQuery<HumanoidAppearanceComponent>();
        var cuffable = GetEntityQuery<CuffableComponent>();

        var agentOnShuttle = false;
        foreach (var player in gridPlayers)
        {
            if (player.AttachedEntity == null ||
                !_mind.TryGetMind(player.AttachedEntity.Value, out var crewMindId, out _))
                continue;

            if (mindId == crewMindId)
            {
                agentOnShuttle = true;
                continue;
            }

            var isHumanoid = humanoids.HasComponent(player.AttachedEntity.Value);
            if (!isHumanoid) // Only humanoids count as enemies
                continue;

            var isAntagonist = _role.MindIsAntagonist(crewMindId); // Goobstation
            if (isAntagonist) // Allow antagonist
                continue;

            var isPersonIncapacitated = _mobState.IsIncapacitated(player.AttachedEntity.Value);
            if (isPersonIncapacitated) // Allow dead and crit
                continue;

            var isPersonCuffed =
                cuffable.TryGetComponent(player.AttachedEntity.Value, out var cuffed)
                && cuffed.CuffedHandCount > 0;
            if (isPersonCuffed) // Allow handcuffed
                continue;

            return false;
        }

        return agentOnShuttle;
    }
}
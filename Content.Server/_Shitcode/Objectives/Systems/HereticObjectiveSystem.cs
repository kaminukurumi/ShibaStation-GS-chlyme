// SPDX-FileCopyrightText: 2024 Piras314 <p1r4s@proton.me>
// SPDX-FileCopyrightText: 2024 username <113782077+whateverusername0@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 whateverusername0 <whateveremail>
// SPDX-FileCopyrightText: 2025 gus <august.eymann@gmail.com>
// SPDX-FileCopyrightText: 2025 Misandry <mary@thughunt.ing>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Server._Goobstation.Objectives.Components;
using Content.Server.Objectives.Systems;
using Content.Shared.Objectives.Components;

namespace Content.Server._Goobstation.Objectives.Systems;

public sealed partial class HereticObjectiveSystem : EntitySystem
{
    [Dependency] private readonly NumberObjectiveSystem _number = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<HereticKnowledgeConditionComponent, ObjectiveGetProgressEvent>(OnGetKnowledgeProgress);
        SubscribeLocalEvent<HereticSacrificeConditionComponent, ObjectiveGetProgressEvent>(OnGetSacrificeProgress);
    }

    private void OnGetKnowledgeProgress(Entity<HereticKnowledgeConditionComponent> ent, ref ObjectiveGetProgressEvent args)
    {
        var target = _number.GetTarget(ent);
        if (target != 0)
            args.Progress = MathF.Min(ent.Comp.Researched / target, 1f);
        else args.Progress = 1f;
    }
    private void OnGetSacrificeProgress(Entity<HereticSacrificeConditionComponent> ent, ref ObjectiveGetProgressEvent args)
    {
        var target = _number.GetTarget(ent);
        if (target != 0)
            args.Progress = MathF.Min(ent.Comp.Sacrificed / target, 1f);
        else args.Progress = 1f;
    }
}
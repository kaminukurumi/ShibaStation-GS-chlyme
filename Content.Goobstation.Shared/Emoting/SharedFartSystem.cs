// SPDX-FileCopyrightText: 2024 Piras314 <p1r4s@proton.me>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Misandry <mary@thughunt.ing>
// SPDX-FileCopyrightText: 2025 gus <august.eymann@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.GameStates;

namespace Content.Goobstation.Shared.Emoting;
public abstract class SharedFartSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<FartComponent, ComponentGetState>(OnGetState);
    }

    private void OnGetState(Entity<FartComponent> ent, ref ComponentGetState args)
    {
        args.State = new FartComponentState(ent.Comp.Emote, ent.Comp.FartTimeout, ent.Comp.FartInhale, ent.Comp.SuperFarted);
    }
}
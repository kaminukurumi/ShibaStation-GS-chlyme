// SPDX-FileCopyrightText: 2024 Piras314 <p1r4s@proton.me>
// SPDX-FileCopyrightText: 2024 whateverusername0 <whateveremail>
// SPDX-FileCopyrightText: 2025 gus <august.eymann@gmail.com>
// SPDX-FileCopyrightText: 2025 Misandry <mary@thughunt.ing>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Goobstation.Server.Changeling.Objectives.Systems;

namespace Content.Goobstation.Server.Changeling.Objectives.Components;

[RegisterComponent, Access(typeof(ChangelingObjectiveSystem), typeof(ChangelingSystem))]
public sealed partial class AbsorbConditionComponent : Component
{
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float Absorbed = 0f;
}
// SPDX-FileCopyrightText: 2024 Piras314 <p1r4s@proton.me>
// SPDX-FileCopyrightText: 2024 Mary <mary@thughunt.ing>
// SPDX-FileCopyrightText: 2025 gus <august.eymann@gmail.com>
// SPDX-FileCopyrightText: 2025 Misandry <mary@thughunt.ing>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Server.Objectives.Components;

namespace Content.Server._Goobstation.Objectives.Components;

/// <summary>
/// Sets the target for <see cref="TargetObjectiveComponent"/> to a random traitor
/// If there are no traitors it will fallback to any person.
/// </summary>
[RegisterComponent]
public sealed partial class PickRandomTraitorComponent : Component
{
}
// SPDX-FileCopyrightText: 2024 Piras314 <p1r4s@proton.me>
// SPDX-FileCopyrightText: 2024 gluesniffler <159397573+gluesniffler@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

namespace Content.Shared._Shitmed.Medical.Surgery;

/// <summary>
///     Raised on the target entity.
/// </summary>
[ByRefEvent]
public record struct SurgeryStepDamageChangeEvent(EntityUid User, EntityUid Body, EntityUid Part, EntityUid Step);
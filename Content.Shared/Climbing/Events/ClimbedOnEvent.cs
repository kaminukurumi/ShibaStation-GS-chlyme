// SPDX-FileCopyrightText: 2023 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
//
// SPDX-License-Identifier: MIT

namespace Content.Shared.Climbing.Events;

/// <summary>
///     Raised on an entity when it is climbed on.
/// </summary>
[ByRefEvent]
public readonly record struct ClimbedOnEvent(EntityUid Climber, EntityUid Instigator);
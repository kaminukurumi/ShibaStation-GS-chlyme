// SPDX-FileCopyrightText: 2023 Kara <lunarautomaton6@gmail.com>
//
// SPDX-License-Identifier: MIT

namespace Content.Shared.Ghost;

/// <summary>
/// This is used for marking entities which should receive all local chat message, even when out of range
/// </summary>
[RegisterComponent]
public sealed partial class GhostHearingComponent : Component
{
}
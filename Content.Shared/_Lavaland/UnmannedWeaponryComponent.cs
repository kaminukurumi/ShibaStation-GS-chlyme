// SPDX-FileCopyrightText: 2025 gluesniffler <159397573+gluesniffler@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.GameStates;

namespace Content.Shared._Lavaland.Components;

/// <summary>
/// Marker component for shuttle weaponry to prevent cheesing hierophant.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class UnmannedWeaponryComponent : Component;
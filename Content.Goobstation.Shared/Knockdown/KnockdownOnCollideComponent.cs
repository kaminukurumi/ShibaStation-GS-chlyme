// SPDX-FileCopyrightText: 2024 Aviu00 <93730715+Aviu00@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 gus <august.eymann@gmail.com>
// SPDX-FileCopyrightText: 2025 Misandry <mary@thughunt.ing>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Goobstation.Common.Standing;

namespace Content.Goobstation.Shared.Knockdown;

[RegisterComponent]
public sealed partial class KnockdownOnCollideComponent : Component
{
    [DataField]
    public DropHeldItemsBehavior Behavior = DropHeldItemsBehavior.NoDrop;
}
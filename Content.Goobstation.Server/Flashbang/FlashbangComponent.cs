// SPDX-FileCopyrightText: 2024 Aviu00 <93730715+Aviu00@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 gus <august.eymann@gmail.com>
// SPDX-FileCopyrightText: 2025 Misandry <mary@thughunt.ing>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

namespace Content.Goobstation.Server.Flashbang;

[RegisterComponent]
public sealed partial class FlashbangComponent : Component
{
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float StunTime = 2f;

    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float KnockdownTime = 10f;
}
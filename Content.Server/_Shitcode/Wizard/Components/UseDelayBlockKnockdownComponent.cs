// SPDX-FileCopyrightText: 2025 gus <august.eymann@gmail.com>
// SPDX-FileCopyrightText: 2025 Misandry <mary@thughunt.ing>
// SPDX-FileCopyrightText: 2025 Aviu00 <93730715+Aviu00@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.Audio;

namespace Content.Server._Goobstation.Wizard.Components;

[RegisterComponent]
public sealed partial class UseDelayBlockKnockdownComponent : Component
{
    [DataField]
    public string Delay = "default";

    [DataField]
    public bool ResetDelayOnSuccess = true;

    [DataField]
    public SoundSpecifier? KnockdownSound = new SoundPathSpecifier("/Audio/Effects/Lightning/lightningbolt.ogg");

    [DataField]
    public bool DoSparks = true;
}
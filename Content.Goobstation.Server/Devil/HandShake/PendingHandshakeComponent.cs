// SPDX-FileCopyrightText: 2025 GoobBot <uristmchands@proton.me>
// SPDX-FileCopyrightText: 2025 SolsticeOfTheWinter <solsticeofthewinter@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

namespace Content.Goobstation.Server.Devil.HandShake;

[RegisterComponent]
public sealed partial class PendingHandshakeComponent : Component
{
    [DataField]
    public EntityUid? Offerer;

    [DataField]
    public TimeSpan ExpiryTime;
}

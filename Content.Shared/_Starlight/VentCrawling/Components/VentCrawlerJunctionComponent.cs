// SPDX-FileCopyrightText: 2025 fishbait <gnesse@gmail.com>
// SPDX-FileCopyrightText: 2025 Rinary <72972221+Rinary1@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 ss14-Starlight <ss14-Starlight@outlook.com>
// SPDX-FileCopyrightText: 2025 Fishbait <Fishbait@git.ml>
// SPDX-FileCopyrightText: 2025 Aiden <aiden@djkraz.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

namespace Content.Shared._Starlight.VentCrawling.Components;

[RegisterComponent, Virtual]
public partial class VentCrawlerJunctionComponent : Component
{
    /// <summary>
    ///     The angles to connect to.
    /// </summary>
    [DataField("degrees")] public List<Angle> Degrees = new();
}
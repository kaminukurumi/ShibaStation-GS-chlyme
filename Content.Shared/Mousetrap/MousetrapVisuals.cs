// SPDX-FileCopyrightText: 2022 Flipp Syder <76629141+vulppine@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Kara <lunarautomaton6@gmail.com>
//
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization;

namespace Content.Shared.Mousetrap;

[Serializable, NetSerializable]
public enum MousetrapVisuals : byte
{
    Visual,
    Armed,
    Unarmed
}
// SPDX-FileCopyrightText: 2024 Aidenkrz <aiden@djkraz.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.Serialization;
using Content.Shared.DoAfter;

namespace Content.Shared.Item.PseudoItem;


[Serializable, NetSerializable]
public sealed partial class PseudoItemInsertDoAfterEvent : SimpleDoAfterEvent
{
}

// SPDX-FileCopyrightText: 2023 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
//
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization;

namespace Content.Shared.Construction;

[Serializable, NetSerializable]
public enum ConstructionVisuals : byte
{
    Key,
    Layer,
    Wired,
}
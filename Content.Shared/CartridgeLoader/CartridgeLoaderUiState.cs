// SPDX-FileCopyrightText: 2022 Julian Giebel <juliangiebel@live.de>
// SPDX-FileCopyrightText: 2023 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Piras314 <p1r4s@proton.me>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.Serialization;

namespace Content.Shared.CartridgeLoader;

[Virtual]
[Serializable, NetSerializable]
public class CartridgeLoaderUiState : BoundUserInterfaceState
{
    public NetEntity? ActiveUI;
    public List<NetEntity> Programs;

    public CartridgeLoaderUiState(List<NetEntity> programs, NetEntity? activeUI)
    {
        Programs = programs;
        ActiveUI = activeUI;
    }
}
// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 DoutorWhite <thedoctorwhite@gmail.com>
// SPDX-FileCopyrightText: 2025 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.Maps;
using Robust.Shared.Noise;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared.Parallax.Biomes.Layers;

[Serializable, NetSerializable]
public sealed partial class BiomeTileLayer : IBiomeLayer
{
    [DataField] public FastNoiseLite Noise { get; private set; } = new(0);

    /// <inheritdoc/>
    [DataField]
    public float Threshold { get; private set; } = 0.5f;

    /// <inheritdoc/>
    [DataField] public bool Invert { get; private set; } = false;

    /// <summary>
    /// Which tile variants to use for this layer. Uses all of the tile's variants if none specified
    /// </summary>
    [DataField]
    public List<byte>? Variants = null;

    [DataField(required: true)]
    public ProtoId<ContentTileDefinition> Tile = string.Empty;
}
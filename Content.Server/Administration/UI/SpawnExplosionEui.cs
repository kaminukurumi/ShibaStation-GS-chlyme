// SPDX-FileCopyrightText: 2022 ElectroJr <leonsfriedrich@gmail.com>
// SPDX-FileCopyrightText: 2022 Moony <moonheart08@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 moonheart08 <moonheart08@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 0x6273 <0x40@keemail.me>
// SPDX-FileCopyrightText: 2024 Kara <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2024 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Server.EUI;
using Content.Server.Explosion.EntitySystems;
using Content.Shared.Administration;
using Content.Shared.Eui;
using JetBrains.Annotations;

namespace Content.Server.Administration.UI;

/// <summary>
///     Admin Eui for spawning and preview-ing explosions
/// </summary>
[UsedImplicitly]
public sealed class SpawnExplosionEui : BaseEui
{
    private readonly ExplosionSystem _explosionSystem;
    private readonly ISawmill _sawmill;

    public SpawnExplosionEui()
    {
        _explosionSystem = IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<ExplosionSystem>();
        _sawmill = IoCManager.Resolve<ILogManager>().GetSawmill("explosion");
    }

    public override void HandleMessage(EuiMessageBase msg)
    {
        base.HandleMessage(msg);

        if (msg is not SpawnExplosionEuiMsg.PreviewRequest request)
            return;

        if (request.TotalIntensity <= 0 || request.IntensitySlope <= 0)
            return;

        var explosion = _explosionSystem.GenerateExplosionPreview(request);

        if (explosion == null)
        {
            _sawmill.Error("Failed to generate explosion preview.");
            return;
        }

        SendMessage(new SpawnExplosionEuiMsg.PreviewData(explosion, request.IntensitySlope, request.TotalIntensity));
    }
}
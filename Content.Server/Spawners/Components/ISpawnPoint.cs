// SPDX-FileCopyrightText: 2024 778b <33431126+778b@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 metalgearsloth <comedian_vs_clown@hotmail.com>
//
// SPDX-License-Identifier: MIT

namespace Content.Server.Spawners.Components;

public interface ISpawnPoint
{
    SpawnPointType SpawnType { get; set; }
}

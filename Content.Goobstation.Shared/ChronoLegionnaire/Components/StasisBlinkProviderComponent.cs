// SPDX-FileCopyrightText: 2024 Piras314 <p1r4s@proton.me>
// SPDX-FileCopyrightText: 2024 BombasterDS <115770678+BombasterDS@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 gus <august.eymann@gmail.com>
// SPDX-FileCopyrightText: 2025 Misandry <mary@thughunt.ing>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Goobstation.Shared.ChronoLegionnaire.EntitySystems;
using Content.Shared.Actions;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Goobstation.Shared.ChronoLegionnaire.Components
{
    /// <summary>
    /// Marks an clothing that will give stasis blink ability to wearer
    /// </summary>
    [RegisterComponent, NetworkedComponent, Access(typeof(SharedStasisBlinkProviderSystem)), AutoGenerateComponentState]
    public sealed partial class StasisBlinkProviderComponent : Component
    {
        /// <summary>
        /// The action blink id.
        /// </summary>
        [DataField]
        public EntProtoId<WorldTargetActionComponent> BlinkAction = "ActionChronoBlink";

        [DataField, AutoNetworkedField]
        public EntityUid? BlinkActionEntity;
    }
}
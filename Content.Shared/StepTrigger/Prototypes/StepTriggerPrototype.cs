// SPDX-FileCopyrightText: 2024 Adeinitas <147965189+adeinitas@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Danger Revolution! <142105406+DangerRevolution@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Timemaster99 <57200767+Timemaster99@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 VMSolidus <evilexecutive@gmail.com>
// SPDX-FileCopyrightText: 2024 gluesniffler <159397573+gluesniffler@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.Prototypes;

namespace Content.Shared.StepTrigger.Prototypes;

/// <summary>
///     Goobstation Change: Prototype representing a StepTriggerType in YAML.
///     Meant to only have an ID property, as that is the only thing that
///     gets saved in StepTriggerGroup.
/// </summary>
[Prototype]
public sealed partial class StepTriggerTypePrototype : IPrototype
{
    [ViewVariables, IdDataField]
    public string ID { get; private set; } = default!;
}
// SPDX-FileCopyrightText: 2024 Adeinitas <147965189+adeinitas@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Danger Revolution! <142105406+DangerRevolution@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Timemaster99 <57200767+Timemaster99@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 VMSolidus <evilexecutive@gmail.com>
// SPDX-FileCopyrightText: 2024 gluesniffler <159397573+gluesniffler@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.StepTrigger.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.StepTrigger.Prototypes;

/// <summary>
///     Goobstation: A group of <see cref="StepTriggerTypePrototype">
///     Used to determine StepTriggerTypes like Tags.
///     Used for better work with Immunity.
///     StepTriggerTypes in StepTriggerTypes.yml
/// </summary>
/// <code>
/// stepTriggerGroups:
///   types:
///   - Lava
///   - Landmine
///   - Shard
///   - Chasm
///   - Mousetrap
///   - SlipTile
///   - SlipEntity
/// </code>
[DataDefinition]
[Serializable, NetSerializable]
public sealed partial class StepTriggerGroup
{
    [DataField]
    public List<ProtoId<StepTriggerTypePrototype>>? Types = null;

    /// <summary>
    ///     Checks if types of this StepTriggerGroup is similar to types of AnotherGroup
    /// </summary>
    public bool IsValid(StepTriggerGroup? anotherGroup)
    {
        if (Types is null)
            return false;

        foreach (var type in Types)
        {
            if (anotherGroup != null
                && anotherGroup.Types != null
                && anotherGroup.Types.Contains(type))
                return true;
        }
        return false;
    }

    /// <summary>
    ///     Checks validation (if types of this StepTriggerGroup are similar to types of
    ///     another StepTriggerComponent.
    /// </summary>
    public bool IsValid(StepTriggerComponent component)
    {
        if (component.TriggerGroups is null)
            return false;

        return IsValid(component.TriggerGroups);
    }

    /// <summary>
    ///     Checks validation (if types of this StepTriggerGroup are similar to types of
    ///     another StepTriggerImmuneComponent.
    /// </summary>
    public bool IsValid(StepTriggerImmuneComponent component)
    {
        if (component.Whitelist is null)
            return false;

        return IsValid(component.Whitelist);
    }
}
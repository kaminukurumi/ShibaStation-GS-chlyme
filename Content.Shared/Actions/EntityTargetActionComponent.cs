// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 ShadowCommander <10494922+ShadowCommander@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 ActiveMammmoth <140334666+ActiveMammmoth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 ActiveMammmoth <kmcsmooth@gmail.com>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 gus <august.eymann@gmail.com>
// SPDX-FileCopyrightText: 2025 keronshb <54602815+keronshb@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.Whitelist;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared.Actions;

/// <summary>
/// Used on action entities to define an action that triggers when targeting an entity.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class EntityTargetActionComponent : BaseTargetActionComponent
{
    public override BaseActionEvent? BaseEvent => Event;

    /// <summary>
    ///     The local-event to raise when this action is performed.
    /// </summary>
    [DataField("event")]
    [NonSerialized]
    public EntityTargetActionEvent? Event;

    /// <summary>
    /// Determines which entities are valid targets for this action.
    /// </summary>
    /// <remarks>No whitelist check when null.</remarks>
    [DataField("whitelist")] public EntityWhitelist? Whitelist;

    /// <summary>
    /// Determines which entities are NOT valid targets for this action.
    /// </summary>
    /// <remarks>No blacklist check when null.</remarks>
    [DataField] public EntityWhitelist? Blacklist;

    /// <summary>
    /// Whether this action considers the user as a valid target entity when using this action.
    /// </summary>
    [DataField("canTargetSelf")] public bool CanTargetSelf = true;
}

[Serializable, NetSerializable]
public sealed class EntityTargetActionComponentState : BaseActionComponentState
{
    public EntityWhitelist? Whitelist;
    public EntityWhitelist? Blacklist;
    public bool CanTargetSelf;

    public EntityTargetActionComponentState(EntityTargetActionComponent component, IEntityManager entManager) : base(component, entManager)
    {
        Whitelist = component.Whitelist;
        Blacklist = component.Blacklist;
        CanTargetSelf = component.CanTargetSelf;
    }
}
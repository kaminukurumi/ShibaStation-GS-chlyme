// SPDX-FileCopyrightText: 2022 Kara <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2022 Kara D <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2022 mirrorcult <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2023 Chief-Engineer <119664036+Chief-Engineer@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2023 Ygg01 <y.laughing.man.y@gmail.com>
// SPDX-FileCopyrightText: 2023 metalgearsloth <comedian_vs_clown@hotmail.com>
// SPDX-FileCopyrightText: 2023 moonheart08 <moonheart08@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Aidenkrz <aiden@djkraz.com>
// SPDX-FileCopyrightText: 2024 Jezithyr <jezithyr@gmail.com>
// SPDX-FileCopyrightText: 2024 Plykiya <58439124+Plykiya@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 ShadowCommander <10494922+ShadowCommander@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 ShadowCommander <shadowjjt@gmail.com>
// SPDX-FileCopyrightText: 2024 Tayrtahn <tayrtahn@gmail.com>
// SPDX-FileCopyrightText: 2024 deltanedas <39013340+deltanedas@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 deltanedas <@deltanedas:kde.org>
// SPDX-FileCopyrightText: 2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 plykiya <plykiya@protonmail.com>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Aviu00 <aviu00@protonmail.com>
// SPDX-FileCopyrightText: 2025 GoobBot <uristmchands@proton.me>
// SPDX-FileCopyrightText: 2025 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 SX_7 <sn1.test.preria.2002@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Linq;
using System.Numerics;
using Content.Shared.Administration.Managers;
using Content.Shared.Database;
using Content.Shared.Follower.Components;
using Content.Shared.Ghost;
using Content.Shared.Hands;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Pulling.Events;
using Content.Shared.Polymorph;
using Content.Shared.Tag;
using Content.Shared.Verbs;
using Robust.Shared.Containers;
using Robust.Shared.GameStates;
using Robust.Shared.Map;
using Robust.Shared.Map.Events;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Utility;

namespace Content.Shared.Follower;

public sealed class FollowerSystem : EntitySystem
{
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly TagSystem _tagSystem = default!;
    [Dependency] private readonly SharedContainerSystem _containerSystem = default!;
    [Dependency] private readonly SharedJointSystem _jointSystem = default!;
    [Dependency] private readonly SharedPhysicsSystem _physicsSystem = default!;
    [Dependency] private readonly INetManager _netMan = default!;
    [Dependency] private readonly ISharedAdminManager _adminManager = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<GetVerbsEvent<AlternativeVerb>>(OnGetAlternativeVerbs);
        SubscribeLocalEvent<FollowerComponent, MoveInputEvent>(OnFollowerMove);
        SubscribeLocalEvent<FollowerComponent, PullStartedMessage>(OnPullStarted);
        SubscribeLocalEvent<FollowerComponent, EntityTerminatingEvent>(OnFollowerTerminating);
        SubscribeLocalEvent<FollowerComponent, AfterAutoHandleStateEvent>(OnAfterHandleState);

        SubscribeLocalEvent<FollowedComponent, ComponentGetStateAttemptEvent>(OnFollowedAttempt);
        SubscribeLocalEvent<FollowerComponent, GotEquippedHandEvent>(OnGotEquippedHand);
        SubscribeLocalEvent<FollowedComponent, EntityTerminatingEvent>(OnFollowedTerminating);
        SubscribeLocalEvent<BeforeSerializationEvent>(OnBeforeSave);
        SubscribeLocalEvent<FollowedComponent, PolymorphedEvent>(OnFollowedPolymorphed);
    }

    private void OnFollowedAttempt(Entity<FollowedComponent> ent, ref ComponentGetStateAttemptEvent args)
    {
        if (args.Cancelled)
            return;

        // Clientside VV stay losing
        var playerEnt = args.Player?.AttachedEntity;

        if (playerEnt == null ||
            !ent.Comp.Following.Contains(playerEnt.Value) && !HasComp<GhostComponent>(playerEnt.Value))
        {
            args.Cancelled = true;
        }
    }

    private void OnBeforeSave(BeforeSerializationEvent ev)
    {
        // Some followers will not be map savable. This ensures that maps don't get saved with some entities that have
        // empty/invalid followers, by just stopping any following happening on the map being saved.
        // I hate this so much.
        // TODO WeakEntityReference
        // We need some way to store entity references in a way that doesn't imply that the entity still exists.
        // Then we wouldn't have to deal with this shit.

        var maps = ev.Entities.Select(x => Transform(x).MapUid).ToHashSet();

        var query = AllEntityQuery<FollowerComponent, TransformComponent, MetaDataComponent>();
        while (query.MoveNext(out var uid, out var follower, out var xform, out var meta))
        {
            if (meta.EntityPrototype == null || meta.EntityPrototype.MapSavable)
                continue;

            if (!maps.Contains(xform.MapUid))
                continue;

            StopFollowingEntity(uid, follower.Following);
        }
    }

    private void OnGetAlternativeVerbs(GetVerbsEvent<AlternativeVerb> ev)
    {
        if (ev.User == ev.Target || IsClientSide(ev.Target))
            return;

        if (HasComp<GhostComponent>(ev.User))
        {
            var verb = new AlternativeVerb()
            {
                Priority = 10,
                Act = () => StartFollowingEntity(ev.User, ev.Target),
                Impact = LogImpact.Low,
                Text = Loc.GetString("verb-follow-text"),
                Icon = new SpriteSpecifier.Texture(new("/Textures/Interface/VerbIcons/open.svg.192dpi.png"))
            };
            ev.Verbs.Add(verb);
        }

        if (_tagSystem.HasTag(ev.Target, "ForceableFollow"))
        {
            if (!ev.CanAccess || !ev.CanInteract)
                return;

            var verb = new AlternativeVerb
            {
                Priority = 10,
                Act = () => StartFollowingEntity(ev.Target, ev.User),
                Impact = LogImpact.Low,
                Text = Loc.GetString("verb-follow-me-text"),
                Icon = new SpriteSpecifier.Texture(new ("/Textures/Interface/VerbIcons/close.svg.192dpi.png")),
            };

            ev.Verbs.Add(verb);
        }
    }

    private void OnFollowerMove(EntityUid uid, FollowerComponent component, ref MoveInputEvent args)
    {
        if (args.HasDirectionalMovement)
            StopFollowingEntity(uid, component.Following);
    }

    private void OnPullStarted(EntityUid uid, FollowerComponent component, PullStartedMessage args)
    {
        StopFollowingEntity(uid, component.Following);
    }

    private void OnGotEquippedHand(EntityUid uid, FollowerComponent component, GotEquippedHandEvent args)
    {
        StopFollowingEntity(uid, component.Following, deparent:false);
    }

    private void OnFollowerTerminating(EntityUid uid, FollowerComponent component, ref EntityTerminatingEvent args)
    {
        StopFollowingEntity(uid, component.Following, deparent: false);
    }

    private void OnAfterHandleState(Entity<FollowerComponent> entity, ref AfterAutoHandleStateEvent args)
    {
        StartFollowingEntity(entity, entity.Comp.Following);
    }

    // Since we parent our observer to the followed entity, we need to detach
    // before they get deleted so that we don't get recursively deleted too.
    private void OnFollowedTerminating(EntityUid uid, FollowedComponent component, ref EntityTerminatingEvent args)
    {
        StopAllFollowers(uid, component);
    }

    private void OnFollowedPolymorphed(Entity<FollowedComponent> entity, ref PolymorphedEvent args)
    {
        foreach (var follower in entity.Comp.Following)
        {
            if (_tagSystem.HasTag(follower, "FollowerStayOnPolymorph"))
                continue;
            // Stop following the target's old entity and start following the new one
            StartFollowingEntity(follower, args.NewEntity);
        }
    }

    /// <summary>
    ///     Makes an entity follow another entity, by parenting to it.
    /// </summary>
    /// <param name="follower">The entity that should follow</param>
    /// <param name="entity">The entity to be followed</param>
    public void StartFollowingEntity(EntityUid follower, EntityUid entity)
    {
        // No recursion for you
        var targetXform = Transform(entity);
        while (targetXform.ParentUid.IsValid())
        {
            if (targetXform.ParentUid == follower)
                return;

            targetXform = Transform(targetXform.ParentUid);
        }

        // Cleanup old following.
        if (TryComp<FollowerComponent>(follower, out var followerComp))
        {
            // Already following you goob
            if (followerComp.Following == entity)
                return;

            StopFollowingEntity(follower, followerComp.Following, deparent: false, removeComp: false);
        }
        else
        {
            followerComp = AddComp<FollowerComponent>(follower);
        }

        followerComp.Following = entity;

        var followedComp = EnsureComp<FollowedComponent>(entity);

        if (!followedComp.Following.Add(follower))
            return;

        if (TryComp<JointComponent>(follower, out var joints))
            _jointSystem.ClearJoints(follower, joints);

        var xform = Transform(follower);
        _containerSystem.AttachParentToContainerOrGrid((follower, xform));

        // If we didn't get to parent's container.
        if (xform.ParentUid != Transform(xform.ParentUid).ParentUid)
        {
            _transform.SetCoordinates(follower, xform, new EntityCoordinates(entity, Vector2.Zero), rotation: Angle.Zero);
        }

        _physicsSystem.SetLinearVelocity(follower, Vector2.Zero);

        EnsureComp<OrbitVisualsComponent>(follower);

        var followerEv = new StartedFollowingEntityEvent(entity, follower);
        var entityEv = new EntityStartedFollowingEvent(entity, follower);

        RaiseLocalEvent(follower, followerEv);
        RaiseLocalEvent(entity, entityEv);
        Dirty(entity, followedComp);
        Dirty(follower, followerComp);
    }

    /// <summary>
    ///     Forces an entity to stop following another entity, if it is doing so.
    /// </summary>
    /// <param name="deparent">Should the entity deparent itself</param>
    public void StopFollowingEntity(EntityUid uid, EntityUid target, FollowedComponent? followed = null, bool deparent = true, bool removeComp = true)
    {
        if (!Resolve(target, ref followed, false))
            return;

        if (!TryComp<FollowerComponent>(uid, out var followerComp) || followerComp.Following != target)
            return;

        followed.Following.Remove(uid);
        if (followed.Following.Count == 0)
            RemComp<FollowedComponent>(target);

        if (removeComp)
        {
            RemComp<FollowerComponent>(uid);
            RemComp<OrbitVisualsComponent>(uid);
        }

        var uidEv = new StoppedFollowingEntityEvent(target, uid);
        var targetEv = new EntityStoppedFollowingEvent(target, uid);

        RaiseLocalEvent(uid, uidEv, true);
        RaiseLocalEvent(target, targetEv, false);
        Dirty(target, followed);
        RaiseLocalEvent(uid, uidEv);
        RaiseLocalEvent(target, targetEv);

        if (!deparent || !TryComp(uid, out TransformComponent? xform))
            return;

        _transform.AttachToGridOrMap(uid, xform);
        if (xform.MapUid != null)
            return;

        if (_netMan.IsClient)
        {
            _transform.DetachEntity(uid, xform);
            return;
        }

        Log.Warning($"A follower has been detached to null-space and will be deleted. Follower: {ToPrettyString(uid)}. Followed: {ToPrettyString(target)}");
        QueueDel(uid);
    }

    /// <summary>
    ///     Forces all of an entity's followers to stop following it.
    /// </summary>
    public void StopAllFollowers(EntityUid uid,
        FollowedComponent? followed=null)
    {
        if (!Resolve(uid, ref followed))
            return;

        foreach (var player in followed.Following)
        {
            StopFollowingEntity(player, uid, followed);
        }
    }

    /// <summary>
    /// Gets the entity with the most non-admin ghosts following it.
    /// </summary>
    public EntityUid? GetMostGhostFollowed()
    {
        EntityUid? picked = null;
        var most = 0;

        // Keep a tally of how many ghosts are following each entity
        var followedEnts = new Dictionary<EntityUid, int>();

        // Look for followers that are ghosts and are player controlled
        var query = EntityQueryEnumerator<FollowerComponent, GhostComponent, ActorComponent>();
        while (query.MoveNext(out _, out var follower, out _, out var actor))
        {
            // Exclude admins
            if (_adminManager.IsAdmin(actor.PlayerSession))
                continue;

            var followed = follower.Following;
            // Add new entry or increment existing
            followedEnts.TryGetValue(followed, out var currentValue);
            followedEnts[followed] = currentValue + 1;

            if (followedEnts[followed] > most)
            {
                picked = followed;
                most = followedEnts[followed];
            }
        }

        return picked;
    }
}

public abstract class FollowEvent : EntityEventArgs
{
    public EntityUid Following;
    public EntityUid Follower;

    protected FollowEvent(EntityUid following, EntityUid follower)
    {
        Following = following;
        Follower = follower;
    }
}

/// <summary>
///     Raised on an entity when it start following another entity.
/// </summary>
public sealed class StartedFollowingEntityEvent : FollowEvent
{
    public StartedFollowingEntityEvent(EntityUid following, EntityUid follower) : base(following, follower)
    {
    }
}

/// <summary>
///     Raised on an entity when it stops following another entity.
/// </summary>
public sealed class StoppedFollowingEntityEvent : FollowEvent
{
    public StoppedFollowingEntityEvent(EntityUid following, EntityUid follower) : base(following, follower)
    {
    }
}

/// <summary>
///     Raised on an entity when it start following another entity.
/// </summary>
public sealed class EntityStartedFollowingEvent : FollowEvent
{
    public EntityStartedFollowingEvent(EntityUid following, EntityUid follower) : base(following, follower)
    {
    }
}

/// <summary>
///     Raised on an entity when it starts being followed by another entity.
/// </summary>
public sealed class EntityStoppedFollowingEvent : FollowEvent
{
    public EntityStoppedFollowingEvent(EntityUid following, EntityUid follower) : base(following, follower)
    {
    }
}

// SPDX-FileCopyrightText: 2024 Aiden <aiden@djkraz.com>
// SPDX-FileCopyrightText: 2024 deltanedas <@deltanedas:kde.org>
// SPDX-FileCopyrightText: 2024 plykiya <plykiya@protonmail.com>
// SPDX-FileCopyrightText: 2024 Арт <123451459+JustArt1m@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Tayrtahn <tayrtahn@gmail.com>
// SPDX-FileCopyrightText: 2024 PJBot <pieterjan.briers+bot@gmail.com>
// SPDX-FileCopyrightText: 2024 Mr. 27 <45323883+Dutch-VanDerLinde@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Ed <96445749+TheShuEd@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 DrSmugleaf <10968691+DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 lzk <124214523+lzk228@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Errant <35878406+Errant-4@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 IProduceWidgets <107586145+IProduceWidgets@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 osjarw <62134478+osjarw@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Vasilis <vasilis@pikachu.systems>
// SPDX-FileCopyrightText: 2024 deltanedas <39013340+deltanedas@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Rouge2t7 <81053047+Sarahon@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Flareguy <78941145+Flareguy@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Ubaser <134914314+UbaserB@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 beck-thompson <107373427+beck-thompson@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 HS <81934438+HolySSSS@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Plykiya <58439124+Plykiya@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Chief-Engineer <119664036+Chief-Engineer@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Truoizys <153248924+Truoizys@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 TsjipTsjip <19798667+TsjipTsjip@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2024 Alice "Arimah" Heurlin <30327355+arimah@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.Clothing.Components;
using Content.Shared.Inventory.Events;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Storage;
using Content.Shared.Whitelist;
using Robust.Shared.Containers;
using Robust.Shared.Timing;

namespace Content.Shared.Clothing.EntitySystems;

public sealed partial class PilotedClothingSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly SharedMoverController _moverController = default!;
    [Dependency] private readonly EntityWhitelistSystem _whitelist = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PilotedClothingComponent, EntInsertedIntoContainerMessage>(OnEntInserted);
        SubscribeLocalEvent<PilotedClothingComponent, EntRemovedFromContainerMessage>(OnEntRemoved);
        SubscribeLocalEvent<PilotedClothingComponent, GotEquippedEvent>(OnEquipped);
        SubscribeLocalEvent<PilotedClothingComponent, GotUnequippedEvent>(OnUnequipped);
    }

    private void OnEntInserted(Entity<PilotedClothingComponent> entity, ref EntInsertedIntoContainerMessage args)
    {
        // Make sure the entity was actually inserted into storage and not a different container.
        if (!TryComp(entity, out StorageComponent? storage) || args.Container != storage.Container)
            return;

        // Check potential pilot against whitelist, if one exists.
        if (_whitelist.IsWhitelistFail(entity.Comp.PilotWhitelist, args.Entity))
            return;

        entity.Comp.Pilot = args.Entity;
        Dirty(entity);

        // Attempt to setup control link, if Pilot and Wearer are both present.
        StartPiloting(entity);
    }

    private void OnEntRemoved(Entity<PilotedClothingComponent> entity, ref EntRemovedFromContainerMessage args)
    {
        // Make sure the removed entity is actually the pilot.
        if (args.Entity != entity.Comp.Pilot)
            return;

        StopPiloting(entity);
        entity.Comp.Pilot = null;
        Dirty(entity);
    }

    private void OnEquipped(Entity<PilotedClothingComponent> entity, ref GotEquippedEvent args)
    {
        if (!TryComp(entity, out ClothingComponent? clothing))
            return;

        // Make sure the clothing item was equipped to the right slot, and not just held in a hand.
        var isCorrectSlot = (clothing.Slots & args.SlotFlags) != Inventory.SlotFlags.NONE;
        if (!isCorrectSlot)
            return;

        entity.Comp.Wearer = args.Equipee;
        Dirty(entity);

        // Attempt to setup control link, if Pilot and Wearer are both present.
        StartPiloting(entity);
    }

    private void OnUnequipped(Entity<PilotedClothingComponent> entity, ref GotUnequippedEvent args)
    {
        StopPiloting(entity);

        entity.Comp.Wearer = null;
        Dirty(entity);
    }

    /// <summary>
    /// Attempts to establish movement/interaction relay connection(s) from Pilot to Wearer.
    /// If either is missing, fails and returns false.
    /// </summary>
    private bool StartPiloting(Entity<PilotedClothingComponent> entity)
    {
        // Make sure we have both a Pilot and a Wearer
        if (entity.Comp.Pilot == null || entity.Comp.Wearer == null)
            return false;

        if (!_timing.IsFirstTimePredicted)
            return false;

        var pilotEnt = entity.Comp.Pilot.Value;
        var wearerEnt = entity.Comp.Wearer.Value;

        // Add component to block prediction of wearer
        EnsureComp<PilotedByClothingComponent>(wearerEnt);

        if (entity.Comp.RelayMovement)
        {
            // Establish movement input relay.
            _moverController.SetRelay(pilotEnt, wearerEnt);
        }

        var pilotEv = new StartedPilotingClothingEvent(entity, wearerEnt);
        RaiseLocalEvent(pilotEnt, ref pilotEv);

        var wearerEv = new StartingBeingPilotedByClothing(entity, pilotEnt);
        RaiseLocalEvent(wearerEnt, ref wearerEv);

        return true;
    }

    /// <summary>
    /// Removes components from the Pilot and Wearer to stop the control relay.
    /// Returns false if a connection does not already exist.
    /// </summary>
    private bool StopPiloting(Entity<PilotedClothingComponent> entity)
    {
        if (entity.Comp.Pilot == null || entity.Comp.Wearer == null)
            return false;

        // Clean up components on the Pilot
        var pilotEnt = entity.Comp.Pilot.Value;
        RemCompDeferred<RelayInputMoverComponent>(pilotEnt);

        // Clean up components on the Wearer
        var wearerEnt = entity.Comp.Wearer.Value;
        RemCompDeferred<MovementRelayTargetComponent>(wearerEnt);
        RemCompDeferred<PilotedByClothingComponent>(wearerEnt);

        // Raise an event on the Pilot
        var pilotEv = new StoppedPilotingClothingEvent(entity, wearerEnt);
        RaiseLocalEvent(pilotEnt, ref pilotEv);

        // Raise an event on the Wearer
        var wearerEv = new StoppedBeingPilotedByClothing(entity, pilotEnt);
        RaiseLocalEvent(wearerEnt, ref wearerEv);

        return true;
    }
}

/// <summary>
/// Raised on the Pilot when they gain control of the Wearer.
/// </summary>
[ByRefEvent]
public record struct StartedPilotingClothingEvent(EntityUid Clothing, EntityUid Wearer);

/// <summary>
/// Raised on the Pilot when they lose control of the Wearer,
/// due to the Pilot exiting the clothing or the clothing being unequipped by the Wearer.
/// </summary>
[ByRefEvent]
public record struct StoppedPilotingClothingEvent(EntityUid Clothing, EntityUid Wearer);

/// <summary>
/// Raised on the Wearer when the Pilot gains control of them.
/// </summary>
[ByRefEvent]
public record struct StartingBeingPilotedByClothing(EntityUid Clothing, EntityUid Pilot);

/// <summary>
/// Raised on the Wearer when the Pilot loses control of them
/// due to the Pilot exiting the clothing or the clothing being unequipped by the Wearer.
/// </summary>
[ByRefEvent]
public record struct StoppedBeingPilotedByClothing(EntityUid Clothing, EntityUid Pilot);
// SPDX-FileCopyrightText: 2024 Aiden <aiden@djkraz.com>
// SPDX-FileCopyrightText: 2024 deltanedas <@deltanedas:kde.org>
// SPDX-FileCopyrightText: 2024 plykiya <plykiya@protonmail.com>
// SPDX-FileCopyrightText: 2024 Арт <123451459+JustArt1m@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 redfire1331 <Redfire1331@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Tayrtahn <tayrtahn@gmail.com>
// SPDX-FileCopyrightText: 2024 PJBot <pieterjan.briers+bot@gmail.com>
// SPDX-FileCopyrightText: 2024 Mr. 27 <45323883+Dutch-VanDerLinde@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Ed <96445749+TheShuEd@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 DrSmugleaf <10968691+DrSmugleaf@users.noreply.github.com>
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
// SPDX-FileCopyrightText: 2024 neutrino <67447925+neutrino-laser@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Boaz1111 <149967078+Boaz1111@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Redfire1331 <125223432+Redfire1331@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 MilenVolf <63782763+MilenVolf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Ghagliiarghii <68826635+Ghagliiarghii@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Alex Pavlenko <diraven@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Clothing;
using Content.Shared.Fluids.Components;

namespace Content.Shared.Fluids.EntitySystems;

/// <inheritdoc cref="SpillWhenWornComponent"/>
public sealed class SpillWhenWornSystem : EntitySystem
{
    [Dependency] private readonly SharedSolutionContainerSystem _solutionContainer = default!;
    [Dependency] private readonly SharedPuddleSystem _puddle = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SpillWhenWornComponent, ClothingGotEquippedEvent>(OnGotEquipped);
        SubscribeLocalEvent<SpillWhenWornComponent, ClothingGotUnequippedEvent>(OnGotUnequipped);
        SubscribeLocalEvent<SpillWhenWornComponent, SolutionAccessAttemptEvent>(OnSolutionAccessAttempt);
    }

    private void OnGotEquipped(Entity<SpillWhenWornComponent> ent, ref ClothingGotEquippedEvent args)
    {
        if (_solutionContainer.TryGetSolution(ent.Owner, ent.Comp.Solution, out var soln, out var solution)
            && solution.Volume > 0)
        {
            // Spill all solution on the player
            var drainedSolution = _solutionContainer.Drain(ent.Owner, soln.Value, solution.Volume);
            _puddle.TrySplashSpillAt(ent.Owner, Transform(args.Wearer).Coordinates, drainedSolution, out _);
        }

        // Flag as worn after draining, otherwise we'll block ourself from accessing!
        ent.Comp.IsWorn = true;
        Dirty(ent);
    }

    private void OnGotUnequipped(Entity<SpillWhenWornComponent> ent, ref ClothingGotUnequippedEvent args)
    {
        ent.Comp.IsWorn = false;
        Dirty(ent);
    }

    private void OnSolutionAccessAttempt(Entity<SpillWhenWornComponent> ent, ref SolutionAccessAttemptEvent args)
    {
        // If we're not being worn right now, we don't care
        if (!ent.Comp.IsWorn)
            return;

        // Make sure it's the right solution
        if (ent.Comp.Solution != args.SolutionName)
            return;

        args.Cancelled = true;
    }
}
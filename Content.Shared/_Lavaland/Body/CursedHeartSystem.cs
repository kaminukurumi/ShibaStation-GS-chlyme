// SPDX-FileCopyrightText: 2025 Rouden <149893554+Roudenn@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 deltanedas <@deltanedas:kde.org>
// SPDX-FileCopyrightText: 2025 Milon <plmilonpl@gmail.com>
// SPDX-FileCopyrightText: 2025 Unlumination <144041835+Unlumy@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 McBosserson <148172569+McBosserson@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 GoobBot <uristmchands@proton.me>
// SPDX-FileCopyrightText: 2025 coderabbitai[bot] <136622811+coderabbitai[bot]@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 gluesniffler <159397573+gluesniffler@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 whateverusername0 <whateveremail>
// SPDX-FileCopyrightText: 2025 username <113782077+whateverusername0@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 deltanedas <39013340+deltanedas@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 FaDeOkno <143940725+FaDeOkno@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Aineias1 <dmitri.s.kiselev@gmail.com>
// SPDX-FileCopyrightText: 2025 Piras314 <p1r4s@proton.me>
// SPDX-FileCopyrightText: 2025 Aidenkrz <aiden@djkraz.com>
// SPDX-FileCopyrightText: 2025 TheBorzoiMustConsume <197824988+TheBorzoiMustConsume@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 gluesniffler <linebarrelerenthusiast@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.Actions;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Interaction.Events;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._Lavaland.Body;

// TODO: Use Shitmed instead of Shitcode
public sealed class CursedHeartSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _actions = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly MobStateSystem _mobState = default!;
    [Dependency] private readonly DamageableSystem _damage = default!;
    //[Dependency] private readonly BloodstreamSystem _bloodstream = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<CursedHeartComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<CursedHeartComponent, ComponentShutdown>(OnShutdown);
        SubscribeLocalEvent<CursedHeartComponent, PumpHeartActionEvent>(OnPump);

        SubscribeLocalEvent<CursedHeartGrantComponent, UseInHandEvent>(OnUseInHand);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<CursedHeartComponent, MobStateComponent>();
        while (query.MoveNext(out var uid, out var comp, out var state))
        {
            if (state.CurrentState is MobState.Critical or MobState.Dead)
                continue;

            if (_timing.CurTime < comp.LastPump + TimeSpan.FromSeconds(comp.MaxDelay))
                continue;

            Damage(uid);
            comp.LastPump = _timing.CurTime;
        }
    }

    private void Damage(EntityUid uid)
    {
        // TODO: WHY BLOODSTREAM IS NOT IN SHARED RAAAAAGH
        //_bloodstream.TryModifyBloodLevel(uid, -50, spill: false);
        _damage.TryChangeDamage(uid, new DamageSpecifier(_proto.Index<DamageGroupPrototype>("Airloss"), 50), true, false);
        _popup.PopupEntity(Loc.GetString("popup-cursed-heart-damage"), uid, uid, PopupType.MediumCaution);
    }

    private void OnMapInit(EntityUid uid, CursedHeartComponent comp, MapInitEvent args)
    {
        _actions.AddAction(uid, ref comp.PumpActionEntity, "ActionPumpCursedHeart");
    }

    private void OnShutdown(EntityUid uid, CursedHeartComponent comp, ComponentShutdown args)
    {
        _actions.RemoveAction(uid, comp.PumpActionEntity);
    }

    private void OnPump(EntityUid uid, CursedHeartComponent comp, PumpHeartActionEvent args)
    {
        if (args.Handled)
            return;

        args.Handled = true;
        _audio.PlayGlobal(new SoundPathSpecifier("/Audio/_Lavaland/heartbeat.ogg"), uid);
        _damage.TryChangeDamage(uid, new DamageSpecifier(_proto.Index<DamageGroupPrototype>("Brute"), -5), true, false);
        _damage.TryChangeDamage(uid, new DamageSpecifier(_proto.Index<DamageGroupPrototype>("Airloss"), -5), true, false);
        //_bloodstream.TryModifyBloodLevel(uid, 17);
        comp.LastPump = _timing.CurTime;
    }

    private void OnUseInHand(EntityUid uid, CursedHeartGrantComponent comp, UseInHandEvent args)
    {
        if (HasComp<CursedHeartComponent>(args.User))
        {
            _popup.PopupEntity(Loc.GetString("popup-cursed-heart-already-cursed"), args.User, args.User, PopupType.MediumCaution);
            args.Handled = true;
            return;
        }

        _audio.PlayGlobal(new SoundPathSpecifier("/Audio/_Lavaland/heartbeat.ogg"), args.User);
        var heart = EnsureComp<CursedHeartComponent>(args.User);
        heart.LastPump = _timing.CurTime;
        QueueDel(uid);
        args.Handled = true;
    }
}
// SPDX-FileCopyrightText: 2024 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Pieter-Jan Briers <pieterjan.briers@gmail.com>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Visne <39844191+Visne@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2022 Flipp Syder <76629141+vulppine@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto <gradientvera@outlook.com>
// SPDX-FileCopyrightText: 2021 pointer-to-null <91910481+pointer-to-null@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021 Clyybber <darkmine956@gmail.com>
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto <6766154+Zumorica@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 keronshb <54602815+keronshb@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Chief-Engineer <119664036+Chief-Engineer@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Paul Ritter <ritter.paul1@googlemail.com>
// SPDX-FileCopyrightText: 2022 metalgearsloth <metalgearsloth@gmail.com>
// SPDX-FileCopyrightText: 2022 0x6273 <0x40@keemail.me>
// SPDX-FileCopyrightText: 2022 wrexbe <81056464+wrexbe@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 wrexbe <wrexbe@protonmail.com>
// SPDX-FileCopyrightText: 2022 Willhelm53 <97707302+Willhelm53@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 mirrorcult <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2022 Acruid <shatter66@gmail.com>
// SPDX-FileCopyrightText: 2024 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Tomás Alves <tomasalves35@gmail.com>
// SPDX-FileCopyrightText: 2023 TemporalOroboros <TemporalOroboros@gmail.com>
// SPDX-FileCopyrightText: 2023 Kacper Urbańczyk <kacperjaroslawurbanczyk@gmail.com>
// SPDX-FileCopyrightText: 2023 deltanedas <39013340+deltanedas@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 deltanedas <@deltanedas:kde.org>
// SPDX-FileCopyrightText: 2023 Vordenburg <114301317+Vordenburg@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 chromiumboy <50505512+chromiumboy@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Slava0135 <40753025+Slava0135@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Slava0135 <super.novalskiy_0135@inbox.ru>
// SPDX-FileCopyrightText: 2023 Errant <35878406+dmnct@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 faint <46868845+ficcialfaint@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 deltanedas <deltanedas@laptop>
// SPDX-FileCopyrightText: 2023 deltanedas <user@zenith>
// SPDX-FileCopyrightText: 2023 Dawid Bla <46636558+DawBla@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 gluesniffler <159397573+gluesniffler@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Fildrance <fildrance@gmail.com>
// SPDX-FileCopyrightText: 2024 ScarKy0 <scarky0@onet.eu>
// SPDX-FileCopyrightText: 2024 pa.pecherskij <pa.pecherskij@interfax.ru>
// SPDX-FileCopyrightText: 2024 Aidenkrz <aiden@djkraz.com>
// SPDX-FileCopyrightText: 2024 Ed <96445749+TheShuEd@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Varen <ychwack@hotmail.it>
// SPDX-FileCopyrightText: 2024 Jezithyr <jezithyr@gmail.com>
// SPDX-FileCopyrightText: 2025 Aviu00 <93730715+Aviu00@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Server._Goobstation.Wizard.Components;
using Content.Server.Administration.Logs;
using Content.Server.Beam.Components;
using Content.Server.Light.Components;
using Content.Server.NodeContainer;
using Content.Server.NodeContainer.EntitySystems;
using Content.Server.NodeContainer.NodeGroups;
using Content.Server.NodeContainer.Nodes;
using Content.Server.Power.Components;
using Content.Server.Power.EntitySystems;
using Content.Server.Power.NodeGroups;
using Content.Server.Weapons.Melee;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Database;
using Content.Shared.Electrocution;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Inventory;
using Content.Shared.Jittering;
using Content.Shared.Maps;
using Content.Shared.Popups;
using Content.Shared.Speech.EntitySystems;
using Content.Shared.StatusEffect;
using Content.Shared.Stunnable;
using Content.Shared.Tag;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Map;
using Robust.Shared.Physics.Events;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Spawners;
using PullableComponent = Content.Shared.Movement.Pulling.Components.PullableComponent;
using PullerComponent = Content.Shared.Movement.Pulling.Components.PullerComponent;

namespace Content.Server.Electrocution;

public sealed class ElectrocutionSystem : SharedElectrocutionSystem
{
    [Dependency] private readonly IAdminLogManager _adminLogger = default!;
    [Dependency] private readonly IMapManager _mapManager = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly DamageableSystem _damageable = default!;
    [Dependency] private readonly EntityLookupSystem _entityLookup = default!;
    [Dependency] private readonly MeleeWeaponSystem _meleeWeapon = default!;
    [Dependency] private readonly NodeContainerSystem _nodeContainer = default!;
    [Dependency] private readonly NodeGroupSystem _nodeGroup = default!;
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly StatusEffectsSystem _statusEffects = default!;
    [Dependency] private readonly SharedJitteringSystem _jittering = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly SharedStunSystem _stun = default!;
    [Dependency] private readonly SharedStutteringSystem _stuttering = default!;
    [Dependency] private readonly TagSystem _tag = default!;
    [Dependency] private readonly MetaDataSystem _metaData = default!;

    [ValidatePrototypeId<StatusEffectPrototype>]
    private const string StatusEffectKey = "Electrocution";

    [ValidatePrototypeId<DamageTypePrototype>]
    private const string DamageType = "Shock";

    // Yes, this is absurdly small for a reason.
    public const float ElectrifiedDamagePerWatt = 0.0015f; // Goobstation - This information is allowed to be public, and was needed in BatteryElectrocuteChargeSystem.cs
    private const float RecursiveDamageMultiplier = 0.75f;
    private const float RecursiveTimeMultiplier = 0.8f;

    private const float ParalyzeTimeMultiplier = 1f;

    private const float StutteringTimeMultiplier = 1.5f;

    private const float JitterTimeMultiplier = 0.75f;
    private const float JitterAmplitude = 80f;
    private const float JitterFrequency = 8f;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ElectrifiedComponent, StartCollideEvent>(OnElectrifiedStartCollide);
        SubscribeLocalEvent<ElectrifiedComponent, AttackedEvent>(OnElectrifiedAttacked);
        SubscribeLocalEvent<ElectrifiedComponent, InteractHandEvent>(OnElectrifiedHandInteract);
        SubscribeLocalEvent<ElectrifiedComponent, InteractUsingEvent>(OnElectrifiedInteractUsing);
        SubscribeLocalEvent<RandomInsulationComponent, MapInitEvent>(OnRandomInsulationMapInit);
        SubscribeLocalEvent<PoweredLightComponent, AttackedEvent>(OnLightAttacked);

        UpdatesAfter.Add(typeof(PowerNetSystem));
    }

    public override void Update(float frameTime)
    {
        UpdateElectrocutions(frameTime);
        UpdateState(frameTime);
    }

    private void UpdateElectrocutions(float frameTime)
    {
        var query = EntityQueryEnumerator<ElectrocutionComponent, PowerConsumerComponent>();
        while (query.MoveNext(out var uid, out var electrocution, out _))
        {
            var timePassed = Math.Min(frameTime, electrocution.TimeLeft);

            electrocution.TimeLeft -= timePassed;

            if (!MathHelper.CloseTo(electrocution.TimeLeft, 0))
                continue;

            // We tried damage scaling based on power in the past and it really wasn't good.
            // Various scaling types didn't fix tiders and HV grilles instantly critting players.

            QueueDel(uid);
        }
    }

    private void UpdateState(float frameTime)
    {
        var query = EntityQueryEnumerator<ActivatedElectrifiedComponent, ElectrifiedComponent, TransformComponent>();
        while (query.MoveNext(out var uid, out var activated, out var electrified, out var transform))
        {
            activated.TimeLeft -= frameTime;
            if (activated.TimeLeft <= 0 || !IsPowered(uid, electrified, transform))
            {
                _appearance.SetData(uid, ElectrifiedVisuals.ShowSparks, false);
                RemComp<ActivatedElectrifiedComponent>(uid);
            }
        }
    }

    private bool IsPowered(EntityUid uid, ElectrifiedComponent electrified, TransformComponent transform)
    {
        if (!electrified.Enabled)
            return false;
        if (electrified.NoWindowInTile)
        {
            var tileRef = transform.Coordinates.GetTileRef(EntityManager, _mapManager);

            if (tileRef != null)
            {
                foreach (var entity in _entityLookup.GetLocalEntitiesIntersecting(tileRef.Value, flags: LookupFlags.StaticSundries))
                {
                    if (_tag.HasTag(entity, "Window"))
                        return false;
                }
            }
        }
        if (electrified.UsesApcPower)
        {
            if (!this.IsPowered(uid, EntityManager))
                return false;
        }
        else if (electrified.RequirePower && PoweredNode(uid, electrified) == null)
            return false;

        return true;
    }

    private void OnElectrifiedStartCollide(EntityUid uid, ElectrifiedComponent electrified, ref StartCollideEvent args)
    {
        // Goob edit start
        if (!electrified.OnBump)
            return;
        if (TryComp(uid, out BeamComponent? beam))
        {
            var struck = EnsureComp<StruckByLightningComponent>(args.OtherEntity);
            if (!struck.BeamIndices.Add(beam.BeamIndex))
                return;
            if (TryComp(uid, out TimedDespawnComponent? despawn))
                struck.Lifetime = MathF.Max(struck.Lifetime, despawn.Lifetime + 1f);
        }
        TryDoElectrifiedAct(uid, args.OtherEntity, 1, electrified);
        // Goob edit end
    }

    private void OnElectrifiedAttacked(EntityUid uid, ElectrifiedComponent electrified, AttackedEvent args)
    {
        if (!electrified.OnAttacked)
            return;

        if (_meleeWeapon.GetDamage(args.Used, args.User).Empty)
            return;

        TryDoElectrifiedAct(uid, args.User, 1, electrified);
    }

    private void OnElectrifiedHandInteract(EntityUid uid, ElectrifiedComponent electrified, InteractHandEvent args)
    {
        if (electrified.OnHandInteract)
            TryDoElectrifiedAct(uid, args.User, 1, electrified);
    }

    private void OnLightAttacked(EntityUid uid, PoweredLightComponent component, AttackedEvent args)
    {
        if (!component.CurrentLit || args.Used != args.User)
            return;

        if (_meleeWeapon.GetDamage(args.Used, args.User).Empty)
            return;

        DoCommonElectrocution(args.User, uid, component.UnarmedHitShock, component.UnarmedHitStun, false);
    }

    private void OnElectrifiedInteractUsing(EntityUid uid, ElectrifiedComponent electrified, InteractUsingEvent args)
    {
        if (!electrified.OnInteractUsing)
            return;

        var siemens = TryComp<InsulatedComponent>(args.Used, out var insulation)
            ? insulation.Coefficient
            : 1;

        TryDoElectrifiedAct(uid, args.User, siemens, electrified);
    }

    public bool TryDoElectrifiedAct(EntityUid uid, EntityUid targetUid,
        float siemens = 1,
        ElectrifiedComponent? electrified = null,
        NodeContainerComponent? nodeContainer = null,
        TransformComponent? transform = null)
    {
        if (!Resolve(uid, ref electrified, ref transform, false))
            return false;

        if (!IsPowered(uid, electrified, transform))
            return false;

        if (!_random.Prob(electrified.Probability))
            return false;

        EnsureComp<ActivatedElectrifiedComponent>(uid);
        _appearance.SetData(uid, ElectrifiedVisuals.ShowSparks, true);

        siemens *= electrified.SiemensCoefficient;
        if (!DoCommonElectrocutionAttempt(targetUid, uid, ref siemens, electrified.IgnoreInsulation) || siemens <= 0) // Goob edit
            return false; // If electrocution would fail, do nothing.

        var targets = new List<(EntityUid entity, int depth)>();
        GetChainedElectrocutionTargets(targetUid, targets);
        if (!electrified.RequirePower || electrified.UsesApcPower)
        {
            var lastRet = true;
            for (var i = targets.Count - 1; i >= 0; i--)
            {
                var (entity, depth) = targets[i];

                if (entity == electrified.IgnoredEntity) // Goobstation
                    continue;

                lastRet = TryDoElectrocution(
                    entity,
                    uid,
                    (int) (electrified.ShockDamage * MathF.Pow(RecursiveDamageMultiplier, depth)),
                    TimeSpan.FromSeconds(electrified.ShockTime * MathF.Pow(RecursiveTimeMultiplier, depth)),
                    true,
                    electrified.SiemensCoefficient,
                    ignoreInsulation: electrified.IgnoreInsulation // Goobstation
                );
            }
            return lastRet;
        }

        var node = PoweredNode(uid, electrified, nodeContainer);
        if (node?.NodeGroup is not IBasePowerNet)
            return false;

        var (damageScalar, timeScalar) = node.NodeGroupID switch
        {
            NodeGroupID.HVPower => (electrified.HighVoltageDamageMultiplier, electrified.HighVoltageTimeMultiplier),
            NodeGroupID.MVPower => (electrified.MediumVoltageDamageMultiplier, electrified.MediumVoltageTimeMultiplier),
            _ => (1f, 1f)
        };

        {
            var lastRet = true;
            for (var i = targets.Count - 1; i >= 0; i--)
            {
                var (entity, depth) = targets[i];

                if (entity == electrified.IgnoredEntity) // Goobstation
                    continue;

                lastRet = TryDoElectrocutionPowered(
                    entity,
                    uid,
                    node,
                    (int) (electrified.ShockDamage * MathF.Pow(RecursiveDamageMultiplier, depth) * damageScalar),
                    TimeSpan.FromSeconds(electrified.ShockTime * MathF.Pow(RecursiveTimeMultiplier, depth) * timeScalar),
                    true,
                    electrified.SiemensCoefficient,
                    electrified.IgnoreInsulation); // Goob edit
            }
            return lastRet;
        }
    }

    private Node? PoweredNode(EntityUid uid, ElectrifiedComponent electrified, NodeContainerComponent? nodeContainer = null)
    {
        if (!Resolve(uid, ref nodeContainer, false))
            return null;

        return TryNode(electrified.HighVoltageNode) ?? TryNode(electrified.MediumVoltageNode) ?? TryNode(electrified.LowVoltageNode);

        Node? TryNode(string? id)
        {
            if (id != null &&
                _nodeContainer.TryGetNode<Node>(nodeContainer, id, out var tryNode) &&
                tryNode.NodeGroup is IBasePowerNet { NetworkNode: { LastCombinedMaxSupply: > 0 } })
            {
                return tryNode;
            }
            return null;
        }
    }

    /// <inheritdoc/>
    public override bool TryDoElectrocution(
        EntityUid uid, EntityUid? sourceUid, int shockDamage, TimeSpan time, bool refresh, float siemensCoefficient = 1f,
        StatusEffectsComponent? statusEffects = null, bool ignoreInsulation = false)
    {
        if (!DoCommonElectrocutionAttempt(uid, sourceUid, ref siemensCoefficient, ignoreInsulation)
            || !DoCommonElectrocution(uid, sourceUid, shockDamage, time, refresh, siemensCoefficient, statusEffects))
            return false;

        RaiseLocalEvent(uid, new ElectrocutedEvent(uid, sourceUid, siemensCoefficient, shockDamage), true); // Goobstation
        return true;
    }

    private bool TryDoElectrocutionPowered(
        EntityUid uid,
        EntityUid sourceUid,
        Node node,
        int shockDamage,
        TimeSpan time,
        bool refresh,
        float siemensCoefficient = 1f,
        bool ignoreInsulation = false, // Goobstation
        StatusEffectsComponent? statusEffects = null,
        TransformComponent? sourceTransform = null)
    {
        if (!DoCommonElectrocutionAttempt(uid, sourceUid, ref siemensCoefficient, ignoreInsulation)) // Goob edit
            return false;

        if (!DoCommonElectrocution(uid, sourceUid, shockDamage, time, refresh, siemensCoefficient, statusEffects))
            return false;

        // Coefficient needs to be higher than this to do a powered electrocution!
        if (siemensCoefficient <= 0.5f)
            return true;

        if (!Resolve(sourceUid, ref sourceTransform)) // This shouldn't really happen, but just in case...
            return true;

        var electrocutionEntity = Spawn($"VirtualElectrocutionLoad{node.NodeGroupID}", sourceTransform.Coordinates);

        var nodeContainer = Comp<NodeContainerComponent>(electrocutionEntity);

        if (!_nodeContainer.TryGetNode<ElectrocutionNode>(nodeContainer, "electrocution", out var electrocutionNode))
            return false;

        var electrocutionComponent = Comp<ElectrocutionComponent>(electrocutionEntity);

        // This shows up in the power monitor.
        // Yes. Yes exactly.
        _metaData.SetEntityName(electrocutionEntity, MetaData(uid).EntityName);

        electrocutionNode.CableEntity = sourceUid;
        electrocutionNode.NodeName = node.Name;

        _nodeGroup.QueueReflood(electrocutionNode);

        electrocutionComponent.TimeLeft = 1f;
        electrocutionComponent.Electrocuting = uid;
        electrocutionComponent.Source = sourceUid;

        RaiseLocalEvent(uid, new ElectrocutedEvent(uid, sourceUid, siemensCoefficient, shockDamage), true); // Goobstation

        return true;
    }

    private bool DoCommonElectrocutionAttempt(EntityUid uid, EntityUid? sourceUid, ref float siemensCoefficient, bool ignoreInsulation = false)
    {

        var attemptEvent = new ElectrocutionAttemptEvent(uid, sourceUid, siemensCoefficient,
            ignoreInsulation ? SlotFlags.NONE : ~SlotFlags.POCKET);
        RaiseLocalEvent(uid, attemptEvent, true);

        // Cancel the electrocution early, so we don't recursively electrocute anything.
        if (attemptEvent.Cancelled)
            return false;

        siemensCoefficient = attemptEvent.SiemensCoefficient;
        return true;
    }

    private bool DoCommonElectrocution(EntityUid uid, EntityUid? sourceUid,
        int? shockDamage, TimeSpan time, bool refresh, float siemensCoefficient = 1f,
        StatusEffectsComponent? statusEffects = null)
    {
        if (siemensCoefficient <= 0)
            return false;

        if (shockDamage != null)
        {
            shockDamage = (int) (shockDamage * siemensCoefficient);

            if (shockDamage.Value <= 0)
                return false;
        }

        if (!Resolve(uid, ref statusEffects, false) ||
            !_statusEffects.CanApplyEffect(uid, StatusEffectKey, statusEffects))
        {
            return false;
        }

        if (!_statusEffects.TryAddStatusEffect<ElectrocutedComponent>(uid, StatusEffectKey, time, refresh, statusEffects))
            return false;

        var shouldStun = siemensCoefficient > 0.5f;

        if (shouldStun)
            _stun.TryParalyze(uid, time * ParalyzeTimeMultiplier, refresh, statusEffects);

        // TODO: Sparks here.

        if (shockDamage is { } dmg)
        {
            var actual = _damageable.TryChangeDamage(uid,
                new DamageSpecifier(_prototypeManager.Index<DamageTypePrototype>(DamageType), dmg), origin: sourceUid);

            if (actual != null)
            {
                _adminLogger.Add(LogType.Electrocution,
                    $"{ToPrettyString(uid):entity} received {actual.GetTotal():damage} powered electrocution damage{(sourceUid != null ? " from " + ToPrettyString(sourceUid.Value) : ""):source}");
            }
        }

        _stuttering.DoStutter(uid, time * StutteringTimeMultiplier, refresh, statusEffects);
        _jittering.DoJitter(uid, time * JitterTimeMultiplier, refresh, JitterAmplitude, JitterFrequency, true, statusEffects);

        _popup.PopupEntity(Loc.GetString("electrocuted-component-mob-shocked-popup-player"), uid, uid);

        var filter = Filter.PvsExcept(uid, entityManager: EntityManager);

        var identifiedUid = Identity.Entity(uid, ent: EntityManager);
        // TODO: Allow being able to pass EntityUid to Loc...
        if (sourceUid != null)
        {
            _popup.PopupEntity(Loc.GetString("electrocuted-component-mob-shocked-by-source-popup-others",
                ("mob", identifiedUid), ("source", (sourceUid.Value))), uid, filter, true);
            PlayElectrocutionSound(uid, sourceUid.Value);
        }
        else
        {
            _popup.PopupEntity(Loc.GetString("electrocuted-component-mob-shocked-popup-others",
                ("mob", identifiedUid)), uid, filter, true);
        }

        return true;
    }

    private void GetChainedElectrocutionTargets(EntityUid source, List<(EntityUid entity, int depth)> all)
    {
        var visited = new HashSet<EntityUid>();

        GetChainedElectrocutionTargetsRecurse(source, 0, visited, all); // Goob edit
    }

    private void GetChainedElectrocutionTargetsRecurse(
        EntityUid entity,
        int depth,
        HashSet<EntityUid> visited,
        List<(EntityUid entity, int depth)> all)
    {
        all.Add((entity, depth));
        visited.Add(entity);

        if (TryComp<PullableComponent>(entity, out var pullable) &&
            pullable.Puller is { Valid: true } pullerId &&
            !visited.Contains(pullerId))
        {
            GetChainedElectrocutionTargetsRecurse(pullerId, depth + 1, visited, all);
        }

        if (TryComp<PullerComponent>(entity, out var puller) &&
            puller.Pulling is { Valid: true } pullingId &&
            !visited.Contains(pullingId))
        {
            GetChainedElectrocutionTargetsRecurse(pullingId, depth + 1, visited, all);
        }
    }

    private void OnRandomInsulationMapInit(EntityUid uid, RandomInsulationComponent randomInsulation,
        MapInitEvent args)
    {
        if (!TryComp<InsulatedComponent>(uid, out var insulated))
            return;

        if (randomInsulation.List.Length == 0)
            return;

        SetInsulatedSiemensCoefficient(uid, _random.Pick(randomInsulation.List), insulated);
    }

    private void PlayElectrocutionSound(EntityUid targetUid, EntityUid sourceUid, ElectrifiedComponent? electrified = null)
    {
        if (!Resolve(sourceUid, ref electrified, false) || !electrified.PlaySoundOnShock)
        {
            return;
        }
        _audio.PlayPvs(electrified.ShockNoises, targetUid, AudioParams.Default.WithVolume(electrified.ShockVolume));
    }
}
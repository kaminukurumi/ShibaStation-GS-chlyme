// SPDX-FileCopyrightText: 2024 Piras314 <p1r4s@proton.me>
// SPDX-FileCopyrightText: 2024 gluesniffler <159397573+gluesniffler@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.Body.Components;
using Content.Shared._Shitmed.Body.Organ;
using Content.Shared.Eye.Blinding.Components;
using Content.Shared.Eye.Blinding.Systems;

namespace Content.Server.Body.Systems
{
    public sealed class EyesSystem : EntitySystem
    {
        [Dependency] private readonly IEntityManager _entityManager = default!;
        [Dependency] private readonly BlindableSystem _blindableSystem = default!;
        [Dependency] private readonly BodySystem _bodySystem = default!;

        public override void Initialize()
        {
            base.Initialize();

            SubscribeLocalEvent<EyesComponent, OrganEnabledEvent>(OnOrganEnabled);
            SubscribeLocalEvent<EyesComponent, OrganDisabledEvent>(OnOrganDisabled);
        }

        private void HandleSight(EntityUid newEntity, EntityUid oldEntity)
        {
            if (TerminatingOrDeleted(newEntity) || TerminatingOrDeleted(oldEntity))
                return;

            BlindableComponent? newSight;
            BlindableComponent? oldSight;
            //transfer existing component to organ
            if (!TryComp(newEntity, out newSight))
                newSight = EnsureComp<BlindableComponent>(newEntity);

            if (!TryComp(oldEntity, out oldSight))
                oldSight = EnsureComp<BlindableComponent>(oldEntity);

            //give new sight all values of old sight
            _blindableSystem.TransferBlindness(newSight, oldSight, newEntity);

            var hasOtherEyes = false;
            //check for other eye components on owning body and owning body organs (if old entity has a body)
            if (TryComp<BodyComponent>(oldEntity, out var body))
            {
                if (TryComp<EyesComponent>(oldEntity, out var bodyEyes)) //some bodies see through their skin!!! (slimes)
                    hasOtherEyes = true;
                else
                {
                    foreach (var (organ, _) in _bodySystem.GetBodyOrgans(oldEntity, body))
                    {
                        if (TryComp<EyesComponent>(organ, out var eyes))
                        {
                            hasOtherEyes = true;
                            break;
                        }
                    }
                    //TODO (MS14): Should we do this for body parts too? might be a little overpowered but could be funny/interesting
                }
            }

            //if there are no existing eye components for the old entity - set old sight to be blind otherwise leave it as is
            if (!hasOtherEyes && !TryComp<EyesComponent>(oldEntity, out var self))
                _blindableSystem.AdjustEyeDamage((oldEntity, oldSight), oldSight.MaxDamage);

        }

        private void OnOrganEnabled(EntityUid uid, EyesComponent component, OrganEnabledEvent args)
        {
            if (TerminatingOrDeleted(uid)
            || args.Organ.Comp.Body is not { Valid: true } body)
                return;

            RemComp<TemporaryBlindnessComponent>(body);
            HandleSight(uid, body);
        }

        private void OnOrganDisabled(EntityUid uid, EyesComponent component, OrganDisabledEvent args)
        {
            if (TerminatingOrDeleted(uid)
            || args.Organ.Comp.Body is not { Valid: true } body)
                return;

            EnsureComp<TemporaryBlindnessComponent>(body);
            HandleSight(body, uid);
        }
    }
}
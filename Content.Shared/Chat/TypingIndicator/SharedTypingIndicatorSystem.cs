// SPDX-FileCopyrightText: 2022 Alex Evgrashin <aevgrashin@yandex.ru>
// SPDX-FileCopyrightText: 2023 Vordenburg <114301317+Vordenburg@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Morb <14136326+Morb0@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 beck-thompson <107373427+beck-thompson@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Tayrtahn <tayrtahn@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.ActionBlocker;
using Content.Shared.Clothing;
using Content.Shared.Inventory;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Shared.Chat.TypingIndicator;

/// <summary>
///     Supports typing indicators on entities.
/// </summary>
public abstract class SharedTypingIndicatorSystem : EntitySystem
{
    [Dependency] private readonly ActionBlockerSystem _actionBlocker = default!;
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
    [Dependency] private readonly IGameTiming _timing = default!;

    /// <summary>
    ///     Default ID of <see cref="TypingIndicatorPrototype"/>
    /// </summary>
    [ValidatePrototypeId<TypingIndicatorPrototype>]
    public const string InitialIndicatorId = "default";

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<PlayerAttachedEvent>(OnPlayerAttached);
        SubscribeLocalEvent<TypingIndicatorComponent, PlayerDetachedEvent>(OnPlayerDetached);

        SubscribeLocalEvent<TypingIndicatorClothingComponent, ClothingGotEquippedEvent>(OnGotEquipped);
        SubscribeLocalEvent<TypingIndicatorClothingComponent, ClothingGotUnequippedEvent>(OnGotUnequipped);
        SubscribeLocalEvent<TypingIndicatorClothingComponent, InventoryRelayedEvent<BeforeShowTypingIndicatorEvent>>(BeforeShow);

        SubscribeAllEvent<TypingChangedEvent>(OnTypingChanged);
    }

    private void OnPlayerAttached(PlayerAttachedEvent ev)
    {
        // when player poses entity we want to make sure that there is typing indicator
        EnsureComp<TypingIndicatorComponent>(ev.Entity);
        // we also need appearance component to sync visual state
        EnsureComp<AppearanceComponent>(ev.Entity);
    }

    private void OnPlayerDetached(EntityUid uid, TypingIndicatorComponent component, PlayerDetachedEvent args)
    {
        // player left entity body - hide typing indicator
        SetTypingIndicatorEnabled(uid, false);
    }

    private void OnGotEquipped(Entity<TypingIndicatorClothingComponent> entity, ref ClothingGotEquippedEvent args)
    {
        entity.Comp.GotEquippedTime = _timing.CurTime;
    }

    private void OnGotUnequipped(Entity<TypingIndicatorClothingComponent> entity, ref ClothingGotUnequippedEvent args)
    {
        entity.Comp.GotEquippedTime = null;
    }

    private void BeforeShow(Entity<TypingIndicatorClothingComponent> entity, ref InventoryRelayedEvent<BeforeShowTypingIndicatorEvent> args)
    {
        args.Args.TryUpdateTimeAndIndicator(entity.Comp.TypingIndicatorPrototype, entity.Comp.GotEquippedTime);
    }

    private void OnTypingChanged(TypingChangedEvent ev, EntitySessionEventArgs args)
    {
        var uid = args.SenderSession.AttachedEntity;
        if (!Exists(uid))
        {
            Log.Warning($"Client {args.SenderSession} sent TypingChangedEvent without an attached entity.");
            return;
        }

        // check if this entity can speak or emote
        if (!_actionBlocker.CanEmote(uid.Value) && !_actionBlocker.CanSpeak(uid.Value))
        {
            // nah, make sure that typing indicator is disabled
            SetTypingIndicatorEnabled(uid.Value, false);
            return;
        }

        SetTypingIndicatorEnabled(uid.Value, ev.IsTyping);
    }

    private void SetTypingIndicatorEnabled(EntityUid uid, bool isEnabled, AppearanceComponent? appearance = null)
    {
        if (!Resolve(uid, ref appearance, false))
            return;

        _appearance.SetData(uid, TypingIndicatorVisuals.IsTyping, isEnabled, appearance);
    }
}
// SPDX-FileCopyrightText: 2024 PrPleGoo <PrPleGoo@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2024 Rane <60792108+Elijahrane@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 SX_7 <sn1.test.preria.2002@gmail.com>
// SPDX-FileCopyrightText: 2025 Milon <milonpl.git@proton.me>
// SPDX-FileCopyrightText: 2025 Aviu00 <93730715+Aviu00@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.GameTicking;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Robust.Client.Player;
using Robust.Shared.Player;

namespace Content.Client.Overlays;

/// <summary>
/// This is a base system to make it easier to enable or disabling UI elements based on whether or not the player has
/// some component, either on their controlled entity on some worn piece of equipment.
/// </summary>
public abstract class EquipmentHudSystem<T> : EntitySystem where T : IComponent
{
    [Dependency] private readonly IPlayerManager _player = default!;

    [ViewVariables]
    protected bool IsActive;
    protected virtual SlotFlags TargetSlots => ~SlotFlags.POCKET;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<T, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<T, ComponentRemove>(OnRemove);

        SubscribeLocalEvent<LocalPlayerAttachedEvent>(OnPlayerAttached);
        SubscribeLocalEvent<LocalPlayerDetachedEvent>(OnPlayerDetached);

        SubscribeLocalEvent<T, GotEquippedEvent>(OnCompEquip);
        SubscribeLocalEvent<T, GotUnequippedEvent>(OnCompUnequip);

        SubscribeLocalEvent<T, RefreshEquipmentHudEvent<T>>(OnRefreshComponentHud);
        SubscribeLocalEvent<T, InventoryRelayedEvent<RefreshEquipmentHudEvent<T>>>(OnRefreshEquipmentHud);

        SubscribeLocalEvent<RoundRestartCleanupEvent>(OnRoundRestart);
    }

    private void Update(RefreshEquipmentHudEvent<T> ev)
    {
        IsActive = true;
        UpdateInternal(ev);
    }

    public void Deactivate()
    {
        if (!IsActive)
            return;

        IsActive = false;
        DeactivateInternal();
    }

    protected virtual void UpdateInternal(RefreshEquipmentHudEvent<T> args) { }

    protected virtual void DeactivateInternal() { }

    private void OnStartup(Entity<T> ent, ref ComponentStartup args)
    {
        RefreshOverlay();
    }

    private void OnRemove(Entity<T> ent, ref ComponentRemove args)
    {
        RefreshOverlay();
    }

    private void OnPlayerAttached(LocalPlayerAttachedEvent args)
    {
        RefreshOverlay();
    }

    private void OnPlayerDetached(LocalPlayerDetachedEvent args)
    {
        if (_player.LocalSession?.AttachedEntity is null)
            Deactivate();
    }

    private void OnCompEquip(Entity<T> ent, ref GotEquippedEvent args)
    {
        RefreshOverlay();
    }

    private void OnCompUnequip(Entity<T> ent, ref GotUnequippedEvent args)
    {
        RefreshOverlay();
    }

    private void OnRoundRestart(RoundRestartCleanupEvent args)
    {
        Deactivate();
    }

    protected virtual void OnRefreshEquipmentHud(Entity<T> ent, ref InventoryRelayedEvent<RefreshEquipmentHudEvent<T>> args)
    {
        // Goobstation edit
        args.Args.Active = true;
        args.Args.Components.Add(ent.Comp);
    }

    protected virtual void OnRefreshComponentHud(Entity<T> ent, ref RefreshEquipmentHudEvent<T> args)
    {
        args.Active = true;
        args.Components.Add(ent.Comp);
    }

    protected void RefreshOverlay()
    {
        if (_player.LocalSession?.AttachedEntity is not { } entity)
            return;

        var ev = new RefreshEquipmentHudEvent<T>(TargetSlots);
        RaiseLocalEvent(entity, ref ev);

        if (ev.Active)
            Update(ev);
        else
            Deactivate();
    }
}
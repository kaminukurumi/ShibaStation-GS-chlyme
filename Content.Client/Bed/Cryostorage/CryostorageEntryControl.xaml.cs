// SPDX-FileCopyrightText: 2024 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
//
// SPDX-License-Identifier: MIT

using Content.Shared.Bed.Cryostorage;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;

namespace Content.Client.Bed.Cryostorage;

[GenerateTypedNameReferences]
public sealed partial class CryostorageEntryControl : BoxContainer
{
    public event Action<string>? SlotRemoveButtonPressed;
    public event Action<string>? HandRemoveButtonPressed;

    public NetEntity Entity;
    public bool LastOpenState;

    public CryostorageEntryControl(CryostorageContainedPlayerData data)
    {
        RobustXamlLoader.Load(this);
        Entity = data.PlayerEnt;
        Update(data);
    }

    public void Update(CryostorageContainedPlayerData data)
    {
        LastOpenState = Collapsible.BodyVisible;
        Heading.Title = data.PlayerName;
        Body.Visible = data.ItemSlots.Count != 0 && data.HeldItems.Count != 0;

        ItemsContainer.Children.Clear();
        foreach (var (name, itemName) in data.ItemSlots)
        {
            var control = new CryostorageSlotControl(name, itemName);
            control.Button.OnPressed += _ => SlotRemoveButtonPressed?.Invoke(name);
            ItemsContainer.AddChild(control);
        }

        foreach (var (name, held) in data.HeldItems)
        {
            var control = new CryostorageSlotControl(Loc.GetString("cryostorage-ui-filler-hand"), held);
            control.Button.OnPressed += _ => HandRemoveButtonPressed?.Invoke(name);
            ItemsContainer.AddChild(control);
        }
        Collapsible.BodyVisible = LastOpenState;
    }
}
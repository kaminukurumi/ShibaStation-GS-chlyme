// SPDX-FileCopyrightText: 2024 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
//
// SPDX-License-Identifier: MIT

using Content.Client.Message;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;

namespace Content.Client.Bed.Cryostorage;

[GenerateTypedNameReferences]
public sealed partial class CryostorageSlotControl : BoxContainer
{
    public CryostorageSlotControl(string name, string itemName)
    {
        RobustXamlLoader.Load(this);

        SlotLabel.SetMarkup(Loc.GetString("cryostorage-ui-label-slot-name", ("slot", name)));
        ItemLabel.Text = itemName;
    }
}
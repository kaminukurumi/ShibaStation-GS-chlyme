// SPDX-FileCopyrightText: 2024 Winkarst <74284083+Winkarst-cpu@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Prototypes;

namespace Content.Client.VendingMachines.UI;

[GenerateTypedNameReferences]
public sealed partial class VendingMachineItem : BoxContainer
{
    public VendingMachineItem(EntProtoId entProto, string text)
    {
        RobustXamlLoader.Load(this);

        ItemPrototype.SetPrototype(entProto);

        NameLabel.Text = text;
    }

    public void SetText(string text)
    {
        NameLabel.Text = text;
    }
}

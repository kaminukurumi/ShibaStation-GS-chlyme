// SPDX-FileCopyrightText: 2023 Checkraze <71046427+Cheackraze@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Piras314 <p1r4s@proton.me>
// SPDX-FileCopyrightText: 2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Client.UserInterface.Controls;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;

namespace Content.Client.Cargo.UI;

[GenerateTypedNameReferences]
public sealed partial class CargoPalletMenu : FancyWindow
{
    public Action? SellRequested;
    public Action? AppraiseRequested;

    public CargoPalletMenu()
    {
        RobustXamlLoader.Load(this);
        SellButton.OnPressed += OnSellPressed;
        AppraiseButton.OnPressed += OnAppraisePressed;
    }

    public void SetAppraisal(int amount)
    {
        AppraisalLabel.Text = Loc.GetString("cargo-console-menu-points-amount", ("amount", amount.ToString()));
    }

    public void SetCount(int count)
    {
        CountLabel.Text = count.ToString();
    }
    public void SetEnabled(bool enabled)
    {
        AppraiseButton.Disabled = !enabled;
        SellButton.Disabled = !enabled;
    }

    private void OnSellPressed(BaseButton.ButtonEventArgs obj)
    {
        SellRequested?.Invoke();
    }

    private void OnAppraisePressed(BaseButton.ButtonEventArgs obj)
    {
        AppraiseRequested?.Invoke();
    }
}
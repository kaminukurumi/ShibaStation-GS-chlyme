// SPDX-FileCopyrightText: 2024 Piras314 <p1r4s@proton.me>
// SPDX-FileCopyrightText: 2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Client.Stylesheets;
using Robust.Client.AutoGenerated;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;

namespace Content.Client.Salvage.UI;

/// <summary>
/// Generic window for offering multiple selections with a timer.
/// </summary>
[GenerateTypedNameReferences]
public sealed partial class OfferingWindowOption : PanelContainer
{
    private bool _claimed;

    public string? Title
    {
        get => TitleStripe.Text;
        set => TitleStripe.Text = value;
    }

    public event Action<BaseButton.ButtonEventArgs>? ClaimPressed;

    public OfferingWindowOption()
    {
        RobustXamlLoader.Load(this);
        IoCManager.InjectDependencies(this);

        LayoutContainer.SetAnchorPreset(this, LayoutContainer.LayoutPreset.Wide);
        BigPanel.PanelOverride = new StyleBoxFlat(new Color(30, 30, 34));

        ClaimButton.OnPressed += args =>
        {
            ClaimPressed?.Invoke(args);
        };
    }

    public void AddContent(Control control)
    {
        ContentBox.AddChild(control);
    }

    public bool Disabled
    {
        get => ClaimButton.Disabled;
        set => ClaimButton.Disabled = value;
    }

    public bool Claimed
    {
        get => _claimed;
        set
        {
            if (_claimed == value)
                return;

            _claimed = value;

            if (_claimed)
            {
                ClaimButton.AddStyleClass(StyleBase.ButtonCaution);
                ClaimButton.Text = Loc.GetString("offering-window-claimed");
            }
            else
            {
                ClaimButton.RemoveStyleClass(StyleBase.ButtonCaution);
                ClaimButton.Text = Loc.GetString("offering-window-claim");
            }
        }
    }
}
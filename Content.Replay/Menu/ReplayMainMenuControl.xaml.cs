// SPDX-FileCopyrightText: 2023 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Piras314 <p1r4s@proton.me>
// SPDX-FileCopyrightText: 2025 Rane <60792108+Elijahrane@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 bedroomvampire <leannetoni@proton.me>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Client.Resources;
using Content.Client.Stylesheets;
using Robust.Client.AutoGenerated;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;

namespace Content.Replay.Menu;

[GenerateTypedNameReferences]
public sealed partial class ReplayMainMenuControl : Control
{
    public ReplayMainMenuControl(IResourceCache resCache)
    {
        RobustXamlLoader.Load(this);

        LayoutContainer.SetAnchorPreset(this, LayoutContainer.LayoutPreset.Wide);

        LayoutContainer.SetAnchorPreset(VBox, LayoutContainer.LayoutPreset.TopRight);
        LayoutContainer.SetMarginRight(VBox, -25);
        LayoutContainer.SetMarginTop(VBox, 30);
        LayoutContainer.SetGrowHorizontal(VBox, LayoutContainer.GrowDirection.Begin);

        Subtext.FontOverride = resCache.NotoStack2ElectricBoogaloo("/Fonts/NotoSansDisplay/NotoSansDisplay-Bold.ttf", 24); // Goobstation - ZH text support
        var logoTexture = resCache.GetResource<TextureResource>("/Textures/Logo/logo.png");
        Logo.Texture = logoTexture;

        LayoutContainer.SetAnchorPreset(InfoContainer, LayoutContainer.LayoutPreset.BottomLeft);
        LayoutContainer.SetGrowHorizontal(InfoContainer, LayoutContainer.GrowDirection.End);
        LayoutContainer.SetGrowVertical(InfoContainer, LayoutContainer.GrowDirection.Begin);
        InfoContainer.PanelOverride = new StyleBoxFlat()
        {
            BackgroundColor = Color.FromHex("#303033"),
            BorderColor = Color.FromHex("#5a5a5a"),
            BorderThickness = new Thickness(4)
        };
    }
}
// SPDX-FileCopyrightText: 2022 Julian Giebel <juliangiebel@live.de>
// SPDX-FileCopyrightText: 2023 0x6273 <0x40@keemail.me>
// SPDX-FileCopyrightText: 2023 TemporalOroboros <TemporalOroboros@gmail.com>
//
// SPDX-License-Identifier: MIT

using Robust.Client.AutoGenerated;
using Robust.Client.Graphics;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;

namespace Content.Client.PDA;

[GenerateTypedNameReferences]
public sealed partial class PdaProgramItem : ContainerButton
{
    public const string StylePropertyBgColor = "backgroundColor";
    public const string NormalBgColor = "#313138";
    public const string HoverColor = "#3E6C45";

    private readonly StyleBoxFlat _styleBox = new()
    {
        BackgroundColor = Color.FromHex("#25252a"),
    };

    public Color BackgroundColor
    {
        get => _styleBox.BackgroundColor;
        set => _styleBox.BackgroundColor = value;
    }

    public PdaProgramItem()
    {
        RobustXamlLoader.Load(this);
        Panel.PanelOverride = _styleBox;
    }

    protected override void Draw(DrawingHandleScreen handle)
    {
        base.Draw(handle);

        if (TryGetStyleProperty<Color>(StylePropertyBgColor, out var bgColor))
            BackgroundColor = bgColor;

    }
}
// SPDX-FileCopyrightText: 2025 gluesniffler <159397573+gluesniffler@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 coderabbitai[bot] <136622811+coderabbitai[bot]@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Aiden <aiden@djkraz.com>
// SPDX-FileCopyrightText: 2025 FaDeOkno <logkedr18@gmail.com>
// SPDX-FileCopyrightText: 2025 FaDeOkno <143940725+FaDeOkno@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Client.Lathe;
using Content.Shared.Research.Prototypes;
using Robust.Client.AutoGenerated;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Goobstation.Client.Research.UI;

[GenerateTypedNameReferences]
public sealed partial class MiniRecipeCardControl : Control
{
    public MiniRecipeCardControl(TechnologyPrototype technology, LatheRecipePrototype proto, IPrototypeManager prototypeManager, SpriteSystem sprite, LatheSystem lathe)
    {
        RobustXamlLoader.Load(this);

        var discipline = prototypeManager.Index(technology.Discipline);
        Background.ModulateSelfOverride = discipline.Color;
        NameLabel.SetMessage(lathe.GetRecipeName(proto));

        if (proto.Result.HasValue)
            Showcase.Texture = sprite.Frame0(prototypeManager.Index(proto.Result.Value));

        if (proto.Description.HasValue)
        {
            var tooltip = new Tooltip();
            tooltip.SetMessage(FormattedMessage.FromUnformatted(lathe.GetRecipeDescription(proto)));
            Main.TooltipSupplier = _ => tooltip;
        }
    }
}
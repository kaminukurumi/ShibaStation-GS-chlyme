﻿using Content.Shared.Decals;
using Robust.Client.AutoGenerated;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface.CustomControls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Prototypes;

namespace Content.Client.Decals.UI;

[GenerateTypedNameReferences]
public sealed partial class PaletteColorPicker : DefaultWindow
{
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly IResourceCache _resourceCache = default!;

    private readonly TextureResource _tex;

    public PaletteColorPicker()
    {
        RobustXamlLoader.Load(this);
        IoCManager.InjectDependencies(this);

        _tex = _resourceCache.GetResource<TextureResource>("/Textures/Interface/Nano/button.svg.96dpi.png");

        var i = 0;
        foreach (var palette in _prototypeManager.EnumeratePrototypes<ColorPalettePrototype>())
        {
            Palettes.AddItem(palette.Name);
            Palettes.SetItemMetadata(i, palette); // ew
            i += 1;
        }

        Palettes.OnItemSelected += args =>
        {
            Palettes.SelectId(args.Id);
            SetupList();
        };

        Palettes.Select(0);
        SetupList();
    }

    private void SetupList()
    {
        PaletteList.Clear();
        foreach (var (color, value) in (Palettes.SelectedMetadata as ColorPalettePrototype)!.Colors)
        {
            var item = PaletteList.AddItem(color, _tex.Texture);
            item.Metadata = value;
            item.IconModulate = value;
        }
    }
}

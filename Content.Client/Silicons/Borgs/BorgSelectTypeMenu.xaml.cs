// SPDX-FileCopyrightText: 2024 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2024 Piras314 <p1r4s@proton.me>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Linq;
using Content.Client.UserInterface.Controls;
using Content.Shared.Guidebook;
using Content.Shared.Silicons.Borgs;
using Content.Shared.Silicons.Borgs.Components;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Prototypes;

namespace Content.Client.Silicons.Borgs;

/// <summary>
/// Menu used by borgs to select their type.
/// </summary>
/// <seealso cref="BorgSelectTypeUserInterface"/>
/// <seealso cref="BorgSwitchableTypeComponent"/>
[GenerateTypedNameReferences]
public sealed partial class BorgSelectTypeMenu : FancyWindow
{
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;

    private BorgTypePrototype? _selectedBorgType;

    public event Action<ProtoId<BorgTypePrototype>>? ConfirmedBorgType;

    [ValidatePrototypeId<GuideEntryPrototype>]
    private static readonly List<ProtoId<GuideEntryPrototype>> GuidebookEntries = new() { "Cyborgs", "Robotics" };

    public BorgSelectTypeMenu()
    {
        RobustXamlLoader.Load(this);
        IoCManager.InjectDependencies(this);

        var group = new ButtonGroup();
        foreach (var borgType in _prototypeManager.EnumeratePrototypes<BorgTypePrototype>().OrderBy(PrototypeName))
        {
            var button = new Button
            {
                Text = PrototypeName(borgType),
                Group = group,
            };
            button.OnPressed += _ =>
            {
                _selectedBorgType = borgType;
                UpdateInformation(borgType);
            };
            SelectionsContainer.AddChild(button);
        }

        ConfirmTypeButton.OnPressed += ConfirmButtonPressed;
        HelpGuidebookIds = GuidebookEntries;
    }

    private void UpdateInformation(BorgTypePrototype prototype)
    {
        _selectedBorgType = prototype;

        InfoContents.Visible = true;
        InfoPlaceholder.Visible = false;
        ConfirmTypeButton.Disabled = false;

        NameLabel.Text = PrototypeName(prototype);
        DescriptionLabel.Text = Loc.GetString($"borg-type-{prototype.ID}-desc");
        ChassisView.SetPrototype(prototype.DummyPrototype);
    }

    private void ConfirmButtonPressed(BaseButton.ButtonEventArgs obj)
    {
        if (_selectedBorgType == null)
            return;

        ConfirmedBorgType?.Invoke(_selectedBorgType);
    }

    private static string PrototypeName(BorgTypePrototype prototype)
    {
        return Loc.GetString($"borg-type-{prototype.ID}-name");
    }
}
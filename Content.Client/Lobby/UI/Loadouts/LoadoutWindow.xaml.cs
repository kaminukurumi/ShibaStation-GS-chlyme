// SPDX-FileCopyrightText: 2024 Aiden <aiden@djkraz.com>
// SPDX-FileCopyrightText: 2024 Alice "Arimah" Heurlin <30327355+arimah@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Chief-Engineer <119664036+Chief-Engineer@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 DrSmugleaf <10968691+DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Ed <96445749+TheShuEd@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Emisse <99158783+Emisse@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Errant <35878406+Errant-4@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Firewatch <54725557+musicmanvr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Flareguy <78941145+Flareguy@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 HS <81934438+HolySSSS@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 IProduceWidgets <107586145+IProduceWidgets@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 JustCone <141039037+JustCone14@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Mervill <mervills.email@gmail.com>
// SPDX-FileCopyrightText: 2024 Mr. 27 <45323883+Dutch-VanDerLinde@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Mr. 27 <koolthunder019@gmail.com>
// SPDX-FileCopyrightText: 2024 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 PJBot <pieterjan.briers+bot@gmail.com>
// SPDX-FileCopyrightText: 2024 Piras314 <p1r4s@proton.me>
// SPDX-FileCopyrightText: 2024 Plykiya <58439124+Plykiya@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 PopGamer46 <yt1popgamer@gmail.com>
// SPDX-FileCopyrightText: 2024 Rouge2t7 <81053047+Sarahon@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Spessmann <156740760+Spessmann@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Truoizys <153248924+Truoizys@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 TsjipTsjip <19798667+TsjipTsjip@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Ubaser <134914314+UbaserB@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Vasilis <vasilis@pikachu.systems>
// SPDX-FileCopyrightText: 2024 Winkarst <74284083+Winkarst-cpu@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 beck-thompson <107373427+beck-thompson@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 coolboy911 <85909253+coolboy911@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 deltanedas <39013340+deltanedas@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 deltanedas <@deltanedas:kde.org>
// SPDX-FileCopyrightText: 2024 lunarcomets <140772713+lunarcomets@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 osjarw <62134478+osjarw@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 plykiya <plykiya@protonmail.com>
// SPDX-FileCopyrightText: 2024 saintmuntzer <47153094+saintmuntzer@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Арт <123451459+JustArt1m@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 ArtisticRoomba <145879011+ArtisticRoomba@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2025 Tayrtahn <tayrtahn@gmail.com>
// SPDX-FileCopyrightText: 2025 lzk <124214523+lzk228@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Numerics;
using Content.Client.UserInterface.Controls;
using Content.Shared.Dataset;
using Content.Shared.Preferences;
using Content.Shared.Preferences.Loadouts;
using Content.Shared.Random.Helpers;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Client.Lobby.UI.Loadouts;

[GenerateTypedNameReferences]
public sealed partial class LoadoutWindow : FancyWindow
{
    public event Action<string>? OnNameChanged;
    public event Action<ProtoId<LoadoutGroupPrototype>, ProtoId<LoadoutPrototype>>? OnLoadoutPressed;
    public event Action<ProtoId<LoadoutGroupPrototype>, ProtoId<LoadoutPrototype>>? OnLoadoutUnpressed;

    private List<LoadoutGroupContainer> _groups = new();

    public HumanoidCharacterProfile Profile;

    public LoadoutWindow(HumanoidCharacterProfile profile, RoleLoadout loadout, RoleLoadoutPrototype proto, ICommonSession session, IDependencyCollection collection)
    {
        RobustXamlLoader.Load(this);
        Profile = profile;
        var protoManager = collection.Resolve<IPrototypeManager>();
        RoleNameEdit.IsValid = text => text.Length <= HumanoidCharacterProfile.MaxLoadoutNameLength;

        // Hide if we can't edit the name.
        if (!proto.CanCustomizeName)
        {
            RoleNameBox.Visible = false;
        }
        else
        {
            var name = loadout.EntityName;

            LoadoutNameLabel.Text = proto.NameDataset == null ?
                Loc.GetString("loadout-name-edit-label") :
                Loc.GetString("loadout-name-edit-label-dataset");

            RoleNameEdit.ToolTip = Loc.GetString(
                "loadout-name-edit-tooltip",
                ("max", HumanoidCharacterProfile.MaxLoadoutNameLength));
            RoleNameEdit.Text = name ?? string.Empty;
            RoleNameEdit.OnTextChanged += args => OnNameChanged?.Invoke(args.Text);
        }

        // Hide if no groups
        if (proto.Groups.Count == 0)
        {
            LoadoutGroupsContainer.Visible = false;
            SetSize = Vector2.Zero;
        }
        else
        {
            foreach (var group in proto.Groups)
            {
                if (!protoManager.TryIndex(group, out var groupProto))
                    continue;

                if (groupProto.Hidden)
                    continue;

                var container = new LoadoutGroupContainer(profile, loadout, protoManager.Index(group), session, collection);
                LoadoutGroupsContainer.AddTab(container, Loc.GetString(groupProto.Name));
                _groups.Add(container);

                container.OnLoadoutPressed += args =>
                {
                    OnLoadoutPressed?.Invoke(group, args);
                };

                container.OnLoadoutUnpressed += args =>
                {
                    OnLoadoutUnpressed?.Invoke(group, args);
                };
            }
        }
    }

    public void RefreshLoadouts(RoleLoadout loadout, ICommonSession session, IDependencyCollection collection)
    {
        foreach (var group in _groups)
        {
            group.RefreshLoadouts(Profile, loadout, session, collection);
        }
    }
}